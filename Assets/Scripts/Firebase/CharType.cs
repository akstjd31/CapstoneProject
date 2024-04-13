using Firebase.Auth;
using Firebase.Firestore;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    private Text nickname_Error_Text;

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
        nickname_Error_Text = GameObject.Find("nickname_errMsg").GetComponent<Text>();


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
        InnerSubmitNickname();
    }

    private async void InnerSubmitNickname()
    {
        string nickname = nickname_input.text;
        //Debug.Log($"nickname : {nickname}");

        //�Էµ� �г����� �ߺ��� ���
        if(!await IsDuplicationNickname_Async(nickname))
        {
            //Debug.Log("�ߺ��� �г���");
            nickname_Error_Text.text = "�ߺ��� �г����Դϴ�";
            return;
        }

        nickname_Error_Text.text = "";
        await UserData.SetNickname(nickname);

        explane.SetActive(false);
        charType_canvas.SetActive(false);
        nickname_canvas.SetActive(false);

        PhotonAccess();
    }

    private async Task<bool> IsDuplicationNickname_Async(string input)
    {
        bool result = true;

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.Log("failed makeDB : user is null");
            return false;
        }

        // Firestore �ν��Ͻ� ����
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // �÷��� "A"�� ��� ������ �����ɴϴ�.
        await db.Collection("Nickname").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;

                // ��� ������ ���� �ݺ��ϸ鼭 �ʵ� �̸��� �о�ɴϴ�.
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    IDictionary<string, object> data = document.ToDictionary();
                    Debug.Log($"data : {data}");
                    
                    if(data["nickname"].Equals(input))
                    {
                        result = false;
                        break;
                    }
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch documents: " + task.Exception);
            }
        });

        return result;
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

    private void PhotonAccess()
    {
        PhotonManager.ConnectWithRegister();
        //PhotonNetwork.ConnectUsingSettings();
    }
}
