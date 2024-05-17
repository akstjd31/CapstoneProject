using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BejeweledPillar : MonoBehaviour
{
    public GameObject jewelObj;
    public Sprite[] jewelSprite;

    [SerializeField] int idx;
    private void Start()
    {
        int rand = Random.Range(0, 3);
        idx = rand;

        jewelObj.GetComponent<SpriteRenderer>().sprite = jewelSprite[rand];
    }

    private void Update()
    {
        
    }

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
