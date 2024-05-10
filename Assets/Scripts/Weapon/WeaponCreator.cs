using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class WeaponCreator : MonoBehaviour
{
    List<Weapon> weaponList = new List<Weapon>(); // 무기 종류가 담길 리스트
    public WeaponSO weaponSO; // 무기의 종류 ScriptableObject

    PhotonView pv;
    //private NetworkManager networkManager;
    bool weaponCreated = false; // 맵에서 무기의 생성 여부
    //NetworkManager networkManager;
    //int weaponViewID = 1;

    void Awake()
    {
        for (var i = 0; i < weaponSO.weaponList.Length; i++)
            weaponList.Add(weaponSO.weaponList[i]);

        
        //networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }
    
    void Update()
    {
        if (!weaponCreated && GameObject.FindGameObjectWithTag("Player"))
        {
            // 첫 플레이어가 접속하면 생성
            if (PhotonNetwork.IsMasterClient)
            {
                WeaponInstantiator();
                weaponCreated = true;
            }
        }
    }

    void WeaponInstantiator()
    {
        for (var i = 0; i < weaponList.Count; i++)
        {
            GameObject newWeapon = PhotonNetwork.Instantiate(weaponList[i].Prefab.name, new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f)), Quaternion.identity);
            pv = newWeapon.GetComponent<PhotonView>();

            //pv.ViewID = weaponViewID;
        }
    }
}
