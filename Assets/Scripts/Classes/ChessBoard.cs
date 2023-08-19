using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public static ChessBoard instance;
    public static Vector2Int BoardLocationMin = new Vector2Int(0, 0);
    public static Vector2Int BoardLocationMax = new Vector2Int(7, 7);
    private Vector2 BoardSize = new Vector2(2.105f, 2.105f);
    public List<GameObject> ChessPosition;
    public List<GameObject> ChessPrefab;
    private readonly GameObject[,] ChessBoardGrids = new GameObject[8, 8];
    public readonly Selection[,] ChessSelections = new Selection[8, 8];
    public GameObject GridPrefab;
    public Dictionary<ChessType, List<GameObject>> chessGO;
    public List<Material> materials;

    private void OnEnable()
    {
        InitChessGO();

        foreach (var PosGameObject in ChessPosition)
        {
            foreach (var pair in chessGO)
            {
                if(!PosGameObject.name.Contains(pair.Key.ToString())) continue;

                GameObject chessInstance = Instantiate(
                    ChessPrefab[Mathf.Abs((int)pair.Key) - 1], 
                    PosGameObject.transform.position, PosGameObject.transform.rotation);
                
                chessInstance.GetComponentInChildren<Renderer>().material = 
                    (int)pair.Key > 0 ? materials[0] : materials[1];
                pair.Value.Add(chessInstance);
            }
        }

        float x = BoardSize.x, y = BoardSize.y;    
        
        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Vector3 position = new Vector3(-7.37f + j * x, 0.001f, -7.37f + i * y);

            GameObject ChessBoardTile = Instantiate(GridPrefab, position, Quaternion.identity);
           
            ChessBoardGrids[i, j] = ChessBoardTile;
            Selection selection = ChessBoardTile.GetComponent<Selection>();
            ChessSelections[i, j] = selection;
            selection.Location = new Vector2Int(i, j);

        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        foreach (var pair in chessGO)
        foreach (var go in pair.Value)
        {
            int index = (int)pair.Key;
            int[] row = { 7, 6, 1, 0 };
            int[] col = { 0, 7, 1, 6, 2, 5, 3, 4 };
            foreach (var y in row)
            foreach (var x in col)
            {
                var Location = new Vector2Int(x, y);
                InitChessComponents(go, index, Location);
            }
        }
    }

    private void InitChessGO()
    {
        chessGO = new Dictionary<ChessType, List<GameObject>>
        {
            { ChessType.WhiteRock , new List<GameObject>()},
            { ChessType.WhiteKnight , new List<GameObject>()},
            { ChessType.WhiteBishop , new List<GameObject>()},
            { ChessType.WhiteQueen , new List<GameObject>()},
            { ChessType.WhiteKing , new List<GameObject>()},
            { ChessType.WhitePawn , new List<GameObject>()},
            { ChessType.BlackRock , new List<GameObject>()},
            { ChessType.BlackKnight , new List<GameObject>()},
            { ChessType.BlackBishop , new List<GameObject>()},
            { ChessType.BlackQueen , new List<GameObject>()},
            { ChessType.BlackKing , new List<GameObject>()},
            { ChessType.BlackPawn , new List<GameObject>()}
        };
    }

    public static void InitChessComponents(GameObject go, int index, Vector2Int Location)
    {
        var idx = Mathf.Abs(index);
            switch (idx)
            {
                case 1:
                {
                    Rock rock = go.AddComponent<Rock>();
                    rock.camp = index > 0 ? Camp.WHITE : Camp.BLACK;
                    rock.Location = Location;
                    break;
                }
                case 2:
                {
                    Knight knight = go.AddComponent<Knight>();
                    knight.camp = index > 0 ? Camp.WHITE : Camp.BLACK;
                    knight.Location = Location;
                    break;
                }
                case 3:
                {
                    Bishop bishop = go.AddComponent<Bishop>();
                    bishop.camp = index > 0 ? Camp.WHITE : Camp.BLACK;
                    bishop.Location = Location;
                    break;
                }
                case 4:
                {
                    Queen queen = go.AddComponent<Queen>();
                    queen.camp = index > 0 ? Camp.WHITE : Camp.BLACK;
                    queen.Location = Location;
                    break;
                }
                case 5:
                {
                    King king = go.AddComponent<King>();
                    king.camp = index > 0 ? Camp.WHITE : Camp.BLACK;
                    king.Location = Location;
                    break;
                }
                default:
                {
                    Pawn pawn = go.AddComponent<Pawn>();
                    pawn.camp = index > 0 ? Camp.WHITE : Camp.BLACK;
                    pawn.Location = Location;
                    break;
                }
            }
    }
}
