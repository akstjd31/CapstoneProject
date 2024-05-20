using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;

public class DarkLongBowSkill : MonoBehaviour
{
    Vector3 skillPos;
    float destroyTimer = 0.0f;
    public float duration = 0.5f;
    [SerializeField] private int viewID = -1;
    // Start is called before the first frame update

    [PunRPC]
    public void InitializeDarkLongBowSkill(int viewID)
    {
        this.viewID = viewID;
    }

    // Start is called before the first frame update
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
                    enemyPv.RPC("DamagePlayerOnHitRPC", RpcTarget.All, viewID, 3.0f);
                }
            }
        }
    }
}
