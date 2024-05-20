using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class IcycleWindBowSkill : MonoBehaviour
{
    Vector3 dir;
    float destroyTimer = 0.0f;
    public float duration = 0.1f;
    float angle;
    [SerializeField] private int viewID = -1;

    [PunRPC]
    public void InitializeShiningCompoundBowSkill(int viewID, Vector3 dir)
    {
        this.viewID = viewID;
        this.dir = dir;
    }

    // Start is called before the first frame update
    void Start()
    {
        angle = this.transform.rotation.z;
        var main = this.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        main.startRotation = angle;
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
