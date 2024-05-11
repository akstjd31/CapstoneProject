using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class ChestController : MonoBehaviourPunCallbacks
{
    string mapDir = "Dungeon/";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("asdfasdf");
            PhotonNetwork.Instantiate(mapDir + "Heart", this.transform.position, Quaternion.identity, 0);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}