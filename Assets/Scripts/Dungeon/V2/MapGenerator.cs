using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator :MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; //공간이 나눠지는 최소 비율
    [SerializeField] float maximumDivideRate; //공간이 나눠지는 최대 비율
    [SerializeField] private GameObject line; //lineRenderer를 사용해서 공간이 나눠진걸 시작적으로 보여주기 위함
    [SerializeField] private GameObject map; //lineRenderer를 사용해서 첫 맵의 사이즈를 보여주기 위함
    [SerializeField] private GameObject roomLine; //lineRenderer를 사용해서 방의 사이즈를 보여주기 위함
    [SerializeField] private int maximumDepth; //트리의 높이, 높을 수록 방을 더 자세히 나누게 됨
    [SerializeField] Tilemap tileMap; 
    [SerializeField] Tile[] roomTile; //방을 구성하는 타일
    [SerializeField] Tile[] wallTile; //방과 외부를 구분지어줄 벽 타일
    [SerializeField] Tile[] outTile; //방 외부의 타일
    int type;
    void Start()
    {   
       // DrawMap(0, 0);
        MakeMap(0);
    }
    void FillBackground()
    {
        for(int i = -10; i < mapSize.x+10; i++)
        {
            for(int j = -10; j < mapSize.y+10; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile[type]);
            }
        }
    }
    void FillWall()
    {      
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
               if(tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == outTile[type])
                {
    
                    for(int x = -1; x <= 1; x++)
                    {
                        for(int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;
                            if(tileMap.GetTile(new Vector3Int(i - mapSize.x / 2+x, j - mapSize.y / 2+y, 0)) == roomTile[type])
                            {
                                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0) , wallTile[type]);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    void Divide(Node tree,int n)
    {
        if (n == maximumDepth) return; //내가 원하는 높이에 도달하면 더 나눠주지 않는다.

        //그 외의 경우에는

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        //가로와 세로중 더 긴것을 구한후, 가로가 길다면 위 좌, 우로 세로가 더 길다면 위, 아래로 나눠주게 될 것이다.
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        //나올 수 있는 최대 길이와 최소 길이중에서 랜덤으로 한 값을 선택
        if (tree.nodeRect.width >= tree.nodeRect.height) //가로가 더 길었던 경우에는 좌 우로 나누게 될 것이며, 이 경우에는 세로 길이는 변하지 않는다.
        {
          
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x,tree.nodeRect.y,split,tree.nodeRect.height));
            //왼쪽 노드에 대한 정보다 
            //위치는 좌측 하단 기준이므로 변하지 않으며, 가로 길이는 위에서 구한 랜덤값을 넣어준다.
            tree.rightNode= new Node(new RectInt(tree.nodeRect.x+split, tree.nodeRect.y, tree.nodeRect.width-split, tree.nodeRect.height));
            //우측 노드에 대한 정보다 
            //위치는 좌측 하단에서 오른쪽으로 가로 길이만큼 이동한 위치이며, 가로 길이는 기존 가로길이에서 새로 구한 가로값을 뺀 나머지 부분이 된다.
           // DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
            //그 후 위 두개의 노드를 나눠준 선을 그리는 함수이다.        
        }
        else
        {
          
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width,split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width , tree.nodeRect.height-split));
            //DrawLine(new Vector2(tree.nodeRect.x , tree.nodeRect.y+ split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y  + split));
       //세로가 더 길었던 경우이다. 자세한 사항은 가로와 같다.
        }
        tree.leftNode.parNode = tree; //자식노드들의 부모노드를 나누기전 노드로 설정
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1); //왼쪽, 오른쪽 자식 노드들도 나눠준다.
        Divide(tree.rightNode, n + 1);//왼쪽, 오른쪽 자식 노드들도 나눠준다.
    }
    private RectInt GenerateRoom(Node tree,int n)
    {
        RectInt rect;
        if (n == maximumDepth) //해당 노드가 리프노드라면 방을 만들어 줄 것이다.
        {
           rect = tree.nodeRect;
            int width = Random.Range(rect.width / 2, rect.width - 1);
            //방의 가로 최소 크기는 노드의 가로길이의 절반, 최대 크기는 가로길이보다 1 작게 설정한 후 그 사이 값중 랜덤한 값을 구해준다.
            int height = Random.Range(rect.height / 2, rect.height - 1);
            //높이도 위와 같다.
            int x = rect.x + Random.Range(1, rect.width - width);
           //방의 x좌표이다. 만약 0이 된다면 붙어 있는 방과 합쳐지기 때문에,최솟값은 1로 해주고, 최댓값은 기존 노드의 가로에서 방의 가로길이를 빼 준 값이다.
           int y = rect.y + Random.Range(1, rect.height - height);        
           //y좌표도 위와 같다.
           rect = new RectInt(x, y, width, height);
           //DrawRectangle(rect);
           FillRoom(rect); 
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode,n+1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }
        return rect;
    }
    private void FillRoom(RectInt rect) {
    for(int i = rect.x; i< rect.x + rect.width; i++)
        {
            for(int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile[type]);
            }
        }
    }

    private void GenerateLoad(Node tree,int n)
    {
        if (n == maximumDepth) //리프 노드라면 이을 자식이 없다.
            return;
        Vector2Int leftNodeCenter = tree.leftNode.center;
        Vector2Int rightNodeCenter = tree.rightNode.center;      
       
           for (int i=Mathf.Min(leftNodeCenter.x, rightNodeCenter.x);i<=Mathf.Max(leftNodeCenter.x, rightNodeCenter.x); i++)
         {
             tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2, 0), roomTile[type]);
         }

         for (int j = Mathf.Min(leftNodeCenter.y, rightNodeCenter.y); j <= Mathf.Max(leftNodeCenter.y, rightNodeCenter.y); j++)
         {
             tileMap.SetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile[type]);
         }
         
        
     
        GenerateLoad(tree.leftNode, n + 1); //자식 노드들도 탐색
        GenerateLoad(tree.rightNode, n + 1);
    }


    private void DrawMap(int x, int y) //x y는 화면의 중앙위치를 뜻함
    {
        //기본적으로 mapSize/2라는 값을 계속해서 빼게 될건데, 화면의 중앙에서 화면의 크기의 반을 빼줘야 좌측 하단좌표를 구할 수 있기 때문이다.
        LineRenderer lineRenderer = Instantiate(map).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2); //좌측 하단
        lineRenderer.SetPosition(1, new Vector2(x + mapSize.x, y) - mapSize / 2); //우측 하단
        lineRenderer.SetPosition(2, new Vector2(x + mapSize.x, y + mapSize.y) - mapSize / 2);//우측 상단
        lineRenderer.SetPosition(3, new Vector2(x, y + mapSize.y) - mapSize / 2); //좌측 상단

    }
    private void DrawRectangle(RectInt rect)
    {
        LineRenderer lineRenderer = Instantiate(roomLine).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(rect.x, rect.y) - mapSize / 2); //좌측 하단
        lineRenderer.SetPosition(1, new Vector2(rect.x +rect.width, rect.y) - mapSize / 2); //우측 하단
        lineRenderer.SetPosition(2, new Vector2(rect.x + rect.width, rect.y + rect.height) - mapSize / 2);//우측 상단
        lineRenderer.SetPosition(3, new Vector2(rect.x, rect.y + rect.height) - mapSize / 2); //좌측 상단
    }
    private void DrawLine(Vector2 from, Vector2 to) //from->to로 이어지는 선을 그리게 될 것이다.
    {
        LineRenderer lineRenderer = Instantiate(line).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, from - mapSize / 2);
        lineRenderer.SetPosition(1, to - mapSize / 2);
    }
    public void MakeMap(int i)
    {
        type = i;
        FillBackground();
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        GenerateLoad(root, 0);
        FillWall();
    }
    
}
