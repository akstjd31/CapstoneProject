using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;


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
        NORMAL, MOVE, ATTACK, ATTACKED, DIE
    }

    public float attackDistanceSpeed; // 공격 시 이동하는 속도

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
    public bool isInDungeon = false;

    public Party party;

    private bool isLobbyScene = false;  //DungeonEntrance와 플레이어의 충돌을 감지하기 위해 현재 씬이 LobbyScene인지 확인하는 변수
    GameObject DungeonCanvas;
    RawImage DungeonImage;
    Button destroyButton;

    Inventory inventory;

    Vector3 mouseWorldPosition;

    //Recorder recorder;

    private bool isDeactiveUI;

    //스킬
    //public PassiveSkill passiveSkill;

    // 공격 포인트와 범위
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    Collider2D hitEnemies;  // 공격 대상

    public bool onHit = false;
    public Vector3 enemyAttackDirection;
    public float knockBackDistanceSpeed;
    public float originalKnockBackDistanceSpeed;
    private float attackAnimSpeed;

    private bool isDeath = false;
    private float deathTime = 1.0f;

    private ShowOnSaleItem showOnSaleItem;  //상점
    private GameObject[] npcList;           //npc
    private bool isActiveSale = false;
    public bool isMoveRoom = false;
    private float interactionDist = 2f;     //상호작용 거리
    private GameObject npcParent;

    private Items items; // 바닥에 놓여있는 아이템

    // 바닥에 떨어진 아이템 체크포인트
    public Transform itemCheckPoint;
    public float itemCheckRange;
    public LayerMask itemLayers;

    // 원거리 발사 위치와 발사체
    public Transform firePoint;
    public Transform projectile;
    [SerializeField] private Vector3 arrowTargetPos;

    private ItemManager itemManager;
    private SPUM_SpriteList spum_SpriteList;

    [SerializeField] private int randIdx = -1;
    [SerializeField] private Item equipItem;

    private Transform jewel;
    
    private GameObject openPartyButton;
    private GameObject createPartyButton;
    private GameObject InputMessage;
    private GameObject Send;

    private GameObject skill_explane;
    private bool isSkillUI = false;

    //public float animSpeed;   // 애니메이션 속도 테스트

    public void ChangeState(State state)
    {
        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)state);
    }

    [PunRPC]
    private void ChangeStateRPC(int state)
    {
        this.state = (State)state;
    }

    void Start()
    {
        // 변수 초기화
        rigid = gameObject.GetComponent<Rigidbody2D>();
        pv = this.GetComponent<PhotonView>();

        // 공용
        //playerStat = GameObject.FindGameObjectWithTag("PlayerStat");
        anim = this.GetComponent<Animator>();
        status = this.GetComponent<Status>();
        //passiveSkill = this.GetComponent<PassiveSkill>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        inventory = canvas.transform.Find("Inventory").GetComponent<Inventory>();
        inventory.SetStatus(status);
        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        spum_SpriteList = this.transform.Find("Root").GetComponent<SPUM_SpriteList>();

        //recorder = GameObject.Find("VoiceManager").GetComponent<Recorder>();
        showOnSaleItem = FindObjectOfType<ShowOnSaleItem>();    //상점
        npcParent = GameObject.Find("npc"); // find npc
        npcList = null;

        ChangeState(State.NORMAL);
        items = null;

        // 로비 씬
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            //Skill UI
            skill_explane = GameObject.Find("Skill_Explane");
            Explane_Pos.skill_explane = skill_explane;
            skill_explane.SetActive(false);

            if (pv.IsMine)
            {
                GameObject.FindGameObjectWithTag("PhotonManager").GetComponent<PhotonManager>().playerWeaponIdx = randIdx;
            }

            //itemManager.GetComponent<PhotonView>().RPC("RandomCommonItemIndex", RpcTarget.AllBuffered, pv.ViewID);
            //CommonWeaponEquipRPC(randIdx);

            chatScript = canvas.GetComponent<Chat>();
            partySystemScript = canvas.GetComponent<PartySystem>();

            //던전 선택 canvas 생성
            MakeDungeonMap();

            //상점 테스트
            showOnSaleItem = FindObjectOfType<ShowOnSaleItem>();
        }

        // 던전 씬
        else if (SceneManager.GetActiveScene().name == "DungeonScene")
        {
            randIdx = GameObject.FindGameObjectWithTag("PhotonManager").GetComponent<PhotonManager>().playerWeaponIdx;
        }

        pv.RPC("CommonWeaponEquipRPC", RpcTarget.AllBuffered, randIdx, status.charType);

        anim.speed = GetAnimSpeed(status.attackSpeed);

        if (pv.IsMine)
        {
            inventory.equippedItem = equipItem;
            inventory.FreshSlot();
        }

        TotalStatus(equipItem);


        if (npcParent != null)
        {
            // npcParent에서 자식 GameObject들을 모두 가져와서 배열에 저장
            npcList = new GameObject[npcParent.transform.childCount];
            for (int i = 0; i < npcParent.transform.childCount; i++)
            {
                npcList[i] = npcParent.transform.GetChild(i).gameObject;
            }
        }
    }

    //Graphic & Input Updates	
    void Update()
    {
        if (pv.IsMine)
        {
            //anim.speed = animSpeed;
            if (!isDeath)
            {
                if (status.HP <= 0)
                {
                    status.HP = 0;
                    anim.SetTrigger("Death");
                    rigid.velocity = Vector2.zero;
                    isDeath = true;
                }

                if (SceneManager.GetActiveScene().name == "LobbyScene")
                {
                    isDeactiveUI = chatScript != null && partySystemScript != null &&
                    !chatScript.chatView.activeSelf && !partySystemScript.partyCreator.activeSelf && !partySystemScript.partyView.activeSelf;
                }
                else
                {
                    isDeactiveUI = true;
                }

                moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                if (state != State.ATTACK && state != State.ATTACKED && !onHit && isDeactiveUI)
                {
                    if (moveDir.x != 0 || moveDir.y != 0)
                    {
                        ChangeState(State.MOVE);
                    }
                    else
                    {
                        rigid.velocity = Vector2.zero;
                        ChangeState(State.NORMAL);
                    }
                }

                // 공격 & 공격 쿨타임 끝나면
                if (Input.GetMouseButtonDown(0) && isAttackCooldownOver &&
                    !EventSystem.current.currentSelectedGameObject && isDeactiveUI && !onHit && !inventory.gameObject.activeSelf)
                {
                    ChangeState(State.ATTACK);

                    Vector3 mouseScreenPosition = Input.mousePosition;

                    // 마우스의 스크린 좌표를 월드 좌표로 변환합니다.
                    mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
                    // 플레이어가 보고 있는 방향에 따른 공격방향
                    //SetDirection();

                    if (jewel != null)
                    {
                        jewel.GetComponent<PhotonView>().RPC("ChangeJewelColor", RpcTarget.All);
                        //InteractJewel(mouseWorldPosition);
                    }

                    if (status.charType.Equals("Warrior"))
                    {
                        attackDistanceSpeed = 14f;
                        isAttackCooldownOver = false;

                        // 적을 공격한 상태.
                        if (isPlayerInRangeOfEnemy)
                        {
                            enemyCtrl.GetComponent<PhotonView>().RPC("EnemyAttackedPlayer", RpcTarget.All, status.attackDamage);
                        }
                    }
                    else
                    {
                        rigid.velocity = Vector2.zero;
                        // 플레이어 좌, 우 스케일 값 변경 (뒤집기)
                        if (mouseWorldPosition.x - this.transform.position.x > 0)
                        {
                            this.transform.localScale = new Vector3(-1, 1, 1);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(1, 1, 1);
                        }
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
                if (Input.GetKeyDown(KeyCode.Return) && chatScript != null &&
                    !partySystemScript.partyCreator.activeSelf && !partySystemScript.partyView.activeSelf)
                {
                    if (!chatScript.inputField.isFocused)
                    {
                        chatScript.inputField.interactable = true;
                        chatScript.inputField.ActivateInputField();
                    }
                }

                // 채팅 끄기
                if (Input.GetKeyDown(KeyCode.Escape) && chatScript != null &&
                    !partySystemScript.partyCreator.activeSelf && !partySystemScript.partyView.activeSelf)
                {
                    chatScript.inputField.text = "";
                    chatScript.inputField.DeactivateInputField();

                    chatScript.CloseChatWindowOnButtonClick();
                }

                // 인벤토리 열기
                if (Input.GetKeyDown(KeyCode.I) && isDeactiveUI)
                {
                    inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);

                    //UI에 보유 금액 표기
                    if(inventory.gameObject.activeSelf)
                    {
                        inventory.FreshSlot();  // 아이템 리스트를 인벤토리에 추가한다. 
                        GameObject.Find("DoubleCurrencyBox").transform.Find("Text").GetComponent<Text>().text = UserInfoManager.GetNowMoney().ToString();
                    }
                }

                // 보이스 참가하기
                //if (SceneManager.GetActiveScene().name == "DungeonScene")
                //{
                    //if (Input.GetKey(KeyCode.T))
                    //{
                    //    recorder.TransmitEnabled = true;
                    //}
                    //else
                    //{
                    //    recorder.TransmitEnabled = false;
                    //}
                //}

                if (partySystemScript != null)
                {
                    PartyHUDActive();
                }

                // 아이템 픽업
                if (Input.GetKeyDown(KeyCode.Z) && items != null)
                {
                    if (inventory.items.Count < inventory.GetInventorySlotLength())
                    {
                        inventory.GetComponent<Inventory>().AddItem(items.item);
                        Destroy(items.gameObject);
                    }
                    else
                    {
                        Debug.Log("인벤토리에 공간이 없습니다!");
                    }
                }

                //store off
                //거리가 멀어지면 자동 종료
                if (isActiveSale)
                {
                    bool inRange = false;

                    foreach (var npc in npcList)
                    {
                        float distance = Vector2.Distance(npc.transform.position, transform.position);

                        if (distance <= interactionDist && showOnSaleItem != null)
                        {
                            //store
                            if(npc.name.StartsWith("npc"))
                            {
                                inRange = true;
                                break;
                            }
                        }

                        //Debug.Log($"dist : {npc.name} => {distance} || ui close condition : {inRange}");
                    }

                    if (!inRange)
                    {
                        showOnSaleItem.CloseShopUI();
                        inventory.gameObject.SetActive(false);
                        isActiveSale = false;
                    }
                }

                //store on/off (close to npc)
                if (Input.GetKeyDown(KeyCode.O))
                {
                    showOnSaleItem = FindObjectOfType<ShowOnSaleItem>();

                    //close UI
                    if (isActiveSale)
                    {
                        showOnSaleItem.CloseShopUI();
                        inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
                        isActiveSale = false;
                    }
                    //open UI
                    else if(npcList != null)
                    {
                        foreach (var npc in npcList)
                        {
                            float distance = Vector3.Distance(npc.transform.position, transform.position);
                            //Debug.Log($"dist : {npc.name} => {distance}");

                            //store
                            if(distance <= interactionDist && npc.name.StartsWith("npc") && showOnSaleItem != null)
                            {
                                showOnSaleItem.ShowShopUI();
                                inventory.transform.SetAsLastSibling();
                                inventory.gameObject.SetActive(true);
                                isActiveSale = true;
                                return;
                            }
                            //skill UI
                            else if(distance <= interactionDist && npc.name.StartsWith("skill") && showOnSaleItem != null)
                            {
                                isSkillUI = true;
                                //GameObject.Find("Main Camera").GetComponent<Camera>().enabled = false;
                                SceneManager.LoadScene("Skill_UI", LoadSceneMode.Additive);
                            }
                            
                        }
                    }
                }
            }
            else
            {
                rigid.velocity = Vector2.zero;
            }
        }

        if(isSkillUI)
        {
            Explane_Pos.SetMousePos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            int layerMask = LayerMask.GetMask("Skill_UI");
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null)
            {
                //Debug.Log($"hit in PlayerCtrl : {hit.collider.name}");
                //CharSkill.SetHitName(hit.collider.name);
            }
            else
            {
                //CharSkill.SetHitName("no hit");
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
                    AttackDirection();
                    Attack();
                    break;
                case State.ATTACKED:
                    KnockBack();
                    break;
                case State.DIE:
                    Death();
                    break;
            }
        }
    }

    public PlayerCtrl GetPartyMember(PlayerCtrl playerCtrl) //파티원 정보 게터
    {
        if(party.GetPartyHeadCount() == 1) //혼자라면 나를 리턴
        {
            return playerCtrl;
        }
        else
        {
            if(party.GetPartyLeaderID() == pv.ViewID) //파티장이 나라면 파티원 리턴
            {
                return PhotonView.Find(party.GetPartyMemberID()).GetComponent<PlayerCtrl>();
            }
            else                                      //내가 파티원이라면 파티장 playerCtrl리턴
            {
                return PhotonView.Find(party.GetPartyLeaderID()).GetComponent<PlayerCtrl>();
            }
        }
    }

    public Item GetEquipItem()
    {
        return this.equipItem;
    }
    public void SetEquipItem(Item item)
    {
        this.equipItem = item;
    }

    public void TotalStatus(Item equippedItem)
    {
        status.attackDamage = status.GetDefaultAttackDamage() + equippedItem.attackDamage;
        status.attackSpeed = equippedItem.attackSpeed;

        SetAnimSpeed(GetAnimSpeed(status.attackSpeed));

        if (equippedItem.bonusStat != BonusStat.NONE)
        {
            switch (equippedItem.bonusStat)
            {
                case BonusStat.HP:
                    status.MAXHP = status.GetDefaultHP() + equippedItem.addValue;
                    break;
                case BonusStat.MOVESPEED:
                    status.moveSpeed = status.GetDefaultMoveSpeed() + (int)equippedItem.addValue;
                    break;
                case BonusStat.EVASIONRATE:
                    status.evasionRate = status.GetDefaultEvasionRate() + equippedItem.addValue;
                    break;
            }
        }
    }

    public void SetAnimSpeed(float animSpeed)
    {
        anim.speed = animSpeed;
    }

    public  float GetAnimSpeed(int speed)
    {
        float animSpeed = 1.0f;
        switch (speed)
        {
            case 1:
                animSpeed = 0.6f;
                break;
            case 2:
                animSpeed = 0.7f;
                break;
            case 3:
                animSpeed = 0.8f;
                break;
            case 4:
                animSpeed = 0.9f;
                break;
            case 5:
                animSpeed = 1.0f;
                break;
            case 6:
                animSpeed = 1.1f;
                break;
            case 7:
                animSpeed = 1.2f;
                break;
            case 8:
                animSpeed = 1.3f;
                break;
            case 9:
                animSpeed = 1.4f;
                break;
            case 10:
                animSpeed = 1.5f;
                break;
        }

        return animSpeed;
    }

    [PunRPC]
    public void SetRandIndex(int rand)
    {
        this.randIdx = rand;
    }

    // 처음 시작할 뽑기로 커먼 아이템 자동선택
    [PunRPC]
    private void CommonWeaponEquipRPC(int rand, string charType)
    {
        if (inventory != null && itemManager != null)
        {
            Item item = null;
            if (charType.Equals("Warrior"))
            {
                item = itemManager.warriorCommonList[rand];
                spum_SpriteList._weaponList[2].sprite = item.itemImage;  // L_Weapon
            }

            else
            {
                item = itemManager.archerCommonList[rand];
                spum_SpriteList._weaponList[0].sprite = item.itemImage;   // R_Weapon
            }
            equipItem = item;
        }
    }

    // 겹쳐져있는 아이템의 우선순위를 확인하여 먹는다.
    private Items CheckItemPriority()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(itemCheckPoint.position, itemCheckRange, itemLayers);

        int maxIdx = 0;
        if (colliders.Length > 2)
        {
            for (var i = 1; i < colliders.Length; i++)
            {
                if (colliders[maxIdx].GetComponent<SpriteRenderer>().sortingOrder < colliders[i].GetComponent<SpriteRenderer>().sortingOrder)
                {
                    maxIdx = i;
                }
            }
        }

        return colliders[maxIdx].GetComponent<Items>();
    }

    public void DeathAnimEvent()
    {
        ChangeState(State.DIE);
        anim.speed = 0f;
    }

    private void Death()
    {
        if (deathTime >= 0.0f)
        {
            deathTime -= Time.deltaTime;
        }
        else
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    // 넉백

    private void KnockBack()
    {
        if (onHit && status.HP > 0)
        {
            if (enemyAttackDirection.x < 0f)
            {
                this.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                this.transform.localScale = new Vector3(1, 1, 1);
            }

            anim.Play("Idle", -1, 0f);
            anim.speed = 0;
            rigid.velocity = Vector2.zero;

            rigid.velocity = enemyAttackDirection.normalized * knockBackDistanceSpeed;

            float attackedSpeedDropMultiplier = 7f;
            knockBackDistanceSpeed -= knockBackDistanceSpeed * attackedSpeedDropMultiplier * Time.deltaTime;

            if (knockBackDistanceSpeed < 0.1f)
            {
                rigid.velocity = Vector2.zero;
                onHit = false;
                knockBackDistanceSpeed = originalKnockBackDistanceSpeed;
                anim.speed = 1f;
                ChangeState(State.NORMAL);
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
        // 플레이어 좌, 우 스케일 값 변경 (뒤집기)
        if (moveDir.x > 0.0f)
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveDir.x < 0.0f)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }

        rigid.velocity = moveDir * status.moveSpeed;
    }

    void Attack()
    {
        if (status.charType.Equals("Warrior"))
        {
            // 공격 범위에 포함된 적
            hitEnemies = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayers);

            if (hitEnemies != null)
            {
                Enemy enemy = hitEnemies.GetComponent<Enemy>();

                if (enemy != null)
                {
                  if(enemy.CompareTag("Chest"))
                  {
                    ChestController chestController = enemy.GetComponent<ChestController>();

                    chestController.ChestBreak();
                  }
                    if (enemy.enemyData.enemyType == EnemyType.BOSS)
                    {
                        float rand = Random.Range(0, 100f);
                        BossCtrl bossCtrl = hitEnemies.GetComponent<BossCtrl>();

                        if (bossCtrl != null && !bossCtrl.onHit)
                        {
                            if (rand > enemy.enemyData.evasionRate)
                            {
                                bossCtrl.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, pv.ViewID);
                                bossCtrl.GetComponent<PhotonView>().RPC("BossKnockbackRPC", RpcTarget.All, mouseWorldPosition - this.transform.position);
                            }
                            else
                            {
                                // 무적으로 인한 회피 or 확률로 나온 회피
                                Debug.Log("회피!");
                            }
                        }
                    }
                    else
                    {
                        EnemyCtrl enemyCtrl = hitEnemies.GetComponent<EnemyCtrl>();

                        if (enemyCtrl != null && !enemyCtrl.onHit)
                        {
                            //enemyCtrl.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, pv.ViewID, passiveSkill.PrideAttack(enemyCtrl, status.attackDamage));
                            enemyCtrl.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, pv.ViewID);
                            enemyCtrl.GetComponent<PhotonView>().RPC("EnemyKnockbackRPC", RpcTarget.All, mouseWorldPosition - this.transform.position);
                        }
                    }
                }
            }
        }
    }
        

    void AttackDirection()
    {
        if (status.charType.Equals("Warrior"))
        {
            // 방향벡터 x좌표의 값에 따른 캐릭터 반전
            if (mouseWorldPosition.x - this.transform.position.x > 0)
            {
                this.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                this.transform.localScale = new Vector3(1, 1, 1);

            }

            // 문제점 : 현재 마우스 위치가 가까우면 플레이어가 조금만 이동함.
            rigid.velocity = (mouseWorldPosition - this.transform.position).normalized * attackDistanceSpeed;

            float attackSpeedDropMultiplier = 5f; // 감소되는 정도
            attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime; // 실제 나아가는 거리 계산

            if (attackDistanceSpeed < 0.5f)
            {
                rigid.velocity = Vector2.zero;
                ChangeState(State.NORMAL);
            }
        }
    }

    // 애니메이션 이벤트 호출 함수
    public void Fire()
    {
        if (pv.IsMine)
        {
            // 총알 프리팹을 firePoint 위치에 인스턴스화
            GameObject arrowPrefab = PhotonNetwork.Instantiate(projectile.name, firePoint.position, firePoint.rotation);

            // PhotonView와 Arrow 컴포넌트를 가져옴
            PhotonView arrowPV = arrowPrefab.GetComponent<PhotonView>();
            if (arrowPV == null)
            {
                Debug.LogError("PhotonView component is missing on the instantiated arrow prefab.");
                return;
            }

            Arrow arrow = arrowPV.GetComponent<Arrow>();
            if (arrow == null)
            {
                Debug.LogError("Arrow component is missing on the instantiated arrow prefab.");
                return;
            }

            // RPC를 사용하여 다른 클라이언트에 총알의 초기 설정 전송
            arrowPV.RPC("InitializeArrow", RpcTarget.AllBuffered, mouseWorldPosition, 3.5f, status.attackDamage, this.tag, pv.ViewID);

            // 상태 변경
            ChangeState(State.NORMAL);
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
    public void PartyHUDActive()
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

            partySystemScript.leavingParty.gameObject.SetActive(true);
        }
        else
        {
            if (partySystemScript.partyMemberHUD[0].activeSelf || partySystemScript.partyMemberHUD[1].activeSelf)
            {
                partySystemScript.partyMemberHUD[0].transform.GetChild(0).GetComponentInChildren<Text>().text = "";
                partySystemScript.partyMemberHUD[1].transform.GetChild(0).GetComponentInChildren<Text>().text = "";

                partySystemScript.partyMemberHUD[0].transform.Find("Ready").gameObject.SetActive(false);
                partySystemScript.partyMemberHUD[1].transform.Find("Ready").gameObject.SetActive(false);

                partySystemScript.partyMemberHUD[0].SetActive(false);
                partySystemScript.partyMemberHUD[1].SetActive(false);
            }

            if (partySystemScript.leavingParty.gameObject.activeSelf)
            {
                partySystemScript.leavingParty.gameObject.SetActive(false);
            }
        }
    }

    private void InteractJewel(Vector2 mousePosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        Debug.Log(hit.collider.name);
        if (hit.collider != null && hit.collider.CompareTag("Jewel"))
        {
            hit.collider.GetComponent<BejeweledPillar>().ChangeJewelColor();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Jewel"))
        {
            jewel = col.collider.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Jewel"))
        {
            jewel = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isLobbyScene && other.name == "DungeonEntrance") //&& DungeonEnterCondition()
        {
            //던전 선택창 출력
            DungeonCanvas.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Item") && items == null)
        {
            items = CheckItemPriority();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            items = null;
        }
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        if (itemCheckPoint == null)
            return;

        Gizmos.DrawWireSphere(itemCheckPoint.position, itemCheckRange);

    }

    public bool IsEnableStore()
    {
        return isActiveSale;
    }

    public bool IsEnableInventory()
    {
        return inventory.enabled;
    }

    public void EnableLobbyUI()
    {
        if (openPartyButton == null || createPartyButton == null || InputMessage == null || Send == null)
        {
            openPartyButton = GameObject.Find("OpenPartyButton");
            createPartyButton = GameObject.Find("CreatePartyButton");
            InputMessage = GameObject.Find("InputMessage");
            Send = GameObject.Find("Send");
        }

        openPartyButton?.SetActive(true);
        createPartyButton.SetActive(true);
        InputMessage.SetActive(true);
        Send.SetActive(true);
    }
    public void DisableLobbyUI()
    {
        if (openPartyButton == null && createPartyButton == null || InputMessage == null || Send == null)
        {
            openPartyButton = GameObject.Find("OpenPartyButton");
            createPartyButton = GameObject.Find("CreatePartyButton");
            InputMessage = GameObject.Find("InputMessage");
            Send = GameObject.Find("Send");
        }

        openPartyButton?.SetActive(false);
        createPartyButton.SetActive(false);
        InputMessage.SetActive(false);
        Send.SetActive(false);
    }
    public GameObject GetSkillExplane()
    {
        return skill_explane;
    }

    public void SetSkillExplanePos(Vector3 pos)
    {
        skill_explane.transform.position = pos;
    }
    public void SetIsSkillUI(bool b)
    {
        isSkillUI = b;
    }
}
