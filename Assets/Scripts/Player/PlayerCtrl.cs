using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;


///////////////////////////////////////////////////////
//////// PunRPC 사용 시 주의사항
//////////////////////////////////////////////////////
///// 1. RPC 사용을 위한 PhotonView는 해당 오브젝트가 photonView 스크립트가 붙어있어야 한다.
///// 2. RPC 사용을 위한 매개변수는 GameObject나 Transform과 같은 형식은 사용할 수 없다.
///// 3. 매개변수가 꼭 필요한 경우 photonview에 있는 ViewID(int형)로 접근하여 하이에라키에 존재하는 ViewID와 비교하여 찾을 수 있다.


public class PlayerCtrl : MonoBehaviourPunCallbacks
{
    // enum 클래스 플레이어 상태
    public enum State
    {
        NORMAL, MOVE, ROLLING, ATTACK, HIT, DIE
    }
    public float movePower = 5f; // 이동에 필요한 힘
    public float rollSpeed;  // 구르는 속도
    public float attackDistanceSpeed; // 공격 시 이동하는 속도
    public float rollCoolTime = 0.0f; // 구르기 쿨타임

    [SerializeField] private bool isPlayerInRangeOfEnemy = false; // 공격가능한 범위인지 아닌지?
    private EnemyCtrl enemyCtrl = null; // 공격한 적의 정보

    [SerializeField] private bool isAttackCooldownOver = true;
    [SerializeField] private float attackCoolTime = 1.0f;

    //[SerializeField] private GameObject playerStat;

    public RuntimeAnimatorController[] animController; // 아이템 획득 시 변경할 애니메이터

    PhotonView pv, weaponPV; // 플레이어 pv, 무기 pv
    Rigidbody2D rigid; // 플레이어 리지드 바디

    Vector3 moveDir, rollDir, attackDir; // 이동 방향, 구르기 방향, 공격 방향

    Animator anim; // 플레이어 애니메이터
    SpriteRenderer spriteRenderer; // 피격 시 색 변경을 위한 스프라이트 렌더러
    private Status status; // 플레이어 상태 스크립트
    private Chat chatScript;
    private PartySystem partySystemScript;

    public string weaponName = "None"; // 초기에는 아무 무기가 없음.

    [SerializeField] private State state; // enum 클래스 변수

    public bool isPartyMember = false; // 파티에 이미 속해있는 상태인지 아닌지 확인하는 변수
    public bool isReady = false; // 파티에 이미 속해있고 던전 입장 준비가 됐는지 확인하는 변수

    public Party party;

    GameObject inventory;

    Vector3 mouseWorldPosition;
    // Getter
    public State GetState()
    {
        return state;
    }

