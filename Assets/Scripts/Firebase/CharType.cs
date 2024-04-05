using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharType : MonoBehaviour
{
    UserInfoManager userInfoManager;
    FirebaseUser currentUser;
    private GameObject warrior;
    private GameObject archer;

    Ray ray;
    RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        // UserInfoManager �ν��Ͻ��� ������
        userInfoManager = UserInfoManager.instance;

        // ���� �α����� ����� ������ ������
        //currentUser = userInfoManager.currentUser;

        warrior = GameObject.Find("Warrior");
        archer = GameObject.Find("Archer");

        if(warrior == null)
        {
            Debug.Log("warrior is null");
        }
        if(archer == null)
        {
            Debug.Log("archer is null");
        }
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //�ش� ��ǥ�� �ִ� ������Ʈ ã��
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

            if (hit.collider != null)
            {
                GameObject click_obj = hit.transform.gameObject;
                Debug.Log(click_obj.name);

            }

            /*
            // ���콺 ��ġ���� Ray�� �߻��մϴ�.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            Debug.Log($"coll : {hit.collider}");

            // Ray�� � ������Ʈ�� �ε������� Ȯ���մϴ�.
            if (hit.collider != null)
            {
                // �ε��� ������Ʈ�� �̸��� ����մϴ�.
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);
            }
            */
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("mouse enter");
    }
}
