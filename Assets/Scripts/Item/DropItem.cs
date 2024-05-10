using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 드랍되는 아이템이 정해지면, 실제로 생성하여 드랍시키는 스크립트
public class DropItem : MonoBehaviour
{
    private DropChanceCalculator dropCalc;
    // Start is called before the first frame update
    void Start()
    {
        dropCalc = this.GetComponent<DropChanceCalculator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SpawnDroppedItem();
        }
    }

    public void SpawnDroppedItem()
    {
        ItemManager itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        Item spawnItem;

        spawnItem = itemManager.GetRandomItemWithProbability(dropCalc.RandomDropItem());

        Instantiate(spawnItem.prefab, this.transform.position, Quaternion.identity); 
    }
}
