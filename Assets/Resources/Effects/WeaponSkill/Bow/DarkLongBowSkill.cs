using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;

public class DarkLongBowSkill : MonoBehaviour
{
    Position skillPos;
    float destroyTimer = 0.0f;
    public float duration = 1.4f;
    [SerializeField] private int viewID = -1;
    // Start is called before the first frame update

    [PunRPC]
    public void InitializeDarkLongBowSkill(int viewID)
    {
        this.viewID = viewID;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        destroyTimer += Time.deltaTime;
        if(destroyTimer > duration)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
