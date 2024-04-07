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
        // UserInfoManager �ν��Ͻ��� ������
        userInfoManager = UserInfoManager.instance;

        // ���� �α����� ����� ������ ������
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
        //�ش� ��ǥ�� �ִ� ������Ʈ ã��
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

    // ���콺 Ŭ���� ȣ��� �Լ�
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("�� GameObject Ŭ����");
    }

    // ���콺�� �ش� ������Ʈ�� ������ �� ȣ��� �Լ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("�� GameObject ȣ�� ����");
    }

    // ���콺�� �ش� ������Ʈ���� ������ �� ȣ��� �Լ�
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("�� GameObject ȣ�� ����");
    }

    void OnGUI()
    {
        
    }
}
