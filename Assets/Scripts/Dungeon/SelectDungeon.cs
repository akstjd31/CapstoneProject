using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDungeon : MonoBehaviour
{
    private HandleMap mapHandler;

    private GameObject MapContainer;
    private GameObject DungeonCanvas;
    private RawImage DungeonImage;
    private Button destroyButton;

    private Toggle[] dungeonPoints;
    [SerializeField]
    private Sprite PointOn;
    [SerializeField]
    private Sprite PointOff;
    private Sprite emptySprite;

    private Toggle nowSelectPoint;
    [SerializeField]
    private string dungeonName = "dungeon name";
    private Text enterName;

    private int mapSizeWidth = 1920;
    private int mapSizeHeight = 1080;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.AddComponent<HandleMap>();
        mapHandler = new();
        MapContainer = new GameObject("MapContainer");
        MapContainer.transform.SetParent(GameObject.Find("Canvas").transform, false);

        MakeDungeonMap();
    }

    private void SetDungeonPoint()
    {
        Vector2 origin = DungeonCanvas.transform.position;  //center of image
        Vector2 mapSize = DungeonCanvas.GetComponent<RectTransform>().sizeDelta;

        //position of points
        //range of x : origin.x - mapSize.x / 2 ~ origin.x + mapSize.x / 2
        //range of y : origin.y - mapSize.y / 2 ~ origin.y + mapSize.y / 2
        List<List<float>> points = new()
        {
            new List<float> { origin.x - mapSize.x / 2.5f,      origin.y + mapSize.y / 2.5f },
            new List<float> { origin.x - mapSize.x / 4,         origin.y - mapSize.y / 4 },
            new List<float> { origin.x + mapSize.x / 2.5f,      origin.y - mapSize.y / 4 },
            new List<float> { origin.x + mapSize.x / 4,         origin.y + mapSize.y / 4 },
        };

        dungeonPoints = new Toggle[points.Count];

        int cnt = 0;
        ToggleGroup toggleGroup = DungeonCanvas.AddComponent<ToggleGroup>();

        foreach (List<float> point in points)
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
            toggleRectTransform.sizeDelta = new Vector2((float)(mapSize.x * 0.13), (float)(mapSize.x * 0.13));
            toggle.graphic = checkmark.GetComponent<Image>();

            if (emptySprite == null)
            {
                emptySprite = bg.sprite;
            }

            SetBackground(bg);
            SetCheckmark(checkmark);
            SetDesc(toggle);

            toggle.gameObject.transform.position = new Vector2(point[0], point[1]);
            //initial sprite
            checkmark.sprite = emptySprite;
            bg.sprite = PointOff;

            toggle.group = toggleGroup;

            toggle.onValueChanged.AddListener((bool isOn) =>
            {
                checkmark.sprite = isOn ? PointOn : emptySprite;
                bg.sprite = isOn ? null : PointOff;
                nowSelectPoint = toggle;

                dungeonName = toggle.name;
                enterName.text = dungeonName;
            });

            dungeonPoints[cnt++] = toggle;
        }
    }

    private void SetDesc(Toggle toggle)
    {
        float boxSize_x = 300;
        float boxSize_y = 100;
        float newY = toggle.transform.position.y + toggle.GetComponent<RectTransform>().sizeDelta.y * 0.5f;

        //background
        GameObject newDescContainer = new GameObject("desc");
        newDescContainer.AddComponent<RawImage>();
        newDescContainer.transform.position = new Vector2(toggle.transform.position.x, newY);
        newDescContainer.transform.SetParent(toggle.transform);

        string imagePath = "PixelArtGUI/Textures/Panels/TitleBar";
        Texture2D texture = Resources.Load<Texture2D>(imagePath);
        newDescContainer.GetComponent<RawImage>().texture = texture;

        RectTransform descContainerRectTransform = newDescContainer.GetComponent<RectTransform>();
        if (descContainerRectTransform != null)
        {
            descContainerRectTransform.sizeDelta = new Vector2(boxSize_x, boxSize_y); 
        }

        //text
        GameObject descTextObject = new("DescText");
        RectTransform descTextRectTransform = descTextObject.AddComponent<RectTransform>();

        descTextRectTransform.anchorMin = Vector2.zero;
        descTextRectTransform.anchorMax = Vector2.one;
        descTextRectTransform.pivot = Vector2.zero;
        descTextRectTransform.sizeDelta = new Vector2(boxSize_x, boxSize_y);

        descTextObject.transform.SetParent(newDescContainer.transform);

        // Text 컴포넌트 추가
        Text descText = descTextObject.AddComponent<Text>();
        descText.text = "토글 위 던전 설명";
        descText.color = Color.black;
        descText.fontSize = 32;
        descText.fontStyle = FontStyle.Bold;
        descText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        descTextRectTransform.anchoredPosition = Vector2.zero;
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
        DungeonCanvas.transform.SetParent(MapContainer.transform, false);
        DungeonCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(mapSizeWidth, mapSizeHeight);

        string imagePath = "dungeon_enter_map";
        Texture2D texture = Resources.Load<Texture2D>(imagePath);
        DungeonCanvas.GetComponent<RawImage>().texture = texture;

        //button
        GameObject buttonObject = new GameObject("DestroyButton");
        destroyButton = buttonObject.AddComponent<Button>();

        Text buttonText = buttonObject.AddComponent<Text>();
        buttonText.text = "X";
        buttonText.fontSize = 56;
        buttonText.color = Color.black;
        
        buttonObject.transform.SetParent(GameObject.Find("Canvas").transform, false);

        RectTransform buttonTransform = buttonObject.GetComponent<RectTransform>();

        // set button's position
        buttonTransform.anchorMin = new Vector2(1f, 1f);
        buttonTransform.anchorMax = new Vector2(1f, 1f);
        buttonTransform.pivot = new Vector2(1f, 1f);
        buttonTransform.anchoredPosition = new Vector2(-10f, -10f);

        destroyButton.onClick.AddListener(DisableDungeonCanvas);

        //information of selected dungeon
        EnterUI();
        //make clickable point
        SetDungeonPoint();

        DungeonCanvas.AddComponent<HandleMap>();
        MapContainer.SetActive(false);
    }

    private void EnterUI()
    {
        //information of selected dungeon UI
        Image EnterDungeonUI = new GameObject("Dungeon_Enter").AddComponent<Image>();
        EnterDungeonUI.transform.SetParent(MapContainer.transform, false);

        RectTransform EnterTransform = EnterDungeonUI.GetComponent<RectTransform>();
        EnterTransform.anchorMin = new Vector2(0.5f, 0f);
        EnterTransform.anchorMax = new Vector2(0.5f, 0f);
        EnterTransform.pivot = new Vector2(0.5f, 0f);
        EnterTransform.anchoredPosition = new Vector2(0f, mapSizeHeight / 10);
        EnterTransform.sizeDelta = new Vector2((float)(mapSizeWidth * 0.25), (float)(mapSizeHeight * 0.15));
        EnterTransform.transform.position = new Vector2(MapContainer.transform.position.x, 
            MapContainer.transform.position.x - mapSizeHeight * 0.85f);

        EnterDungeonUI.color = new Color(0f, 0f, 0f, 0.7f);

        //button
        Button btn_enter = new GameObject("btn_Enter").AddComponent<Button>();
        btn_enter.onClick.AddListener(() =>
        {
            Debug.Log("change scene");

        });
        btn_enter.transform.SetParent(EnterDungeonUI.transform, false);

        CanvasRenderer btnRenderer = btn_enter.gameObject.AddComponent<CanvasRenderer>();
        RectTransform btnRectTransform = btn_enter.gameObject.AddComponent<RectTransform>();
        Image btnImage = btn_enter.gameObject.AddComponent<Image>();

        btnRenderer.cullTransparentMesh = true;
        btnImage.color = new Color(0f, 0f, 0.8f, 1f);

        btnRectTransform.anchorMin = new Vector2(1f, 0.5f);
        btnRectTransform.anchorMax = new Vector2(1f, 0.5f);
        btnRectTransform.pivot = new Vector2(1f, 0.5f);
        btnRectTransform.anchoredPosition = new Vector2(-(float)(EnterTransform.sizeDelta[0] * 0.05), 0f);

        //text
        enterName = new GameObject("enter_name").AddComponent<Text>();
        RectTransform nameTransform = enterName.gameObject.GetComponent<RectTransform>();

        enterName.transform.SetParent(EnterDungeonUI.transform, false);

        enterName.text = dungeonName;
        enterName.alignment = TextAnchor.MiddleLeft;
        enterName.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        enterName.fontSize = 32;
        enterName.fontStyle = FontStyle.Bold;

        nameTransform.anchorMin = new Vector2(0f, 0.5f);
        nameTransform.anchorMax = new Vector2(0f, 0.5f);
        nameTransform.pivot = new Vector2(0f, 0.5f);
        nameTransform.anchoredPosition = new Vector2((float)(EnterTransform.sizeDelta[0] * 0.05), 0f);
        nameTransform.sizeDelta = new Vector2((float)(EnterTransform.sizeDelta[0] * 0.65), (float)(EnterTransform.sizeDelta[1] * 0.4));
    }

    public void DisableDungeonCanvas()
    {
        if (MapContainer != null)
        {
            MapContainer.SetActive(false);
        }
    }

    public void EnableDungeonCanvas()
    {
        if (MapContainer != null)
        {
            MapContainer.SetActive(true);
            DungeonCanvas.AddComponent<HandleMap>();
        }
    }
}
