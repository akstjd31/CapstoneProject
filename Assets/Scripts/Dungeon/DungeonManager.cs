using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static int roomNum = 10;
    //public static GameObject[] endRooms = new GameObject[roomNum];
    public static List<GameObject> endRooms = new List<GameObject>();
    public static int endRoomIndexChecker = 0;
    public static float mapCreateTimer = 0.0f;
    public static bool isMapCreate = false;
    GameObject bossRoom;
    public GameObject bossRoomMarker;
    float bossRoomDistance = 0;
    GameObject shopRoom;
    public GameObject shopRoomMarker;
    GameObject healRoom;
    public GameObject healRoomMarker;
    bool specialRoomSelect = false;

    public static int farherstX = 0;
    public static int farherstY = 0;
    public static int farherstMX = 0;
    public static int farherstMY = 0;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<RoomController>().Invoke("CreateRoom", 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMapCreate)
        {
            mapCreateTimer += Time.deltaTime;
        }
        if(mapCreateTimer > 1.0f)
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
            GameObject healRoomMarker_ = Instantiate(healRoomMarker, this.transform);
            healRoomMarker_.transform.position = healRoom.transform.position;

            GameObject shopRoomMarker_ = Instantiate(shopRoomMarker, this.transform);
            shopRoomMarker_.transform.position = shopRoom.transform.position;
            
            GameObject bossRoomMarker_ = Instantiate(bossRoomMarker, this.transform);
            bossRoomMarker_.transform.position = bossRoom.transform.position;
            specialRoomSelect = true;

            Debug.Log(farherstMX +" "+ farherstX +" "+ farherstMY +" "+ farherstY);
        }
    }
}
