using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static int roomNum = 10;
    //public static GameObject[] endRooms = new GameObject[roomNum];
    public static List<GameObject> endRooms = new List<GameObject>();
    public static int endRoomIndexChecker = 0;
    public static float mapCreateTimer = 0.0f;
    public bool isMapCreate = false;
    GameObject farthestRoom;
    public GameObject farthestRoomMarker;
    float farthestRoomDistance = 0;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<RoomController>().Invoke("CreateRoom", 0.3f);
        farthestRoomMarker = Instantiate(farthestRoomMarker, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(roomNum);
        Debug.Log(mapCreateTimer);
        Debug.Log(isMapCreate);
        if(!isMapCreate)
        {
            mapCreateTimer += Time.deltaTime;
        }
        if(mapCreateTimer > 1.0f)
        {
            isMapCreate = true;
        }
        if(isMapCreate)
        {
            for(int i = 0; i < endRoomIndexChecker; i++)
            {
                float endRoomDistance = Mathf.Sqrt(Mathf.Pow(endRooms[i].transform.position.x,2) + Mathf.Pow(endRooms[i].transform.position.y,2));
                if(farthestRoomDistance < endRoomDistance)
                {
                    farthestRoomDistance = endRoomDistance;
                    farthestRoom = endRooms[i];
                }
            }
            farthestRoomMarker.transform.position = farthestRoom.transform.position;
        }
    }
}
