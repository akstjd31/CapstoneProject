using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;


public class DungeonManager : MonoBehaviourPunCallbacks
{
    public float[] mapSize = new float[2] {36.0f, 20.0f};
    public int roomNum = 8;
    public int createdRoomNum = 1;
    public int coroutineNum = 0;
    public static Vector3 playerRoomPos = new Vector3(0.0f, 0.0f ,0.0f);
    //public static GameObject[] endRooms = new GameObject[roomNum];
    public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> endRooms = new List<GameObject>();
    public float mapCreateTimer = 0.0f;
    public float gridMapCreateTimer = 0.0f;
    public bool isMapCreate = false;
    public bool specialRoomSelect = false;
    public bool NavMeshbaked = false;
    public bool reset = false;
    public GameObject bossRoom;
    public GameObject bossRoomMarker;
    float bossRoomDistance = 0;
    public GameObject shopRoom;
    public GameObject shopRoomMarker;
    public GameObject healRoom;
    public GameObject healRoomMarker;
    public GameObject spawnPoint;
    public GameObject playerMarker;

    string mapDir = "Dungeon/";

    public BakeNavMesh bakeNavMesh;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<RoomController>().Invoke("CreateRoom", 0.3f);
    }

    
    public override void OnCreatedRoom()
    {
        spawnPoint = PhotonNetwork.Instantiate(mapDir + "Spawn", new Vector3(500.0f, 500.0f, 0.0f), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(specialRoomSelect)
        {
            playerMarker.transform.position = new Vector2(playerRoomPos.x / mapSize[0] + spawnPoint.transform.position.x, playerRoomPos.y / mapSize[1] + spawnPoint.transform.position.x);
        }
        gridMapCreateTimer += Time.deltaTime;
        if (!isMapCreate)
        {
            mapCreateTimer += Time.deltaTime;
        }
        if (mapCreateTimer > 2.0f && coroutineNum == 0)
        {
            isMapCreate = true;
            mapCreateTimer = 0.0f;
        }
        if (isMapCreate && !specialRoomSelect)
        {
            if(endRooms.Count < 3)
            {
                DungeonReset();
            }
            for(int i = 0; i < endRooms.Count; i++)
            {
                if(endRooms[i] == null)
                {
                    continue;
                }
                float endRoomDistance = Mathf.Sqrt(Mathf.Pow(endRooms[i].transform.position.x,2) + Mathf.Pow(endRooms[i].transform.position.y,2));
                if(bossRoomDistance < endRoomDistance)
                {
                    bossRoomDistance = endRoomDistance;
                    bossRoom = endRooms[i];
                    endRooms.Remove(endRooms[i]);
                }
            }
            while(true)
            {
                int rannum = Random.Range(0, endRooms.Count);
                Debug.Log(endRooms.Count);
                Debug.Log(rannum);
                if(endRooms[rannum] == null)
                {
                    Debug.Log("null");
                    continue;
                }
                else
                {
                    shopRoom = endRooms[rannum];
                    endRooms.Remove(endRooms[rannum]);
                    Debug.Log("Remove");
                    break;
                }
            }
            while(true)
            {
                int rannum = Random.Range(0, endRooms.Count);
                Debug.Log(endRooms.Count);
                Debug.Log(rannum);
                if(endRooms[rannum] == null)
                {
                    Debug.Log("null");
                    continue;
                }
                else
                {
                    healRoom = endRooms[rannum];
                    endRooms.Remove(endRooms[rannum]);
                    Debug.Log("Remove");
                    break;
                }
            }
            playerMarker = PhotonNetwork.Instantiate(mapDir + playerMarker.name, new Vector2(playerRoomPos.x / mapSize[0] + spawnPoint.transform.position.x, playerRoomPos.y / mapSize[1] + spawnPoint.transform.position.x), Quaternion.identity, 0);
            PhotonNetwork.Instantiate(mapDir + healRoomMarker.name, healRoom.transform.position, Quaternion.identity, 0);
            PhotonNetwork.Instantiate(mapDir + shopRoomMarker.name+"_", shopRoom.transform.position, Quaternion.identity, 0);
            PhotonNetwork.Instantiate(mapDir + bossRoomMarker.name, bossRoom.transform.position, Quaternion.identity, 0);
            specialRoomSelect = true;

        }
        if(isMapCreate && specialRoomSelect && !NavMeshbaked)
        {
            for(int i = 0; i < createdRoomNum; i++)
            {
                if(rooms[i].GetComponent<RoomController>().makePlayMap)
                {
                    if(i == createdRoomNum - 1)
                    {
                        bakeNavMesh.BakeNavigation();
                        NavMeshbaked = true;
                        GameObject.FindGameObjectWithTag("Canvas").GetComponent<Inventory>().gameObject.transform.SetAsLastSibling();
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
    void DungeonReset()
    {
        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            rooms[i].GetComponent<RoomController>().Reset();
            rooms.Remove(rooms[i]);
        }
        for (int i = endRooms.Count - 1; i >= 0; i--)
        {
            endRooms.Remove(endRooms[i]);
        }
        roomNum = 8;
        createdRoomNum = 1;
        coroutineNum = 0;
        mapCreateTimer = 0.0f;
        PhotonNetwork.Destroy(spawnPoint);
        spawnPoint = PhotonNetwork.Instantiate(mapDir + "Spawn", new Vector3(500.0f, 500.0f, 0.0f), Quaternion.identity);
        isMapCreate = false;
        bossRoomDistance = 0;
    }
}