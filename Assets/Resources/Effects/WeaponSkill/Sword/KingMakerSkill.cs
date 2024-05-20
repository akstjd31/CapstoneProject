using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KingMakerSkill : MonoBehaviour
{
    float destroyTimer = 1.0f;
    public float duration = 20.0f;
    int _attackSpeed;
    PlayerCtrl playerCtrl;
    Status status;
    [SerializeField] private int viewID = -1;
    // Start is called before the first frame update
    [PunRPC]
    public void InitializeKingMakerSwordSkill(int viewID)
    {
        this.viewID = viewID;
    }
    // Start is called before the first frame update
    void Start()
    {
        _attackSpeed = status.attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        destroyTimer += Time.deltaTime;
        this.transform.position = playerCtrl.transform.position;
        status.attackSpeed = 20;
        if(destroyTimer > duration)
        {
            status.attackSpeed = _attackSpeed;
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
