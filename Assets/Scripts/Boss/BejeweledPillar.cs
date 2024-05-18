using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BejeweledPillar : MonoBehaviour
{
    public GameObject jewelObj;
    public Sprite[] jewelSprite;
    public bool flag = false;
    private Color color;
    int rand;

    [SerializeField] int idx;
    Dictionary<string, Color> jewelColorData = new Dictionary<string, Color>
{
    { "FG_Magma_Dungeon_794", Color.red },
    { "FG_Magma_Dungeon_716", Color.blue },
    { "FG_Magma_Dungeon_720", Color.green },
    { "FG_Magma_Dungeon_790", Color.yellow }
};
    
    
    private void Start()
    {
        this.GetComponent<PhotonView>().RPC("RandomIndexRPC", RpcTarget.All);
        idx = rand;

        jewelObj.GetComponent<SpriteRenderer>().sprite = jewelSprite[rand];
    }

    [PunRPC]
    private void RandomIndexRPC()
    {
        rand = Random.Range(0, 3);
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public bool Check(Color color)
    {
        string curName = jewelObj.GetComponent<SpriteRenderer>().sprite.name;
        if (jewelColorData[curName].Equals(color))
        {
            return true;
        }

        return false;
    }
    
    private void Update()
    {
        if (!flag && color != null)
        {
            flag = Check(color);
        }
    }

    [PunRPC]
    public void ChangeJewelColor()
    {
        // 인덱스의 끝에 도달한 경우
        if (idx == jewelSprite.Length-1)
        {
            idx = 0;
        }
        else
        {
            idx++;
        }

        jewelObj.GetComponent<SpriteRenderer>().sprite = jewelSprite[idx];
    }
}
