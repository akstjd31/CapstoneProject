using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KingMakerSkill : MonoBehaviour
{
    float destroyTimer = 1.0f;
    public float duration = 20.0f;
    public int _attackSpeed;
    public bool end = false;
    PlayerCtrl playerCtrl;
    Status status;
    [SerializeField] private int viewID = -1;
    // Start is called before the first frame update
    [PunRPC]
    public void InitializeKingMakerSwordSkill(int viewID)
    {
        this.viewID = viewID;
    }

    public void setPlayerCtrl(PlayerCtrl playerCtrl)
    {
        this.playerCtrl = playerCtrl;
    }

    public void setStatus(Status status)
    {
        this.status = status;
    }
    // Start is called before the first frame update
    void Start()
    {
        _attackSpeed = status.attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(status != null && playerCtrl != null)
        destroyTimer += Time.deltaTime;
        this.transform.position = new Vector2(playerCtrl.transform.position.x, playerCtrl.transform.position.y + 1.0f);
        status.attackSpeed = 20;
        if(destroyTimer > duration)
        {
            end = true; 
            status.attackSpeed = _attackSpeed;
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
