using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Skill : MonoBehaviour
{
    public Status status;
    public PlayerCtrl playerCtrl;
    // Start is called before the first frame update
    void Awake()
    {
        playerCtrl = this.GetComponent<PlayerCtrl>();
        status = this.GetComponent<Status>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
