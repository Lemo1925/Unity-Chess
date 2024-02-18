using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ChessBoard : SingletonMonoPun<ChessBoard>
{
    public GameObject gridPrefab;
    public Transform chessCollection, selectionCollection;
    public List<Material> materials;
    public List<GameObject> chessPosition;
    public List<GameObject> chessPrefab;
    public Dictionary<ChessType, List<GameObject>> chessGo;
    private readonly Vector2 _boardSize = new(2.105f, 2.105f);
    public readonly BoardGrid[,] ChessSelections = new BoardGrid[8, 8];
    
    protected override void Awake()
    {
        base.Awake();
        chessCollection = GameObject.Find("ChessCollection").transform;
        selectionCollection = GameObject.Find("SelectionCollection").transform;
        
    }

    private void Start()
    {
        
        InitChessGo();

        foreach (var posGameObject in chessPosition)
        {
            foreach (var pair in chessGo)
            {
                if(!posGameObject.name.Contains(pair.Key.ToString())) continue;

                GameObject chessInstance = Instantiate(
                    chessPrefab[Mathf.Abs((int)pair.Key) - 1], 
                    posGameObject.transform.position, posGameObject.transform.rotation,chessCollection);
                
                chessInstance.GetComponentInChildren<Renderer>().material = 
                    (int)pair.Key > 0 ? materials[0] : materials[1];
                pair.Value.Add(chessInstance);
            }
        }

        float x = _boardSize.x, y = _boardSize.y;    
        
        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Vector3 position = new Vector3(-7.37f + j * x, 0.001f, -7.37f + i * y);
            
            GameObject chessBoardTile = Instantiate(gridPrefab, position, Quaternion.identity,selectionCollection);
           
            BoardGrid grid = chessBoardTile.GetComponent<BoardGrid>();
            ChessSelections[i, j] = grid;
            grid.location = new Vector2Int(i, j);
        }
        
        
        var locationList = GetLocation();
        var index = 0;
        foreach (var pair in chessGo)
        foreach (var go in pair.Value)
            InitChessComponents(go, (int)pair.Key, locationList[index++]);

    }

    private static List<Vector2Int> GetLocation()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        int[] row = { 7, 6, 0, 1 };
        int[] col = { 0, 7, 1, 6, 2, 5, 3, 4 };
        foreach (var y in row)
        foreach (var x in col)
            list.Add(new Vector2Int(x, y));

        return list;
    }
    
    private void InitChessGo()
    {
        chessGo = new Dictionary<ChessType, List<GameObject>>
        {
            { ChessType.WhiteRock, new List<GameObject>() },
            { ChessType.WhiteKnight, new List<GameObject>() },
            { ChessType.WhiteBishop, new List<GameObject>() },
            { ChessType.WhiteQueen, new List<GameObject>() },
            { ChessType.WhiteKing, new List<GameObject>() },
            { ChessType.WhitePawn, new List<GameObject>() },
            { ChessType.BlackRock, new List<GameObject>() },
            { ChessType.BlackKnight, new List<GameObject>() },
            { ChessType.BlackBishop, new List<GameObject>() },
            { ChessType.BlackQueen, new List<GameObject>() },
            { ChessType.BlackKing, new List<GameObject>() },
            { ChessType.BlackPawn, new List<GameObject>() }
        };
    }

    public static void InitChessComponents(GameObject go, int index, Vector2Int location)
    {
        switch (Mathf.Abs(index))
        {
            case 1:
            {
                Rock rock = go.AddComponent<Rock>();
                rock.camp = index > 0 ? Camp.White : Camp.Black;
                rock.location = location;
                break;
            }
            case 2:
            {
                Knight knight = go.AddComponent<Knight>();
                knight.camp = index > 0 ? Camp.White : Camp.Black;
                knight.location = location;
                break;
            }
            case 3:
            {
                Bishop bishop = go.AddComponent<Bishop>();
                bishop.camp = index > 0 ? Camp.White : Camp.Black;
                bishop.location = location;
                break;
            }
            case 4:
            {
                Queen queen = go.AddComponent<Queen>();
                queen.camp = index > 0 ? Camp.White : Camp.Black;
                queen.location = location;
                break;
            }
            case 5:
            {
                King king = go.AddComponent<King>();
                king.camp = index > 0 ? Camp.White : Camp.Black;
                king.location = location;
                break;
            }
            default:
            {
                Pawn pawn = go.AddComponent<Pawn>();
                pawn.camp = index > 0 ? Camp.White : Camp.Black;
                pawn.location = location;
                break;
            }
        }
    }
}
