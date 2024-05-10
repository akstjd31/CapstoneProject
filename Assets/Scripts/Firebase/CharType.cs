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
    private PhotonManager photonManager;

    private const string explane_warrior = "전사 직업 설명";
    private const string explane_archer = "아처 직업 설명";

    private bool isCharTypeCanvas = false;
    private bool isNicknameCanvas = false;
    Vector2 pos;
    RaycastHit2D hit;
    GameObject click_obj;

    private string selectedCharType = "";

    // Start is called before the first frame update
    void Start()
    {
        // UserInfoManager 인스턴스를 가져옴
        userInfoManager = UserInfoManager.GetInstance();

        // 현재 로그인한 사용자 정보를 가져옴
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
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();


        explane.SetActive(false);
        charType_canvas.SetActive(false);
        nickname_canvas.SetActive(false);
    }

    void Update()
    {
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //해당 좌표에 있는 오브젝트 찾기
        hit = Physics2D.Raycast(pos, Vector2.zero);
        //Debug.Log($"isSelected : {isCharTypeCanvas}");

        //직업 선택 전
        if (!isCharTypeCanvas && !isNicknameCanvas)
        {
            charType_canvas.SetActive(false);
            warrior.GetComponent<BoxCollider2D>().enabled = true;
            archer.GetComponent<BoxCollider2D>().enabled = true;

            if (hit.collider != null)
            {
                click_obj = hit.collider.gameObject; //hit.transform.gameObject;
                                                     //Debug.Log($"hit-coll : {click_obj.name}");

                //직업 설명 팝업 설정
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

            //직업 확정 ui로 로직 변경
            if (Input.GetMouseButton(0) && hit.collider != null)
            {
                explane.SetActive(false);
                isCharTypeCanvas = true;
            }
        }
        //직업 선택 후
        else if (isCharTypeCanvas && !isNicknameCanvas)
        {
            explane.SetActive(false);
            charType_canvas.SetActive(true);
            warrior.GetComponent<BoxCollider2D>().enabled = false;
            archer.GetComponent<BoxCollider2D>().enabled = false;

            if (click_obj == warrior)
            {
                charType_input.text = "`전사` 직업을 선택하시겠습니까?";
                selectedCharType = "Warrior";
            }
            else if (click_obj == archer)
            {
                charType_input.text = "`아처` 직업을 선택하시겠습니까?";
                selectedCharType = "Archer";
            }

            //ui 종료
            if(Input.GetMouseButton(0) && hit.collider != null && hit.collider.name == "CharType_Exit")
            {
                //Debug.Log($"coll : {hit.collider.name}");
                charType_canvas.SetActive(false);
                isCharTypeCanvas = false;
            }

            photonManager.SetCharType(selectedCharType);
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

        //입력된 닉네임이 중복인 경우
        if(!await IsDuplicationNickname_Async(nickname))
        {
            //Debug.Log("중복된 닉네임");
            nickname_Error_Text.text = "중복된 닉네임입니다";
            return;
        }

        nickname_Error_Text.text = "";
        PhotonManager.SetNickname(nickname);
        await UserData.SetNickname(nickname);

        explane.SetActive(false);
        charType_canvas.SetActive(false);
        nickname_canvas.SetActive(false);

        Debug.Log("닉네임 설정 비동기 작업 완료");
        PhotonManager.ConnectWithRegister();
        //테스트 용 코드
        //SceneManager.LoadScene("TestScene");
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

        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 컬렉션 "A"의 모든 문서를 가져옵니다.
        await db.Collection("Nickname").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;

                // 모든 문서에 대해 반복하면서 필드 이름을 읽어옵니다.
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    IDictionary<string, object> data = document.ToDictionary();
                    //Debug.Log($"data : {data}");
                    
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
}
