using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class RoomController : MonoBehaviour
{
	public GameObject[] topRooms;
	public GameObject[] rightRoom;
	public GameObject[] bottomRooms;
	public GameObject[] leftRooms;
    public Transform[] mapSpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        mapSpawnPoints = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
        for(int i = 0; i < mapSpawnPoints.Length; i++)
        {
            //Instantiate(mapArray[Random.Range(0, mapArray.Length)], mapSpawnPoints[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
