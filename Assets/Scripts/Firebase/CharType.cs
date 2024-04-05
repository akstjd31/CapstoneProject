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
        // UserInfoManager 인스턴스를 가져옴
        userInfoManager = UserInfoManager.instance;

        // 현재 로그인한 사용자 정보를 가져옴
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
            //해당 좌표에 있는 오브젝트 찾기
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

            if (hit.collider != null)
            {
                GameObject click_obj = hit.transform.gameObject;
                Debug.Log(click_obj.name);

            }

            /*
            // 마우스 위치에서 Ray를 발사합니다.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            Debug.Log($"coll : {hit.collider}");

            // Ray가 어떤 오브젝트에 부딪혔는지 확인합니다.
            if (hit.collider != null)
            {
                // 부딪힌 오브젝트의 이름을 출력합니다.
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
