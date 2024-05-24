using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttackCtrl : MonoBehaviour
{
    private GameObject player;
    GameObject weaponRotator;
    GameObject weaponHolder;
    GameObject weapon;
    GameObject effect;
    float weaponRotatorRotateSpeed = 1440.0f;
    float weaponHolderRotateSpeed = 1620.0f;
    Vector3 mousePos;
    Vector3 effectPullingPosition;
    bool isAttack = false;
    bool attackUp = false;
    // Start is called before the first frame update
    void Start()
    {
        weaponRotator = this.transform.GetChild(0).gameObject;
        weaponHolder = weaponRotator.transform.GetChild(0).gameObject;
        weapon = weaponHolder.transform.GetChild(0).gameObject;
        effect = weapon.GetComponent<WeaponEffect>().Effect;
        effectPullingPosition = new Vector3(50000.0f, 50000.0f, 50000.0f);
        effect = Instantiate(effect, effectPullingPosition, this.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.75f, 0);
        }

        Debug.Log(this.name);
        Debug.Log(weaponRotator.name);
        Debug.Log(weaponHolder.name);
        Debug.Log(weapon.name);
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        LookAt2D(this.gameObject, mousePos);
        if(Input.GetMouseButtonDown(0))
        {
            if(!isAttack)
            {
                effect.transform.position = this.transform.position;
                effect.transform.rotation = this.transform.rotation;
                effect.GetComponent<Animator>().SetBool("isAttack", true);
                StartCoroutine("Attack");
            }
        }
    }

    IEnumerator Attack()
    {
        Debug.Log("Attacking");
        isAttack = true;
        if(attackUp)
        {
            while(weaponRotator.transform.localRotation.z < 0.3)
            {
                yield return null;
                Debug.Log(weaponRotator.transform.localRotation.z);
                Debug.Log(weaponRotator.transform.rotation.z);
                weaponHolder.transform.Rotate(0, 0, weaponHolderRotateSpeed * Time.deltaTime);
                weaponRotator.transform.Rotate(0, 0, weaponRotatorRotateSpeed * Time.deltaTime);
            }
            attackUp = false;
            isAttack = false;
            effect.GetComponent<Animator>().SetBool("isAttack", false);
            yield break;
        }

        else
        {
            while(weaponRotator.transform.localRotation.z > -0.6)
            {
                yield return null;
                Debug.Log(weaponRotator.transform.localRotation.z);
                Debug.Log(weaponRotator.transform.rotation.z);
                weaponHolder.transform.Rotate(0, 0, -weaponHolderRotateSpeed * Time.deltaTime);
                weaponRotator.transform.Rotate(0, 0, -weaponRotatorRotateSpeed * Time.deltaTime);
            }
            attackUp = true;
            isAttack = false;
            effect.GetComponent<Animator>().SetBool("isAttack", false);
            yield break;
        }
    }

    void LookAt2D(GameObject weapon, Vector3 target)
    {
        Vector2 newPos = target - weapon.transform.position;
        float rotZ = Mathf.Atan2(newPos.y, newPos.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
