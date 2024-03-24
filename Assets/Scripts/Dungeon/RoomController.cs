using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomController : MonoBehaviour
{
	public GameObject[] upRooms;
	public GameObject[] rightRooms;
	public GameObject[] downRooms;
	public GameObject[] leftRooms;
    public Transform[] mapSpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("CreateRoom", 0.3f);
        Debug.Log(DungeonManager.roomNum);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateRoom()
    {
        RaycastHit2D hit;
        mapSpawnPoints = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
        if(DungeonManager.roomNum > 0)
        {
            for(int i = 2; i < mapSpawnPoints.Length; i++)
            {
                GameObject roomtemp;
                if(mapSpawnPoints[i].name.Contains("Up"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, Color.red);
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
                    //     Debug.Log("1");
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Right"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right, Color.red);
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
                    //     Debug.Log("2");
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Down"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down, Color.red);
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
                    //     Debug.Log("3");
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Left"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left, Color.red);
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
                    //     Debug.Log("4");
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
                GameObject roomtemp;
                if(mapSpawnPoints[i].name.Contains("Up"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, Color.red);
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
                    //     Debug.Log("5");
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Right"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right, Color.red);
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
                    //     Debug.Log("6");
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Down"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down, Color.red);
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
                    //     Debug.Log("7");
                    // }
                }
                else if(mapSpawnPoints[i].name.Contains("Left"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left, Color.red);
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
                    //     Debug.Log("8");
                    // }
                }
            }
        }
    }
}
