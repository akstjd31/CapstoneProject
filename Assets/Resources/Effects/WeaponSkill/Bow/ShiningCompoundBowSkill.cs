using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ShiningCompoundBowSkill : MonoBehaviour
{
    Vector3 dir;
    float destroyTimer = 0.0f;
    public float duration = 1.0f;
    [SerializeField] private int viewID = -1;
    
    // Start is called before the first frame update
    [PunRPC]
    public void InitializeShiningCompoundBowSkill(int viewID, Vector3 dir)
    {
        this.viewID = viewID;
        this.dir = dir;
    }

    void Start()
    {

    }

    // Update is called once per frame
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
                    enemyPv.RPC("EnemyKnockbackRPC", RpcTarget.All, dir);
                }
            }
        }
    }
}
