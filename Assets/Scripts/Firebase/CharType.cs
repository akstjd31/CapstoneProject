using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharType : MonoBehaviour
{
    UserInfoManager userInfoManager;
    FirebaseUser currentUser;
    [SerializeField]
    private GameObject warrior;
    [SerializeField]
    private GameObject archer;
    private GameObject explane;
    private Text explane_Text;
    private GameObject charType_canvas;
    private Text charType_input;
    private GameObject nickname_canvas;
    private TextMeshProUGUI nickname_input;

    private const string explane_warrior = "���� ���� ����";
    private const string explane_archer = "��ó ���� ����";

    private bool isCharTypeCanvas = false;
    private bool isNicknameCanvas = false;
    Vector2 pos;
    RaycastHit2D hit;
    GameObject click_obj;

    private string selectedCharType = "";

    // Start is called before the first frame update
    void Start()
    {
        // UserInfoManager �ν��Ͻ��� ������
        userInfoManager = UserInfoManager.instance;

        // ���� �α����� ����� ������ ������
        //currentUser = userInfoManager.currentUser;

        warrior = GameObject.Find("Inner_Field_Warrior");
        archer = GameObject.Find("Inner_Field_Archer");
        explane = GameObject.Find("Char_Explanation");
        explane_Text = explane.GetComponentInChildren<Text>();
        charType_canvas = GameObject.Find("CharType_Confirm");
        charType_input = charType_canvas.GetComponentInChildren<Text>();
        nickname_canvas = GameObject.Find("Nickname");
        nickname_input = nickname_canvas.GetComponentInChildren<TextMeshProUGUI>();


        explane.SetActive(false);
        charType_canvas.SetActive(false);
        nickname_canvas.SetActive(false);
    }

    void Update()
    {
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //�ش� ��ǥ�� �ִ� ������Ʈ ã��
        hit = Physics2D.Raycast(pos, Vector2.zero);
        //Debug.Log($"isSelected : {isCharTypeCanvas}");

        //���� ���� ��
        if (!isCharTypeCanvas && !isNicknameCanvas)
        {
            charType_canvas.SetActive(false);
            warrior.GetComponent<BoxCollider2D>().enabled = true;
            archer.GetComponent<BoxCollider2D>().enabled = true;

            if (hit.collider != null)
            {
                click_obj = hit.collider.gameObject; //hit.transform.gameObject;
                                                     //Debug.Log($"hit-coll : {click_obj.name}");

                //���� ���� �˾� ����
                if (hit.collider.gameObject == warrior)
                {
                    explane.SetActive(true);
                    explane.transform.position = pos;
                    explane_Text.text = explane_warrior;
                }
                else if (hit.collider.gameObject == archer)
                {
                    explane.SetActive(true);
                    explane.transform.position = pos;
                    explane_Text.text = explane_archer;
                }
            }
            else
            {
                explane.SetActive(false);
            }

            //���� Ȯ�� ui�� ���� ����
            if (Input.GetMouseButton(0) && hit.collider != null)
            {
                explane.SetActive(false);
                isCharTypeCanvas = true;
            }
        }
        //���� ���� ��
        else if (isCharTypeCanvas && !isNicknameCanvas)
        {
            explane.SetActive(false);
            charType_canvas.SetActive(true);
            warrior.GetComponent<BoxCollider2D>().enabled = false;
            archer.GetComponent<BoxCollider2D>().enabled = false;

            if (click_obj == warrior)
            {
                charType_input.text = "`����` ������ �����Ͻðڽ��ϱ�?";
                selectedCharType = "warrior";
            }
            else if (click_obj == archer)
            {
                charType_input.text = "`��ó` ������ �����Ͻðڽ��ϱ�?";
                selectedCharType = "archer";
            }

            //ui ����
            if(Input.GetMouseButton(0) && hit.collider != null && hit.collider.name == "CharType_Exit")
            {
                //Debug.Log($"coll : {hit.collider.name}");
                charType_canvas.SetActive(false);
                isCharTypeCanvas = false;
            }
        }
    }

    public void SubmitCharType()
    {
        isNicknameCanvas = true;
        isCharTypeCanvas = false;

        warrior.GetComponent<BoxCollider2D>().enabled = false;
        archer.GetComponent<BoxCollider2D>().enabled = false;

        explane.SetActive(false);
        charType_canvas.SetActive(false);
        nickname_canvas.SetActive(true);
    }

    public void CancelSubmit()
    {
        isCharTypeCanvas = false;

        warrior.GetComponent<BoxCollider2D>().enabled = true;
        archer.GetComponent<BoxCollider2D>().enabled = true;

        explane.SetActive(false);
        charType_canvas.SetActive(false);
    }

    public void SubmitNickname()
    {
        string nickname = nickname_input.text;
        Debug.Log($"nickname : {nickname}");

        explane.SetActive(false);
        charType_canvas.SetActive(false);
        nickname_canvas.SetActive(false);

        //�� ��ȯ ����
        //selectedCharType
    }

    public void CancalSubmitNickname()
    {
        nickname_input.text = "";
        isNicknameCanvas = false;

        warrior.GetComponent<BoxCollider2D>().enabled = true;
        archer.GetComponent<BoxCollider2D>().enabled = true;

        explane.SetActive(false);
        charType_canvas.SetActive(false);
        nickname_canvas.SetActive(false);
    }
}
