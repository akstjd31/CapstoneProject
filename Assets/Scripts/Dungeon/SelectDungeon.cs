using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDungeon : MonoBehaviour
{
    private GameObject DungeonCanvas;
    private RawImage DungeonImage;
    private Button destroyButton;

    private Toggle[] dungeonPoints;
    [SerializeField]
    private Sprite PointOn;
    [SerializeField]
    private Sprite PointOff;
    private Sprite emptySprite;

    private int mapSizeWidth = 1920;
    private int mapSizeHeight = 1080;

    // Start is called before the first frame update
    void Awake()
    {
        dungeonPoints = new Toggle[2];
        //PointOn = Resources.Load<Sprite>("backyard.png");
        //PointOff = Resources.Load<Sprite>("dungeon_enter_map.jpg");

        MakeDungeonMap();
    }

    private void SetDungeonPoint()
    {
        //position of points
        List<List<int>> points = new()
        {
            new List<int> { mapSizeWidth / 4, mapSizeHeight / 2 },
            new List<int> { mapSizeWidth / 4 * 3, mapSizeHeight / 2 }
        };

        int cnt = 0;

        foreach (List<int> point in points)
        {
            Toggle toggle;
            Image bg, checkmark;
            RectTransform toggleRectTransform;

            //structure
            toggle = new GameObject("Toggle").AddComponent<Toggle>();
            bg = new GameObject("Background").AddComponent<Image>();
            checkmark = new GameObject("Checkmark").AddComponent<Image>();

            toggle.transform.SetParent(DungeonCanvas.transform, false);
            bg.transform.SetParent(toggle.transform, false);
            checkmark.transform.SetParent(bg.transform, false);

            //components
            toggleRectTransform = toggle.GetComponent<RectTransform>();
            toggleRectTransform.sizeDelta = new Vector2(25, 25);
            toggle.graphic = checkmark.GetComponent<Image>();

            if(emptySprite == null)
            {
                emptySprite = bg.sprite;
            }

            SetBackground(bg);
            SetCheckmark(checkmark);

            toggle.gameObject.transform.position = new Vector2(point[0], point[1]);
            //initial sprite
            checkmark.sprite = emptySprite;
            bg.sprite = PointOff;

            toggle.onValueChanged.AddListener((bool isOn) =>
            {
                checkmark.sprite = isOn ? PointOn : emptySprite;
                bg.sprite = isOn ? null : PointOff;
            });

            dungeonPoints[cnt++] = toggle;
        }
    }

    private void SetCheckmark(Image check)
    {
        CanvasRenderer canvasRenderer = check.GetComponent<CanvasRenderer>();
        canvasRenderer.cullTransparentMesh = true;

        check.sprite = PointOff;
        check.color = Color.white;
        check.material = null;
        check.raycastTarget = true;
        check.maskable = true;
        check.type = Image.Type.Sliced;
        check.fillCenter = true;
    }

    private void SetBackground(Image bg)
    {
        CanvasRenderer canvasRenderer = bg.GetComponent<CanvasRenderer>();
        canvasRenderer.cullTransparentMesh = true;

        bg.color = Color.white;
        bg.material = null;
        bg.raycastTarget = true;
        bg.maskable = true;
        bg.type = Image.Type.Sliced;
        bg.fillCenter = true;
    }

    private void MakeDungeonMap()
    {
        DungeonCanvas = new GameObject("Dungeon_Select");
        DungeonImage = DungeonCanvas.AddComponent<RawImage>();
        DungeonCanvas.transform.SetParent(GameObject.Find("Canvas").transform, false);
        DungeonCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(mapSizeWidth, mapSizeHeight);

        string imagePath = "dungeon_enter_map";
        Texture2D texture = Resources.Load<Texture2D>(imagePath);
        DungeonCanvas.GetComponent<RawImage>().texture = texture;

        // 버튼 GameObject 생성
        GameObject buttonObject = new GameObject("DestroyButton");

        // 생성한 GameObject에 Button 컴포넌트 추가
        destroyButton = buttonObject.AddComponent<Button>();

        // 버튼에 텍스트 추가
        Text buttonText = buttonObject.AddComponent<Text>();
        buttonText.text = "X";
        buttonText.fontSize = 56;
        buttonText.color = Color.black;

        // 생성한 GameObject를 Canvas의 자식으로 설정
        buttonObject.transform.SetParent(GameObject.Find("Canvas").transform, false);

        // RectTransform 컴포넌트 가져오기
        RectTransform buttonTransform = buttonObject.GetComponent<RectTransform>();

        // set button's position
        buttonTransform.anchorMin = new Vector2(1f, 1f);
        buttonTransform.anchorMax = new Vector2(1f, 1f);
        buttonTransform.pivot = new Vector2(1f, 1f);
        buttonTransform.anchoredPosition = new Vector2(-10f, -10f); // 원하는 위치로 조정

        // 버튼이 클릭되었을 때의 동작 설정
        destroyButton.onClick.AddListener(DisableDungeonCanvas);

        //make clickable point
        SetDungeonPoint();

        DungeonCanvas.SetActive(false);
    }

    public void DisableDungeonCanvas()
    {
        if (DungeonCanvas != null)
        {
            DungeonCanvas.SetActive(false);
        }
    }
    
    public void EnableDungeonCanvas()
    {
        if(DungeonCanvas != null)
        {
            DungeonCanvas.SetActive(true);
        }
    }
}
