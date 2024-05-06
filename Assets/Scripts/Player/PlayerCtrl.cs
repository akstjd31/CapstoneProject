using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;
using Photon.Voice.Unity;
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

    GameObject canvas;

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

    private bool isLobbyScene = false;  //DungeonEntrance와 플레이어의 충돌을 감지하기 위해 현재 씬이 LobbyScene인지 확인하는 변수
    GameObject DungeonCanvas;
    RawImage DungeonImage;
    Button destroyButton;

    GameObject inventory;

    Vector3 mouseWorldPosition;

    Recorder recorder;

    UIManager uiManager;

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

        //PhotonNetwork.NickName = pv.ViewID.ToString(); // 임시로 플레이어 닉네임을 ViewID로 설정. (다른 클래스에서 닉네임을 비교하기 때문에)

        // 공용
        //playerStat = GameObject.FindGameObjectWithTag("PlayerStat");
        anim = this.GetComponent<Animator>();
        status = this.GetComponent<Status>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        inventory = canvas.transform.Find("Inventory").gameObject;
        

        state = State.NORMAL;

        // 로비 씬
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            chatScript = canvas.GetComponent<Chat>();
            partySystemScript = canvas.GetComponent<PartySystem>();

            //던전 선택 canvas 생성
            MakeDungeonMap();
        }

        // 던전 씬
        else if (SceneManager.GetActiveScene().name == "DungeonScene")
        {
            recorder = GameObject.Find("VoiceManager").GetComponent<Recorder>();
            uiManager = canvas.GetComponent<UIManager>();

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject otherPlayer = players[0] == this.gameObject ? players[1] : players[0];

            HUD hud1 = uiManager.hud1.GetComponent<HUD>();

            if (pv.IsMine)
            {
                // HUD1은 로컬 플레이어
                hud1.nickName.text = PhotonNetwork.NickName;
                hud1.hpBar.value = status.HP;

                // HUD2 == otherPlayer
                HUD hud2 = uiManager.hud2.GetComponent<HUD>();
                hud2.nickName.text = otherPlayer.GetComponent<PhotonView>().Controller.NickName;
                hud2.hpBar.value = otherPlayer.GetComponent<Status>().HP;
            }
        }
    }

    //Graphic & Input Updates	
    void Update()
    {
        if (pv.IsMine)
        {
            moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if ((state != State.ATTACK && chatScript != null && partySystemScript != null &&
                !chatScript.chatView.activeSelf && !partySystemScript.partyCreator.activeSelf) ||
                (state != State.ATTACK && SceneManager.GetActiveScene().name == "DungeonScene"))
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
            if ((Input.GetMouseButtonDown(0) && isAttackCooldownOver && 
                !EventSystem.current.currentSelectedGameObject &&
                chatScript != null && partySystemScript != null && 
                !inventory.activeSelf && !chatScript.chatView.activeSelf &&
                !partySystemScript.partyCreator.activeSelf && !partySystemScript.partyView.activeSelf) ||
                (Input.GetMouseButtonDown(0) && SceneManager.GetActiveScene().name == "DungeonScene"))
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
            if (Input.GetKeyDown(KeyCode.Return) && partySystemScript != null &&
                !partySystemScript.partyCreator.activeSelf)
            {
                if (!chatScript.inputField.isFocused)
                {
                    chatScript.inputField.interactable = true;
                    chatScript.inputField.ActivateInputField();
                }
            }

            // 채팅 끄기
            if (Input.GetKeyDown(KeyCode.Escape) && partySystemScript != null &&
                !partySystemScript.partyCreator.activeSelf)
            {
                chatScript.inputField.text = "";
                chatScript.inputField.DeactivateInputField();

                chatScript.CloseChatWindowOnButtonClick();
            }
            
            // 인벤토리 열기
            if (Input.GetKeyDown(KeyCode.I) && chatScript != null && partySystemScript != null &&
                !chatScript.chatView.activeSelf && !partySystemScript.partyCreator.activeSelf)
            {
                inventory.SetActive(!inventory.activeSelf);
            }

            // 보이스 참가하기
            if (recorder != null)
            {
                if (Input.GetKey(KeyCode.T))
                {
                    recorder.TransmitEnabled = true;
                }
                else
                {
                    recorder.TransmitEnabled = false;
                }
            }

            if (partySystemScript != null)
            {
                IsPartyHUDActive();
            }
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
    }

    void MoveAnimation()
    {
        anim.SetBool("isMove", true);
        anim.SetBool("isAttack", false);
    }

    void AttackAnimation()
    {
        anim.SetBool("isAttack", true);
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

        float attackSpeedDropMultiplier = 5f; // 감소되는 정도
        attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime; // 실제 나아가는 거리 계산

        if (attackDistanceSpeed < 0.5f)
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
        if (party != null)
        {
            if (party.GetPartyHeadCount() == 1)
            {
                PhotonView partyLeaderPhotonView = PhotonView.Find(party.GetPartyLeaderID());
                partySystemScript.partyMemberHUD[0].transform.GetChild(0).GetComponentInChildren<Text>().text = partyLeaderPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[0].SetActive(true);
                if(partySystemScript.partyMemberHUD[1].activeSelf)
                {
                    partySystemScript.partyMemberHUD[1].SetActive(false);
                }
            }
            else
            {
                PhotonView partyLeaderPhotonView = PhotonView.Find(party.GetPartyLeaderID());
                partySystemScript.partyMemberHUD[0].transform.GetChild(0).GetComponentInChildren<Text>().text = partyLeaderPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[0].SetActive(true);

                PhotonView partyMemberPhotonView = PhotonView.Find(party.GetPartyMemberID());
                partySystemScript.partyMemberHUD[1].transform.GetChild(0).GetComponentInChildren<Text>().text = partyMemberPhotonView.Owner.NickName;
                partySystemScript.partyMemberHUD[1].SetActive(true);
            }
        }
        else
        {
            partySystemScript.partyMemberHUD[0].transform.GetChild(0).GetComponentInChildren<Text>().text = "";
            partySystemScript.partyMemberHUD[1].transform.GetChild(0).GetComponentInChildren<Text>().text = "";

            partySystemScript.partyMemberHUD[0].transform.Find("Ready").gameObject.SetActive(false);
            partySystemScript.partyMemberHUD[1].transform.Find("Ready").gameObject.SetActive(false);

            partySystemScript.partyMemberHUD[0].SetActive(false);
            partySystemScript.partyMemberHUD[1].SetActive(false);
            return;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isLobbyScene && other.name == "DungeonEntrance") //&& DungeonEnterCondition()
        {
            //던전 선택창 출력
            DungeonCanvas.SetActive(true);
        }

        if (other.CompareTag("Weapon")) {
            
        }
        //if (other.CompareTag("Enemy"))
        //{
        //    isPlayerInRangeOfEnemy = true;
        //    enemyCtrl = other.GetComponent<EnemyCtrl>();

        //}
    }

    private void MakeDungeonMap()
    {
        DungeonCanvas = new GameObject("RawImage");
        DungeonImage = DungeonCanvas.AddComponent<RawImage>();
        DungeonCanvas.transform.SetParent(GameObject.Find("Canvas").transform, false);
        DungeonCanvas.AddComponent<RectTransform>();
        DungeonCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);

        string imagePath = "dungeon_enter_map";
        Texture2D texture = Resources.Load<Texture2D>(imagePath);
        Debug.Log($"texture is null : {texture == null}");
        DungeonCanvas.GetComponent<RawImage>().texture = texture;

        // 버튼 GameObject 생성
        GameObject buttonObject = new GameObject("DestroyButton");

        // 생성한 GameObject에 Button 컴포넌트 추가
        destroyButton = buttonObject.AddComponent<Button>();

        // 버튼에 텍스트 추가
        Text buttonText = buttonObject.AddComponent<Text>();
        buttonText.text = "X";
        buttonText.fontSize = 56;
        buttonText.color = Color.black;

        // 생성한 GameObject를 Canvas의 자식으로 설정
        buttonObject.transform.SetParent(GameObject.Find("Canvas").transform, false);

        // RectTransform 컴포넌트 가져오기
        RectTransform buttonTransform = buttonObject.GetComponent<RectTransform>();

        // 버튼의 위치를 설정 (Canvas의 우측 상단)
        buttonTransform.anchorMin = new Vector2(1f, 1f);
        buttonTransform.anchorMax = new Vector2(1f, 1f);
        buttonTransform.pivot = new Vector2(1f, 1f);
        buttonTransform.anchoredPosition = new Vector2(-10f, -10f); // 원하는 위치로 조정

        // 버튼이 클릭되었을 때의 동작 설정
        destroyButton.onClick.AddListener(DisableDungeonCanvas);

        DungeonCanvas.SetActive(false);
    }


    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     isPlayerInRangeOfEnemy = false;
    //     enemyCtrl = null;
    // }

    private bool DungeonEnterCondition()
    {
        Debug.Log($"name : {pv.name}");

        //파티에 속하지 않은 경우
        if (!isPartyMember)
        {
            Debug.Log("파티가 없습니다");
        }
        //2인 파티가 아닌 경우
        else if (party.GetPartyHeadCount() != 2)
        {
            Debug.Log("2인 파티가 아닙니다");
        }
        //모든 인원이 준비 완료 상태인지
        else if (false)     //준비 기능 미구현 상태
        {

        }
        //본인이 파티장이 아닌 경우
        else if (!PhotonNetwork.NickName.Equals(party.GetPartyLeaderID()))
        {
            Debug.Log("파티장이 아닙니다");
        }
        

        return false;
    }

    private void DisableDungeonCanvas()
    {
        DungeonCanvas.SetActive(false);
    }
}
