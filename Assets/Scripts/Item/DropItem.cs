using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 드랍되는 아이템이 정해지면, 실제로 생성하여 드랍시키는 스크립트
public class DropItem : MonoBehaviour
{
    private DropChanceCalculator dropCalc;
    [SerializeField] private string charType = "";   // 직업 정보

    private float plusXPos = 1;

    public AudioSource audioSource;
    public AudioClip[] itemDropSound;

    // Start is called before the first frame update
    void Start()
    {
        dropCalc = this.GetComponent<DropChanceCalculator>();
        audioSource = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    SpawnDroppedItem();
        //}
    }

    public void SetCharType(string charType)
    {
        this.charType = charType;
    }

    [PunRPC]
    public void SpawnDroppedItem()
    {
        ItemManager itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        Item spawnItem;

        ItemType itemType = dropCalc.RandomDropItem();

        Debug.Log(itemType + "당첨!");
        spawnItem = itemManager.GetRandomItemWithProbability(itemType, charType);

        // 보여주기식 위치 변경 -> 수정 필요
        Vector2 newPos = new Vector2(
            this.transform.position.x + plusXPos,
            this.transform.position.y
            );

        if (spawnItem.charType == CharacterType.WARRIOR)
        {
            audioSource.PlayOneShot(itemDropSound[0]);
        }
        else
        {
            audioSource.PlayOneShot(itemDropSound[1]);
        }

        Instantiate(spawnItem.prefab, newPos, Quaternion.identity);

        plusXPos += 1;
    }
}
