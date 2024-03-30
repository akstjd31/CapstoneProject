using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomController : MonoBehaviour
{
	public GameObject[] upRooms;
	public GameObject[] rightRooms;
	public GameObject[] downRooms;
	public GameObject[] leftRooms;
    public Transform[] mapSpawnPoints;

    public bool[] doorList = new bool[4];

    float roomCreateDelay = 0.1f;
    bool makeDoor = false;
    bool doorCheck = false;

    // Start is called before the first frame update
    void Start()
    {
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
                    Debug.Log(ranBin);
                    if(ranBin == 0)
                    {
                        Debug.Log("breakDoor");
                        //hit.transform.localScale = new Vector3(1, 1, 1);
                        hit.transform.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("makeDoor");

                        mapSpawnPoints[i].GetChild(0).gameObject.SetActive(true);
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
            Debug.Log(this.transform.position);
            for(int i = 0; i < 4; i++)
            {
                Debug.Log(doorList[i]);
            } 
        }
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
                GameObject roomtemp;
                if(mapSpawnPoints[i].name.Contains("Up"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        roomtemp = Instantiate(upRooms[Random.Range(1, upRooms.Length-1)]);
                        roomtemp.transform.position = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        roomtemp.transform.SetParent(this.transform.parent);
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
                        roomtemp = Instantiate(rightRooms[Random.Range(1, rightRooms.Length-1)]);
                        roomtemp.transform.position = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        roomtemp.transform.SetParent(this.transform.parent);
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
                        roomtemp = Instantiate(downRooms[Random.Range(1, downRooms.Length-1)]);
                        roomtemp.transform.position = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        roomtemp.transform.SetParent(this.transform.parent);
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
                        roomtemp = Instantiate(leftRooms[Random.Range(1, leftRooms.Length-1)]);
                        roomtemp.transform.position = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        roomtemp.transform.SetParent(this.transform.parent);
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
                yield return new WaitForSeconds(roomCreateDelay);
                GameObject roomtemp;
                if(mapSpawnPoints[i].name.Contains("Up"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, 0.5f, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up * 0.5f, Color.red);
                    if(hit.collider == null)
                    {
                        roomtemp = Instantiate(upRooms[0]);
                        roomtemp.transform.position = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        roomtemp.transform.SetParent(this.transform.parent);
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
                        roomtemp = Instantiate(rightRooms[0]);
                        roomtemp.transform.position = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        roomtemp.transform.SetParent(this.transform.parent);
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
                        roomtemp = Instantiate(downRooms[0]);
                        roomtemp.transform.position = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        roomtemp.transform.SetParent(this.transform.parent);
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
                        roomtemp = Instantiate(leftRooms[0]);
                        roomtemp.transform.position = new Vector2(mapSpawnPoints[i].transform.position.x, mapSpawnPoints[i].transform.position.y);
                        roomtemp.transform.SetParent(this.transform.parent);
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