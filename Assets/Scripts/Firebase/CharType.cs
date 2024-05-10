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
    private Text explane_Title;
    private Text explane_Text;

    private GameObject charType_canvas;
    private Text charType_input;
    private string selectedCharType = "";

    private GameObject nickname_canvas;
    private TMP_InputField nickname_input;
    private Text nickname_Error_Text;
    private Button nickname_submit;

    private const string explane_warrior = "전사는 강력한 근접 전투 능력을 가진 전투 전문가입니다.\n" +
        "전사는 강력한 근접 무기를 사용하여 적을 공격합니다.\n" +
        "전사는 높은 방어력으로 적들을 끌어들이거나 집중 공격을 하여 전장을 지배하는 데 탁월합니다.";
    private const string explane_archer = "아처는 원거리 공격에 능숙한 전문가입니다.\n" +
        "전투 중에는 일반적으로 적들로부터 멀리 떨어져 유리한 위치에서 공격하며,\n" +
        "그들의 빠른 속도와 정확한 조준 능력을 활용하여 전투의 결과를 좌우할 수 있습니다.";

    private bool isCharTypeCanvas = false;
    private bool isNicknameCanvas = false;

    Vector2 pos;
    RaycastHit2D hit;
    GameObject click_obj;

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
        Transform bgTransform = explane.transform.Find("bg");
        explane_Title = bgTransform.transform.Find("Explane_Title").GetComponent<Text>();
        explane_Text = bgTransform.transform.Find("Explane_Text").GetComponent<Text>();

        charType_canvas = GameObject.Find("CharType_Confirm");
        bgTransform = charType_canvas.transform.Find("bg");
        charType_input = bgTransform.GetComponentInChildren<Text>();

        nickname_canvas = GameObject.Find("Nickname");
        bgTransform = nickname_canvas.transform.Find("bg");
        nickname_input = bgTransform.GetComponentInChildren<TMP_InputField>();
        nickname_Error_Text = bgTransform.Find("nickname_errMsg").GetComponent<Text>();
        nickname_submit = bgTransform.Find("btn_nickname_yes").GetComponent<Button>();

        explane.SetActive(false);
        charType_canvas.SetActive(false);
        nickname_canvas.SetActive(false);
    }

    public void OnEndEdit()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(nickname_input.text))
            {
                nickname_submit.onClick.Invoke();
            }
            else
            {
                nickname_Error_Text.text = "닉네임을 입력해주세요";
            }
        }
    }

    public void OnEndEditTMP()
    {
        if (!string.IsNullOrEmpty(nickname_input.text))
        {
            nickname_submit.onClick.Invoke();
        }
        else
        {
            nickname_Error_Text.text = "닉네임을 입력해주세요";
        }
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
                    explane_Title.text = "전사";
                    explane_Text.text = explane_warrior;
                }
                else if (hit.collider.gameObject == archer)
                {
                    explane.SetActive(true);
                    explane.transform.position = pos;
                    explane_Title.text = "아처";
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
                selectedCharType = "warrior";
            }
            else if (click_obj == archer)
            {
                charType_input.text = "`아처` 직업을 선택하시겠습니까?";
                selectedCharType = "archer";
            }

            //ui 종료
            if(Input.GetMouseButton(0) && hit.collider != null && hit.collider.name == "CharType_Exit")
            {
                //Debug.Log($"coll : {hit.collider.name}");
                charType_canvas.SetActive(false);
                isCharTypeCanvas = false;
            }

            GameObject.Find("PhotonManager").GetComponent<PhotonManager>().SetCharType(selectedCharType);
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
