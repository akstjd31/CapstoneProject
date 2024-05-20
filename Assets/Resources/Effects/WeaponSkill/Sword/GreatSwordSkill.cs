using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Mono.Cecil.Cil;

public class GreatSwordSkill : MonoBehaviour
{
    Vector3 dir;
    float destroyTimer = 0.0f;
    public float duration = 0.6f;
    float angle;
    [SerializeField] private int viewID = -1;
    // Start is called before the first frame update
    [PunRPC]
    public void InitializeGreatSwordSkill(int viewID, Vector3 dir)
    {
        this.dir = dir;
        this.viewID = viewID;
    }

    void Start()
    {
        angle = transform.parent.rotation.z;
        var main = this.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        main.startRotation = angle;
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
                    enemyPv.RPC("DamagePlayerOnHitRPC", RpcTarget.All, viewID, 3.0f);
                    enemyPv.RPC("EnemyKnockbackRPC", RpcTarget.All, dir);
                }
            }
        }
    }
}
