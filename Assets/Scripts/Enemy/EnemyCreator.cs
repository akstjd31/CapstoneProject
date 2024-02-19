using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyCreator : MonoBehaviour
{
    List<Enemy> enemyList = new List<Enemy>();
    public EnemySO enemySO;

    PhotonView pv;
    //private NetworkManager networkManager;
    bool enemyCreated = false;
    //NetworkManager networkManager;
    //int weaponViewID = 1;

    void Awake()
    {
        for (var i = 0; i < enemySO.enemyList.Length; i++)
            enemyList.Add(enemySO.enemyList[i]);

        
    }
    
    void Update()
    {
        if (!enemyCreated && GameObject.FindGameObjectWithTag("Player"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                EnemyInstantiator();
                enemyCreated = true;
            }
        }
    }

    void EnemyInstantiator()
    {
        for (var i = 0; i < enemyList.Count; i++)
        {
            GameObject newWeapon = PhotonNetwork.Instantiate(enemyList[i].prefab.name, new Vector2(7, 3), Quaternion.identity);
            //pv = newWeapon.GetComponent<PhotonView>();
        }
    }
}