    void Start()
    {
        // 변수 초기화
        rigid = gameObject.GetComponent<Rigidbody2D>();
        pv = this.GetComponent<PhotonView>();
        //playerStat = GameObject.FindGameObjectWithTag("PlayerStat");
        anim = this.GetComponent<Animator>();
        status = this.GetComponent<Status>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        chatScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Chat>();
        partySystemScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PartySystem>();

        state = State.NORMAL;
        //weaponPV = null;
        //party = null;

        //partySystemScript.partyCreator.transform.GetComponentInChildren<Button>().onClick.AddListener(OnPartyCreationComplete);

        // 임시로 캔버스 자식 직접 지정(수정 필요)
        inventory = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0).gameObject;

    }

    //Graphic & Input Updates	
    void Update()
    {
        if (pv.IsMine)
        {
            moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (state != State.ATTACK && !chatScript.inputField.isFocused)
            {
                if (moveDir.x != 0 || moveDir.y != 0)
                {
                    state = State.MOVE;
                }
                else
                {
                    rigid.velocity = Vector2.zero;
                    state = State.NORMAL;
                }
            }

            // 공격 & 공격 쿨타임 끝나면
            if (Input.GetMouseButtonDown(0) && isAttackCooldownOver && !EventSystem.current.currentSelectedGameObject)
            {
                state = State.ATTACK;

                Vector3 mouseScreenPosition = Input.mousePosition;

                // 마우스의 스크린 좌표를 월드 좌표로 변환합니다.
                mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
                // 플레이어가 보고 있는 방향에 따른 공격방향
                //SetDirection();

                attackDistanceSpeed = 14f;
                isAttackCooldownOver = false;

                // 적을 공격한 상태.
                if (isPlayerInRangeOfEnemy)
                {
                    enemyCtrl.GetComponent<PhotonView>().RPC("EnemyAttackedPlayer", RpcTarget.All, status.attackDamage);
                }
            }

            if (!isAttackCooldownOver)
            {
                if (attackCoolTime > 0.0f)
                {
                    attackCoolTime -= Time.deltaTime;
                }
                else
                {
                    isAttackCooldownOver = true;
                    attackCoolTime = 1.0f;
                }
            }

            // 채팅 입력
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!chatScript.inputField.isFocused)
                {
                    chatScript.inputField.interactable = true;
                    chatScript.inputField.ActivateInputField();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                chatScript.inputField.text = "";
                chatScript.inputField.DeactivateInputField();
                chatScript.inputField.interactable = false;

                chatScript.CloseChatWindowOnButtonClick();
            }
            
            // 인벤토리 열기
            if (Input.GetKeyDown(KeyCode.I) && !chatScript.inputField.isFocused)
            {
                inventory.SetActive(!inventory.activeSelf);
            }

            IsPartyHUDActive();
        }
    }

    void LateUpdate()
    {
        if (pv.IsMine)
        {
            // 상태에 따른 함수 실행
            switch (state)
            {
                case State.NORMAL:
                    IdleAnimation();
                    break;
                case State.MOVE:
                    Move();
                    MoveAnimation();
                    break;
                case State.ATTACK:
                    AttackAnimation();
                    Attack();
                    break;
            }
        }
    }


    void IdleAnimation()
    {
        anim.SetBool("isMove", false);
        anim.SetBool("isAttack", false);
        //anim.SetBool("AttackIdle", false);
    }

    void MoveAnimation()
    {
        anim.SetBool("isMove", true);
        anim.SetBool("isAttack", false);
        //anim.SetBool("AttackIdle", false);
    }

    void AttackAnimation()
    {
        anim.SetBool("isAttack", true);
        //anim.SetBool("AttackIdle", false);
        anim.SetBool("isMove", false);
    }

    // 이동
    void Move()
    {
        moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 플레이어 좌, 우 스케일 값 변경 (뒤집기)
        if (moveDir.x > 0.0f)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveDir.x < 0.0f)
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }

        rigid.velocity = moveDir * movePower;
    }

    void Attack()
    {
        // 방향벡터 x좌표의 값에 따른 캐릭터 반전
        if (mouseWorldPosition.x - this.transform.position.x > 0)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);

        }

        // 문제점 : 현재 마우스 위치가 가까우면 플레이어가 조금만 이동함.
        rigid.velocity = (mouseWorldPosition - this.transform.position).normalized * attackDistanceSpeed;

        float attackSpeedDropMultiplier = 7f; // 감소되는 정도
        attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime; // 실제 나아가는 거리 계산

        if (attackDistanceSpeed < 0.8f)
        {
            rigid.velocity = Vector2.zero;
            state = State.NORMAL;
        }
    }

    //// 2D 캐릭터 방향 결정
    //void SetDirection()
    //{

    //    if (Input.mousePosition.x - this.transform.position.x > 0)
    //    {
    //        attackDir = -transform.right * (Input.mousePosition - this.transform.position);
    //    }
    //    else
    //    {
    //        attackDir = transform.right * (Input.mousePosition - this.transform.position);
    //    }
    //}

    // 플레이어가 파티 방에 속해있는지 확인 후에 정보를 전달해주는 함수
    public void IsPartyHUDActive()
    {
        if (!isPartyMember)
        {
            partySystemScript.partyMemberHUD[0].GetComponentInChildren<Text>().text = "";
            partySystemScript.partyMemberHUD[1].GetComponentInChildren<Text>().text = "";

            partySystemScript.partyMemberHUD[0].SetActive(false);
            partySystemScript.partyMemberHUD[1].SetActive(false);
            return;
        }

        if (party != null)
        {
            if (party.GetPartyHeadCount() == 1)
            {
                PhotonView partyLeaderPhotonView = PhotonView.Find(party.GetPartyLeaderID());
                partySystemScript.partyMemberHUD[0].GetComponentInChildren<Text>().text = partyLeaderPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[0].SetActive(true);
            }
            else
            {
                PhotonView partyLeaderPhotonView = PhotonView.Find(party.GetPartyLeaderID());
                partySystemScript.partyMemberHUD[0].GetComponentInChildren<Text>().text = partyLeaderPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[0].SetActive(true);

                PhotonView partyMemberPhotonView = PhotonView.Find(party.GetPartyMemberID());
                partySystemScript.partyMemberHUD[1].GetComponentInChildren<Text>().text = partyMemberPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[1].SetActive(true);
            }
        }
    }


    [PunRPC]
    void GetWeapon(int viewID)
    {
        // RPC로 전달된 PhotonView의 ID를 사용하여 해당 오브젝트를 찾음
        PhotonView targetPhotonView = PhotonView.Find(viewID);

        // 해당 viewID가 존재하는 경우
        if (targetPhotonView != null)
        {
            string targetName = targetPhotonView.gameObject.name;

            // 현 무기이름을 저장 (-7을 한 이유는 Clone 부분의 문자열을 빼주기 위함
            weaponName = targetName.Substring(0, targetName.Length - 7);

            // 무기교체 시 애니메이터 교체 (수정 필요)
            SetAnimationController(weaponName);

            //PlayerStatDisplay playerStatDisplay = playerStat.GetComponent<PlayerStatDisplay>();

            // 현 플레이어 스탯 + 무기 스탯
            ItemPickUp newItem = targetPhotonView.GetComponent<ItemPickUp>();
            int atkDamage = status.attackDamage + newItem.GetDamage();
            float atkSpeed = status.attackSpeed + newItem.GetSpeed();

            // 적용
            status.attackDamage = atkDamage;
            status.attackSpeed = atkSpeed;

            PhotonView.Destroy(targetPhotonView.gameObject);
        }
        else
        {
            Debug.Log("Not Found");
        }
    }

    // 애니메이터 교체
    void SetAnimationController(string name)
    {
        if (name.Equals("Sword"))
        {
            anim.runtimeAnimatorController = animController[0];
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        isPlayerInRangeOfEnemy = true;
    //        enemyCtrl = other.GetComponent<EnemyCtrl>();

    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    isPlayerInRangeOfEnemy = false;
    //    enemyCtrl = null;
    //}
}
