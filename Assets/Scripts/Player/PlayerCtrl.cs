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
        NORMAL, ROLLING, ATTACK, ATTACKIDLE, HIT, DIE
    }
    public float movePower = 5f; // 이동에 필요한 힘
    public float rollSpeed;  // 구르는 속도
    public float attackDistanceSpeed; // 공격 시 이동하는 속도
    public float rollCoolTime = 0.0f; // 구르기 쿨타임
    public float attackIdleTime = 0.0f; // 공격 시 공격준비상태

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

    public Party party;

    // 상태 변경을 위한 함수
    [PunRPC]
    public void UpdatePlayerState(State newState)
    {
        state = newState;
    }

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
        weaponPV = null;
        party = null;

        //partySystemScript.partyCreator.transform.GetComponentInChildren<Button>().onClick.AddListener(OnPartyCreationComplete);

    }

    void FixedUpdate()
    {   
        if (pv.IsMine)
        {
            // 아이템 줍기
            if (weaponPV != null && Input.GetKeyDown(KeyCode.G))
            {
                PickUp();
            }
            
            // 구르기
            // if (rollCoolTime <= 0.0f)
            // {
            // 	rollCoolTime = 0.0f;
            // 	if (Input.GetKeyDown(KeyCode.Space))
            // 	{
            // 		rollCoolTime = 2.0f;
            //     	rollDir = moveDir;
            //     	rollSpeed = 40f;
            //     	state = State.ROLLING;
            // 	}
            // }
            // else
            // {
            // 	rollCoolTime -= Time.deltaTime;
            // }
            
            // 공격
            
            if (Input.GetMouseButtonDown(0) &&
                !chatScript.inputField.isFocused &&
                !partySystemScript.partyCreator.activeSelf &&
                !partySystemScript.partyView.activeSelf &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                state = State.ATTACK;
                anim.SetTrigger("Attack");

                // 플레이어가 보고 있는 방향에 따른 공격방향
                SetDirection();

                attackDistanceSpeed = 6f;
            }

            IsPartyHUDActive();
        }
    }
    //Graphic & Input Updates	
    void Update()
    {
        if (pv.IsMine)
        {
            // 상태에 따른 애니메이션 실행
            switch (state)
            {
                case State.NORMAL:
                    break;
                case State.ROLLING:
                    anim.SetTrigger("Rolling");
                    break;
                case State.ATTACK:
                    anim.SetBool("isMove", false);
                    break;
                case State.ATTACKIDLE:
                    anim.SetBool("AttackIdle", true);
                    break;
                case State.HIT:
                    break;
                case State.DIE:
                    break;
            }
        }
    }

    void LateUpdate()
    {
        if (pv.IsMine && 
            !chatScript.inputField.isFocused &&
            !partySystemScript.partyCreator.activeSelf &&
            !partySystemScript.partyView.activeSelf)
        {
            // 상태에 따른 함수 실행
            switch (state)
            {
                case State.NORMAL:
                    Move();
                    break;
                case State.ROLLING:
                    StartCoroutine(Roll());
                    break;
                case State.ATTACK:
                    StartCoroutine(Attack());
                    break;
                case State.ATTACKIDLE:
                    StartCoroutine(AttackIdle());
                    break;
                case State.HIT:
                    StartCoroutine(OnDamaged());
                    break;
                case State.DIE:
                    break;
            }
        }
    }

    // 이동
    void Move()
    {
        if (!chatScript.inputField.isFocused)
            moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (moveDir.x != 0 || moveDir.y != 0 || anim.GetBool("AttackIdle"))
        {
            anim.SetBool("AttackIdle", false);
            anim.SetBool("isMove", true);
        }
        else
        {
            anim.SetBool("isMove", false);
        }

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

    // 2D 캐릭터 방향 결정
    void SetDirection()
    {
        if (this.transform.localScale.x == 1)
        {
            attackDir = transform.right;
        }
        else
        {
            attackDir = -transform.right;
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.2f);
        rigid.velocity = attackDir.normalized * attackDistanceSpeed;
        
        
        float attackSpeedDropMultiplier = 7f; // 감소되는 정도
        attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime; // 실제 나아가는 거리 계산

        float attackSpeedMinimum = 3f;


        if (attackDistanceSpeed < attackSpeedMinimum)
        {
            state = State.ATTACKIDLE;
        }
    }

    // 공격 이후 공격준비상태
    IEnumerator AttackIdle()
    {
        yield return new WaitForSeconds(1.0f);
        attackIdleTime = 0.0f;

        anim.SetBool("AttackIdle", false);
        state = State.NORMAL;
    }


    // 구르기 (계산은 공격하고 동일)
    IEnumerator Roll()
    {
        yield return new WaitForSeconds(0.1f);
        rigid.velocity = rollDir.normalized * rollSpeed;


        float rollSpeedDropMultiplier = 5f;
        rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

        float rollSpeedMinimum = 10f;
        if (rollSpeed < rollSpeedMinimum)
        {
            state = State.NORMAL;
        }
    }

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
            if (party.partyMembers.Count == 1)
            {
                PhotonView partyLeaderPhotonView = PhotonView.Find(party.partyMembers[0]);
                partySystemScript.partyMemberHUD[0].GetComponentInChildren<Text>().text = partyLeaderPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[0].SetActive(true);

                if (partySystemScript.partyMemberHUD[1].activeSelf)
                {
                    partySystemScript.partyMemberHUD[1].SetActive(false);
                }
            }
            else
            {
                PhotonView partyLeaderPhotonView = PhotonView.Find(party.partyMembers[0]);
                partySystemScript.partyMemberHUD[0].GetComponentInChildren<Text>().text = partyLeaderPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[0].SetActive(true);

                PhotonView partyMemberPhotonView = PhotonView.Find(party.partyMembers[1]);
                partySystemScript.partyMemberHUD[1].GetComponentInChildren<Text>().text = partyMemberPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[1].SetActive(true);
            }
        }
    }

    // 아이템 줍기
    void PickUp()
    {
        pv.RPC("GetWeapon", RpcTarget.All, weaponPV.ViewID);
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

    // 피격 시 일정 시간 동안 색 변경
    IEnumerator OnDamaged()
    {
        state = State.NORMAL;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.15f);

        spriteRenderer.color = new Color(1, 1, 1, 1);
        if (status.HP <= 0)
        {
            anim.SetTrigger("Die");
            StartCoroutine(Death());
            state = State.DIE;
        }
    }

    // 사망
    IEnumerator Death()
    {
        yield return new WaitForSeconds(2f);
        this.transform.position = Vector2.zero;
        anim.SetTrigger("Idle");
        state = State.NORMAL;
    }

    // 애니메이터 교체
    void SetAnimationController(string name)
    {
        if (name.Equals("Sword"))
        {
            anim.runtimeAnimatorController = animController[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            weaponPV = other.GetComponent<PhotonView>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            weaponPV = null;
        }
    }
}