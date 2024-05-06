using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;

public class RoomController : MonoBehaviourPunCallbacks
{
    GameObject startPoint;
	public GameObject[] upRooms;
	public GameObject[] rightRooms;
	public GameObject[] downRooms;
	public GameObject[] leftRooms;
    public Transform[] mapSpawnPoints;
    public GameObject[] mapArray = new GameObject[5];
    PhotonView roomView;
    string mapDir = "Dungeon/";
    DungeonManager dungeonManager;

    public bool[] doorList = new bool[4];
    public GameObject[] doorObject = new GameObject[4];

    bool makeDoor = false;
    bool doorCheck = false;
    bool makePlayMap = false;
    bool isNameChanged = false;

    private void GenerateNavMesh()
    {
        // Use this if you want to clear existing
        //NavMesh.RemoveAllNavMeshData();  
        var settings = NavMesh.CreateSettings();
        var buildSources = new List<NavMeshBuildSource>();
        // create floor as passable area
        var floor = new NavMeshBuildSource
        {
            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one),
            shape = NavMeshBuildSourceShape.Box,
            size = new Vector3(10, 1, 10)
        };
        buildSources.Add(floor);

        // Create obstacle 
        const int OBSTACLE = 1 << 0;
        var obstacle = new NavMeshBuildSource
        {
            transform = Matrix4x4.TRS(new Vector3(3, 0, 3), Quaternion.identity, Vector3.one),
            shape = NavMeshBuildSourceShape.Box,
            size = new Vector3(1, 1, 1),
            area = OBSTACLE
        };
        buildSources.Add(obstacle);

        // build navmesh
        NavMeshData built = NavMeshBuilder.BuildNavMeshData(
            settings, buildSources, new Bounds(Vector3.zero, new Vector3(10, 10, 10)),
            new Vector3(0, 0, 0), Quaternion.identity);
        NavMesh.AddNavMeshData(built);
    }

    // Start is called before the first frame update
    void Awake()
    {
        roomView = GetComponent<PhotonView>();
        startPoint = GameObject.Find("Spawn");
        dungeonManager = startPoint.transform.parent.GetComponent<DungeonManager>();
    }
    void Start()
    {
        //Invoke("CreateRoom", 0.3f);
        StartCoroutine(CreateRoom());
        //CreateRoom();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(dungeonManager.isMapCreate && !makeDoor)
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

                //GenerateNavMesh();
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
        if(dungeonManager.isMapCreate && makeDoor && !doorCheck)
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
        if(dungeonManager.isMapCreate && dungeonManager.specialRoomSelect && makeDoor && doorCheck && !makePlayMap)
        {
            if(this.name == dungeonManager.bossRoom.name)
            {
                Vector3 gridMapPosition = new Vector3((this.transform.position.x - startPoint.transform.position.x) * dungeonManager.mapSize[0]
                , (this.transform.position.y - startPoint.transform.position.y) * dungeonManager.mapSize[1], 0);
                PhotonNetwork.Instantiate(mapDir + mapArray[Random.Range(0, mapArray.Length)].name, gridMapPosition, Quaternion.identity, 0);
            }
            else if(this.name == dungeonManager.shopRoom.name)
            {
                Vector3 gridMapPosition = new Vector3((this.transform.position.x - startPoint.transform.position.x) * dungeonManager.mapSize[0]
                , (this.transform.position.y - startPoint.transform.position.y) * dungeonManager.mapSize[1], 0);
                PhotonNetwork.Instantiate(mapDir + mapArray[Random.Range(0, mapArray.Length)].name, gridMapPosition, Quaternion.identity, 0);
            }
            else if(this.name == dungeonManager.healRoom.name)
            {
                Vector3 gridMapPosition = new Vector3((this.transform.position.x - startPoint.transform.position.x) * dungeonManager.mapSize[0]
                , (this.transform.position.y - startPoint.transform.position.y) * dungeonManager.mapSize[1], 0);
                PhotonNetwork.Instantiate(mapDir + mapArray[Random.Range(0, mapArray.Length)].name, gridMapPosition, Quaternion.identity, 0);
            }
            else
            {
                //Debug.Log(mapDir + mapArray[Random.Range(0, mapArray.Length)].name);
                Vector3 gridMapPosition = new Vector3((this.transform.position.x - startPoint.transform.position.x) * dungeonManager.mapSize[0]
                , (this.transform.position.y - startPoint.transform.position.y) * dungeonManager.mapSize[1], 0);
                PhotonNetwork.Instantiate(mapDir + mapArray[Random.Range(0, mapArray.Length)].name, gridMapPosition, Quaternion.identity, 0);
            }
            makePlayMap = true;
        }
    }
    [PunRPC]
    void SetActiveRPC(string wallName, bool active)
    {
        GameObject.Find(wallName).SetActive(active);
    }
    private IEnumerator CreateRoom()
    {
        if(this.transform.position.x > dungeonManager.farherstX)
        {
            dungeonManager.farherstX = (int)this.transform.position.x;
        }
        else if(this.transform.position.y > dungeonManager.farherstY)
        {
            dungeonManager.farherstY = (int)this.transform.position.y;
        }
        else if(this.transform.position.x < dungeonManager.farherstMX)
        {
            dungeonManager.farherstMX = (int)this.transform.position.x;
        }
        else if(this.transform.position.y < dungeonManager.farherstMY)
        {
            dungeonManager.farherstMY = (int)this.transform.position.y;
        }

        int roomNumTemp = dungeonManager.roomNum;
        RaycastHit2D hit;
        mapSpawnPoints = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
        if(dungeonManager.roomNum > (int)roomNumTemp / 3)
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
                        dungeonManager.roomNum--;
                        dungeonManager.mapCreateTimer = 0.0f;
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
                        dungeonManager.roomNum--;
                        dungeonManager.mapCreateTimer = 0.0f;
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
                        dungeonManager.roomNum--;
                        dungeonManager.mapCreateTimer = 0.0f;
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
                        dungeonManager.roomNum--;
                        dungeonManager.mapCreateTimer = 0.0f;
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
                dungeonManager.endRooms.Add(this.gameObject);
                dungeonManager.endRoomIndexChecker++;
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
                        dungeonManager.roomNum--;
                        dungeonManager.mapCreateTimer = 0.0f;
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
                        dungeonManager.roomNum--;
                        dungeonManager.mapCreateTimer = 0.0f;
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
                        dungeonManager.roomNum--;
                        dungeonManager.mapCreateTimer = 0.0f;
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
                        dungeonManager.roomNum--;
                        dungeonManager.mapCreateTimer = 0.0f;
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
