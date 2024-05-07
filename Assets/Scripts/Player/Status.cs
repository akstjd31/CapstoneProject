using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Status : MonoBehaviourPunCallbacks
{
    // 텍스트 이름 == 인덱스
    private enum TextList
    {
        NickName = 0, HP, Weapon, RollCoolTime
    }

    public float HP = 30; // 체력
    public float MAXHP = 100; // 최대 체력
    public int attackDamage = 5; // 공격력
    public float attackSpeed = 1.0f; // 공격 속도
    public int agroMeter = 0; // 어그로 미터기

    [SerializeField] private Transform statInfo; // 플레이어의 스탯 정보
    public Text[] stats; // 스탯 정보가 담긴 텍스트

    PlayerCtrl playerCtrl; // 플레이어 스크립트
    PhotonView pv; // 플레이어 pv
    string nickName; // 플레이어 닉네임

    private void Start()
    {
        playerCtrl = this.GetComponent<PlayerCtrl>();
        nickName = playerCtrl.GetComponent<PhotonView>().Owner.NickName;
        pv = this.GetComponent<PhotonView>();

        // 태그로 찾은 후에 텍스트 집어넣기
        //statInfo = GameObject.FindGameObjectWithTag("StatInfo").transform;
        //stats = statInfo.GetChild(0).GetComponentsInChildren<Text>();
    }

    private void Update()
    {
        // 본인한테만 스탯 정보가 보임.
        if (pv.IsMine)
        {
            Vector3 playerPos = Camera.main.WorldToScreenPoint(playerCtrl.transform.position);

            //UpdateText(playerPos);
        }
    }

    [PunRPC]
    public void DamageEnemyOnHitRPC(int damage, Vector3 attackDirection)
    {
        HP -= damage;
        playerCtrl.onHit = true;
        playerCtrl.SetState(PlayerCtrl.State.ATTACKED);
        playerCtrl.enemyAttackDirection = attackDirection;
    }

    // 플레이어 위치에 따른 텍스트 위치 조절
    void UpdateText(Vector3 playerPos)
    {
        stats[(int)TextList.NickName].transform.position = new Vector2(playerPos.x, playerPos.y + 60);
        stats[(int)TextList.HP].transform.position = new Vector2(playerPos.x + 80, playerPos.y + 40);
        stats[(int)TextList.Weapon].transform.position = new Vector2(playerPos.x + 80, playerPos.y + 20);

        stats[(int)TextList.NickName].text = nickName;
        stats[(int)TextList.HP].text = "HP: " + HP;
        stats[(int)TextList.Weapon].text = "Weapon: " + playerCtrl.weaponName;
        //PrintRollCoolTimeText();
    }

    // 구르기는 사용 후 남은 쿨타임 계산때만 보임
    void PrintRollCoolTimeText()
    {
        if (playerCtrl.rollCoolTime <= 0.0f)
        {
            stats[(int)TextList.RollCoolTime].gameObject.SetActive(false);
            stats[(int)TextList.RollCoolTime].text = "RollCoolTime: " + playerCtrl.rollCoolTime;
        }
        else
        {
            stats[(int)TextList.RollCoolTime].gameObject.SetActive(true);
            stats[(int)TextList.RollCoolTime].text = "RollCoolTime: " + playerCtrl.rollCoolTime.ToString("F3");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 던전에 있는 방이 변경될 때마다 어그로미터기 수치를 0으로 만들어줌.
        if (other.CompareTag("TriggerObj"))
        {
            agroMeter = 0;
        }
    }
}
