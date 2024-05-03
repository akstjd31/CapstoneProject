using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomController : MonoBehaviourPunCallbacks
{
    GameObject startPoint;
	public GameObject[] upRooms;
	public GameObject[] rightRooms;
	public GameObject[] downRooms;
	public GameObject[] leftRooms;
    public Transform[] mapSpawnPoints;
    public GameObject[] mapArray = new GameObject[3];
    PhotonView roomView;
    float[] mapSize = new float[2];
    string mapDir = "Dungeon/";

    public bool[] doorList = new bool[4];

    bool makeDoor = false;
    bool doorCheck = false;
    bool makePlayMap = false;
    bool isNameChanged = false;

    // Start is called before the first frame update
    void Start()
    {
        roomView = GetComponent<PhotonView>();
        mapSize[0] = 36.0f;
        mapSize[1] = 20.0f;
        startPoint = GameObject.Find("Spawn");
        //Invoke("CreateRoom", 0.3f);
        StartCoroutine(CreateRoom());
        //CreateRoom();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(DungeonManager.isMapCreate && !makeDoor)
        {
            RaycastHit2D hit;
            if(!isNameChanged)
            {
                this.name = this.name + Time.deltaTime + Random.Range(0.0f, 10.0f);
                for(int i = 1; i < this.GetComponentsInChildren<Transform>().Length; i++)
                {
                    this.GetComponentsInChildren<Transform>()[i].name = this.GetComponentsInChildren<Transform>()[i].name + Time.deltaTime + Random.Range(0.0f, 10.0f);
                }
                for(int i = 2; i < mapSpawnPoints.Length; i++)
                {
                    if(mapSpawnPoints[i].childCount > 0)
                    {
                        mapSpawnPoints[i].GetChild(0).gameObject.name = mapSpawnPoints[i].GetChild(0).gameObject.name + Time.deltaTime + Random.Range(0.0f, 10.0f);
                    }
                }
                isNameChanged = true;
            }

            for(int i = 2; i < mapSpawnPoints.Length; i++)
            {
                hit = Physics2D.Raycast(new Vector2(this.transform.position.x + (mapSpawnPoints[i].position.x - this.transform.position.x) * 0.2f, this.transform.position.y + (mapSpawnPoints[i].position.y - this.transform.position.y) * 0.2f), 
                new Vector2(mapSpawnPoints[i].position.x - this.transform.position.x , mapSpawnPoints[i].position.y - this.transform.position.y), 0.5f);
                Debug.DrawRay(new Vector2(this.transform.position.x + (mapSpawnPoints[i].position.x - this.transform.position.x) * 0.2f, this.transform.position.y + (mapSpawnPoints[i].position.y - this.transform.position.y) * 0.2f), 
                new Vector2(mapSpawnPoints[i].position.x - this.transform.position.x , mapSpawnPoints[i].position.y - this.transform.position.y) * 0.5f, Color.blue);
                if(hit.collider == null)
                {
                    continue;
                }
                else if(hit.transform.CompareTag("Door"))
                {
                    int ranBin = Random.Range(0, 5);
                    //Debug.Log(ranBin);
                    if(ranBin == 0)
                    {
                        //Debug.Log("breakDoor");
                        //hit.transform.localScale = new Vector3(1, 1, 1);
                        //hit.transform.gameObject.SetActive(false);
                        roomView.RPC("SetActiveRPC",RpcTarget.AllBuffered, hit.transform.gameObject.name, false);
                        Debug.Log(hit.transform.gameObject.name);
                    }
                    else
                    {
                        //Debug.Log("makeDoor");
                        mapSpawnPoints[i].GetChild(0).gameObject.SetActive(true);
                        string wallName = mapSpawnPoints[i].GetChild(0).gameObject.name;
                        roomView.RPC("SetActiveRPC",RpcTarget.AllBuffered, wallName, true);
                        Debug.Log(mapSpawnPoints[i].GetChild(0).gameObject.name);
                    }
                }
            }
            makeDoor = true;
        }
        if(DungeonManager.isMapCreate && makeDoor && !doorCheck)
        {
            for(int i = 0; i < 4; i ++)
            {
                RaycastHit2D hit;
                if (i == 0)
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 1 * 0.2f),
                    Vector2.up, 0.5f);
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 1 * 0.2f),
                    Vector2.up * 0.5f, Color.blue);
                    if(hit.collider == null)
                    {
                        doorList[i] = true;
                    }
                    else
                    {
                        doorList[i] = false;
                    }
                }
                else if (i == 1)
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x + 1 * 0.2f, this.transform.position.y),
                    Vector2.right, 0.5f);
                    Debug.DrawRay(new Vector2(this.transform.position.x + 1 * 0.2f, this.transform.position.y),
                    Vector2.right * 0.5f, Color.blue);
                    if(hit.collider == null)
                    {
                        doorList[i] = true;
                    }
                    else
                    {
                        doorList[i] = false;
                    }
                }
                else if (i == 2)
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y - 1 * 0.2f),
                    Vector2.down, 0.5f);
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y - 1 * 0.2f),
                    Vector2.down * 0.5f, Color.blue);
                    if(hit.collider == null)
                    {
                        doorList[i] = true;
                    }
                    else
                    {
                        doorList[i] = false;
                    }
                }
                else if (i == 3)
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x - 1 * 0.2f, this.transform.position.y),
                    Vector2.left, 0.5f);
                    Debug.DrawRay(new Vector2(this.transform.position.x - 1 * 0.2f, this.transform.position.y),
                    Vector2.left * 0.5f, Color.blue);
                    if(hit.collider == null)
                    {
                        doorList[i] = true;
                    }
                    else
                    {
                        doorList[i] = false;
                    }
                }
            }
            doorCheck = true;
            //Debug.Log(this.transform.position);
            // for(int i = 0; i < 4; i++)
            // {
            //     Debug.Log(doorList[i]);
            // } 
        }
        if(DungeonManager.isMapCreate && makeDoor && doorCheck && !makePlayMap)
        {
            Debug.Log(mapDir + mapArray[Random.Range(0, mapArray.Length)].name);
            Vector3 gridMapPosition = new Vector2((this.transform.position.x - startPoint.transform.position.x) * mapSize[0], (this.transform.position.y - startPoint.transform.position.y) * mapSize[1]);
            PhotonNetwork.Instantiate(mapDir + mapArray[Random.Range(0, mapArray.Length)].name, gridMapPosition, Quaternion.identity, 0);
            //gridMap.transform.position = new Vector2(this.transform.position.x * 25.0f, this.transform.position.y * 9.0f);
            makePlayMap = true;

            // for(int i = 0; i < gameObject.transform.childCount; i++)
            // {
            //     gameObject.transform.GetChild(i).gameObject.SetActive(false);
            // }
        }
    }
    [PunRPC]
    void SetActiveRPC(string wallName, bool active)
    {
        GameObject.Find(wallName).SetActive(active);
    }
    private IEnumerator CreateRoom()
    {
        if(this.transform.position.x > DungeonManager.farherstX)
        {
            DungeonManager.farherstX = (int)this.transform.position.x;
        }
        else if(this.transform.position.y > DungeonManager.farherstY)
        {
            DungeonManager.farherstY = (int)this.transform.position.y;
        }
        else if(this.transform.position.x < DungeonManager.farherstMX)
        {
            DungeonManager.farherstMX = (int)this.transform.position.x;
        }
        else if(this.transform.position.y < DungeonManager.farherstMY)
        {
            DungeonManager.farherstMY = (int)this.transform.position.y;
        }

        int roomNumTemp = DungeonManager.roomNum;
        RaycastHit2D hit;
        mapSpawnPoints = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
        if(DungeonManager.roomNum > (int)roomNumTemp / 3)
        {
            for(int i = 2; i < mapSpawnPoints.Length; i++)
            {
                yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
                if(mapSpawnPoints[i].name.Contains("Up"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        Vector3 roomtempPosition = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        //roomtemp = 
                        int rannum = Random.Range(1, upRooms.Length-1);
                        if(upRooms[rannum].name.Contains("(Clone)"))
                        {
                            PhotonNetwork.Instantiate(mapDir + upRooms[rannum].name.Substring(0, upRooms[rannum].name.Length - 7), roomtempPosition, Quaternion.identity, 0);
                        }
                        else
                        {
                            PhotonNetwork.Instantiate(mapDir + upRooms[rannum].name, roomtempPosition, Quaternion.identity, 0);
                        }
                        //roomtemp.transform.SetParent(this.transform.parent);
                        DungeonManager.roomNum--;
                        DungeonManager.mapCreateTimer = 0.0f;
                    }
                    // else
                    // {
                    //     Debug.Log(this.transform.position);
                    //     Debug.Log(hit.transform.name);
                    //     Debug.Log(hit.transform.position);
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Right"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        Vector3 roomtempPosition = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        //roomtemp = 
                        int rannum = Random.Range(1, rightRooms.Length-1);
                        if(rightRooms[rannum].name.Contains("(Clone)"))
                        {
                            PhotonNetwork.Instantiate(mapDir + rightRooms[rannum].name.Substring(0, rightRooms[rannum].name.Length - 7), roomtempPosition, Quaternion.identity, 0);
                        }
                        else
                        {
                            PhotonNetwork.Instantiate(mapDir + rightRooms[rannum].name, roomtempPosition, Quaternion.identity, 0);
                        }
                        //roomtemp.transform.SetParent(this.transform.parent);
                        DungeonManager.roomNum--;
                        DungeonManager.mapCreateTimer = 0.0f;
                    }
                    // else
                    // {
                    //     Debug.Log(this.transform.position);
                    //     Debug.Log(hit.transform.name);
                    //     Debug.Log(hit.transform.position);
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Down"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        Vector3 roomtempPosition = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        //roomtemp = 
                        int rannum = Random.Range(1, downRooms.Length-1);
                        if(downRooms[rannum].name.Contains("(Clone)"))
                        {
                            PhotonNetwork.Instantiate(mapDir + downRooms[rannum].name.Substring(0, downRooms[rannum].name.Length - 7), roomtempPosition, Quaternion.identity, 0);
                        }
                        else
                        {
                            PhotonNetwork.Instantiate(mapDir + downRooms[rannum].name, roomtempPosition, Quaternion.identity, 0);
                        }
                        //roomtemp.transform.SetParent(this.transform.parent);
                        DungeonManager.roomNum--;
                        DungeonManager.mapCreateTimer = 0.0f;
                    }
                    // else
                    // {
                    //     Debug.Log(this.transform.position);
                    //     Debug.Log(hit.transform.name);
                    //     Debug.Log(hit.transform.position);
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Left"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        Vector3 roomtempPosition = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        //roomtemp = 
                        int rannum = Random.Range(1, leftRooms.Length-1);
                        if(leftRooms[rannum].name.Contains("(Clone)"))
                        {
                            PhotonNetwork.Instantiate(mapDir + leftRooms[rannum].name.Substring(0, leftRooms[rannum].name.Length - 7), roomtempPosition, Quaternion.identity, 0);
                        }
                        else
                        {
                            PhotonNetwork.Instantiate(mapDir + leftRooms[rannum].name, roomtempPosition, Quaternion.identity, 0);
                        }
                        //roomtemp.transform.SetParent(this.transform.parent);
                        DungeonManager.roomNum--;
                        DungeonManager.mapCreateTimer = 0.0f;
                    }
                    // else
                    // {
                    //     Debug.Log(this.transform.position);
                    //     Debug.Log(hit.transform.name);
                    //     Debug.Log(hit.transform.position);
                    // }
                }
            }
        }
        else
        {
            if(mapSpawnPoints.Length == 1)
            {
                DungeonManager.endRooms.Add(this.gameObject);
                DungeonManager.endRoomIndexChecker++;
            }
            for(int i = 2; i < mapSpawnPoints.Length; i++)
            {
                yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
                if(mapSpawnPoints[i].name.Contains("Up"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        Vector3 roomtempPosition = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        //roomtemp = 
                        PhotonNetwork.Instantiate(mapDir + upRooms[0].name, roomtempPosition, Quaternion.identity, 0);
                        //roomtemp.transform.SetParent(this.transform.parent);
                        DungeonManager.roomNum--;
                        DungeonManager.mapCreateTimer = 0.0f;
                    }
                    // else
                    // {
                    //     Debug.Log(this.transform.position);
                    //     Debug.Log(hit.transform.name);
                    //     Debug.Log(hit.transform.position);
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Right"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        Vector3 roomtempPosition = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        //roomtemp = 
                        PhotonNetwork.Instantiate(mapDir + rightRooms[0].name, roomtempPosition, Quaternion.identity, 0);
                        //roomtemp.transform.SetParent(this.transform.parent);
                        DungeonManager.roomNum--;
                        DungeonManager.mapCreateTimer = 0.0f;
                    }
                    // else
                    // {
                    //     Debug.Log(this.transform.position);
                    //     Debug.Log(hit.transform.name);
                    //     Debug.Log(hit.transform.position);
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Down"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        Vector3 roomtempPosition = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        //roomtemp = 
                        PhotonNetwork.Instantiate(mapDir + downRooms[0].name, roomtempPosition, Quaternion.identity, 0);
                        //roomtemp.transform.SetParent(this.transform.parent);
                        DungeonManager.roomNum--;
                        DungeonManager.mapCreateTimer = 0.0f;
                    }
                    // else
                    // {
                    //     Debug.Log(this.transform.position);
                    //     Debug.Log(hit.transform.name);
                    //     Debug.Log(hit.transform.position);
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Left"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        Vector3 roomtempPosition = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        //roomtemp = 
                        PhotonNetwork.Instantiate(mapDir + leftRooms[0].name, roomtempPosition, Quaternion.identity, 0);
                        //roomtemp.transform.SetParent(this.transform.parent);
                        DungeonManager.roomNum--;
                        DungeonManager.mapCreateTimer = 0.0f;
                    }
                    // else
                    // {
                    //     Debug.Log(this.transform.position);
                    //     Debug.Log(hit.transform.name);
                    //     Debug.Log(hit.transform.position);
                    // }
                }
            }
        }
        yield break;
    }
}
