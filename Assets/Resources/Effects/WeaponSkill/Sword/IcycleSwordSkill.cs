using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class IcycleSwordSkill : MonoBehaviour
{
    float destroyTimer = 0.0f;
    public float duration = 0.5f;
    [SerializeField] private int viewID = -1;
    // Start is called before the first frame update
    [PunRPC]
    public void InitializeIcycleSwordSkill(int viewID)
    {
        this.viewID = viewID;
    }

    void Update()
    {
        destroyTimer += Time.deltaTime;
        if(destroyTimer > duration)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyCtrl enemyCtrl = other.gameObject.GetComponent<EnemyCtrl>();
            if (enemyCtrl != null)
            {
                PhotonView enemyPv = enemyCtrl.GetComponent<PhotonView>();
                if (enemyPv != null)
                {
                    Debug.Log(viewID);
                    enemyPv.RPC("DamagePlayerOnHitRPC", RpcTarget.All, viewID, 1.5f);
                    enemyCtrl.agent.speed *= 0.6f;
                }
            }
        }
    }
}