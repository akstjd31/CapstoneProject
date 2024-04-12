using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public Enemy[] enemyList;

    public Transform[] spawner;
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    PhotonView pv;

    private GameObject canvas;
    //private NetworkManager networkManager;
    bool enemyCreated = false;
    //NetworkManager networkManager;
    //int weaponViewID = 1;

    void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    public void RemoveEnemy(GameObject enemyObj)
    {
        enemies.Remove(enemyObj);
    }

    public void OnClickEnemyInstantiator()
    {
        int randSpawnerIdx = Random.Range(0, 2);
        GameObject enemyObj = PhotonNetwork.Instantiate("Troll", spawner[randSpawnerIdx].transform.position, Quaternion.identity);

        PhotonView enemyPV = enemyObj.GetComponent<PhotonView>();


        this.GetComponent<PhotonView>().RPC("EnemyCreator", RpcTarget.All, enemyPV.ViewID);
    }

    [PunRPC]
    private void EnemyCreator(int ViewID)
    {
        PhotonView target = PhotonView.Find(ViewID);

        EnemyCtrl enemyCtrlScript = target.GetComponent<EnemyCtrl>();
        Enemy enemy = target.GetComponent<Enemy>();

        enemy.InitSetting();

        enemyCtrlScript.SetEnemy(enemy);

        enemies.Add(target.gameObject);
    }

    [PunRPC]
    private void DestroyEnemy()
    {
        foreach (GameObject obj in enemies)
        {
            EnemyCtrl enemyCtrlScript = obj.GetComponent<EnemyCtrl>();
            enemyCtrlScript.DestroyHPBar();
            PhotonNetwork.Destroy(obj);
        }

        enemies.Clear();
    }

    public void OnClickEnemyDestroyAll()
    {
        this.GetComponent<PhotonView>().RPC("DestroyEnemy", RpcTarget.All);
    }
}
