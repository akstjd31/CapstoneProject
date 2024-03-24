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
        RaycastHit2D hit;
        mapSpawnPoints = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
        if(DungeonManager.roomNum > 0)
        {
            for(int i = 2; i < mapSpawnPoints.Length; i++)
            {
                if(mapSpawnPoints[i].name.Contains("Up"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, LayerMask.GetMask("Room Checker"));
                    Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 0.5f), Vector2.up, Color.red);
                    if(hit.collider == null)
                    {
                        Instantiate(upRooms[Random.Range(0, upRooms.Length-1)], mapSpawnPoints[i]);
                        DungeonManager.roomNum--;
                    }
                    else
                    {
                        Debug.Log("1");
                        Debug.Log(hit.transform.name);
                    }
                }
                else if(mapSpawnPoints[i].name.Contains("Right"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x + 0.5f, this.transform.position.y), Vector2.right, LayerMask.GetMask("Room Checker"));
                    if(hit.collider == null)
                    {
                        Instantiate(rightRooms[Random.Range(0, rightRooms.Length-1)], mapSpawnPoints[i]);
                        DungeonManager.roomNum--;
                    }
                    else
                    {
                        Debug.Log("2");
                        Debug.Log(hit.transform.name);
                    }
                }
                else if(mapSpawnPoints[i].name.Contains("Down"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), Vector2.down, LayerMask.GetMask("Room Checker"));
                    if(hit.collider == null)
                    {
                        Instantiate(downRooms[Random.Range(0, downRooms.Length-1)], mapSpawnPoints[i]);
                        DungeonManager.roomNum--;
                    }
                    else
                    {
                        Debug.Log("3");
                        Debug.Log(hit.transform.name);
                    }
                }
                else if(mapSpawnPoints[i].name.Contains("Left"))
                {
                    hit = Physics2D.Raycast(new Vector2(this.transform.position.x - 0.5f, this.transform.position.y), Vector2.left, LayerMask.GetMask("Room Checker"));
                    if(hit.collider == null)
                    {
                        Instantiate(leftRooms[Random.Range(0, leftRooms.Length-1)], mapSpawnPoints[i]);
                        DungeonManager.roomNum--;
                    }
                    else
                    {
                        Debug.Log("4");
                        Debug.Log(hit.transform.name);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
