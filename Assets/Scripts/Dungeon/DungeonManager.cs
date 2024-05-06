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
    public int roomNum = 5;
    public static Vector3 playerRoomPos = new Vector3(0.0f, 0.0f ,0.0f);
    //public static GameObject[] endRooms = new GameObject[roomNum];
    public List<GameObject> endRooms = new List<GameObject>();
    public int endRoomIndexChecker = 0;
    public float mapCreateTimer = 0.0f;
    public bool isMapCreate = false;
    public bool specialRoomSelect = false;
    public GameObject bossRoom;
    public GameObject bossRoomMarker;
    float bossRoomDistance = 0;
    public GameObject shopRoom;
    public GameObject shopRoomMarker;
    public GameObject healRoom;
    public GameObject healRoomMarker;

    public int farherstX = 0;
    public int farherstY = 0;
    public int farherstMX = 0;
    public int farherstMY = 0;

    string mapDir = "Dungeon/";
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<RoomController>().Invoke("CreateRoom", 0.3f);
    }

    
    public override void OnCreatedRoom()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMapCreate)
        {
            mapCreateTimer += Time.deltaTime;
        }
        if(mapCreateTimer > 1.0f && roomNum < 0)
        {
            isMapCreate = true;
        }
        if(isMapCreate && !specialRoomSelect)
        {
            for(int i = 0; i < endRoomIndexChecker; i++)
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
                    endRooms[i] = null;
                }
            }
            while(true)
            {
                int rannum = Random.Range(0, endRoomIndexChecker);
                if(endRooms[rannum] == null)
                {
                    continue;
                }
                else
                {
                    shopRoom = endRooms[rannum];
                    endRooms[rannum] = null;
                    break;
                }
            }
            while(true)
            {
                int rannum = Random.Range(0, endRoomIndexChecker);
                if(endRooms[rannum] == null)
                {
                    continue;
                }
                else
                {
                    healRoom = endRooms[rannum];
                    endRooms[rannum] = null;
                    break;
                }
            }
            PhotonNetwork.Instantiate(mapDir + healRoomMarker.name, healRoom.transform.position, Quaternion.identity, 0);
            PhotonNetwork.Instantiate(mapDir + shopRoomMarker.name, shopRoom.transform.position, Quaternion.identity, 0);
            PhotonNetwork.Instantiate(mapDir + bossRoomMarker.name, bossRoom.transform.position, Quaternion.identity, 0);
            specialRoomSelect = true;

            Debug.Log(farherstMX +" "+ farherstX +" "+ farherstMY +" "+ farherstY);
        }
    }
}
