using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharType : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    UserInfoManager userInfoManager;
    FirebaseUser currentUser;
    [SerializeField]
    private GameObject warrior;
    [SerializeField]
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

        warrior = GameObject.Find("Field_Warrior");
        archer = GameObject.Find("Field_Archer");

        if(warrior == null)
        {
            Debug.Log("warrior is null");
        }
        if(archer == null)
        {
            Debug.Log("archer is null");
        }

        Debug.Log($"is contain collider {warrior.GetComponent<BoxCollider2D>()}, {archer.GetComponent<BoxCollider2D>()}");
        Debug.Log($"{warrior.transform.position}, {archer.transform.position}");
    }

    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //해당 좌표에 있는 오브젝트 찾기
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject click_obj = hit.collider.gameObject; //hit.transform.gameObject;
            Debug.Log($"hit-coll : {click_obj.name}");
        }

        if (Input.GetMouseButton(0))
        {
            if (hit.collider != null)
            {
                GameObject clickObject = hit.transform.gameObject; //EventSystem.current.currentSelectedGameObject;
                Debug.Log($"click : {clickObject.name}");

                clickObject = EventSystem.current.currentSelectedGameObject;
                Debug.Log($"EventSystem : {clickObject.name}");
            }
            else
            {
                Debug.Log("click collider is null");
            }
        }

    }

    // 마우스 클릭시 호출될 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("빈 GameObject 클릭됨");
    }

    // 마우스가 해당 오브젝트에 들어왔을 때 호출될 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("빈 GameObject 호버 시작");
    }

    // 마우스가 해당 오브젝트에서 나갔을 때 호출될 함수
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("빈 GameObject 호버 종료");
    }

    void OnGUI()
    {
        
    }
}
