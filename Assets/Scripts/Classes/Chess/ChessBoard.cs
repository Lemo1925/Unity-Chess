using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public static ChessBoard instance;
    public List<GameObject> ChessPosition;
    public List<GameObject> ChessPrefab;
    private readonly GameObject[,] ChessBoardGrids = new GameObject[8, 8];
    public readonly Selection[,] ChessSelections = new Selection[8, 8];
    public GameObject GridPrefab;
    private Dictionary<ChessType, List<GameObject>> chessGO;
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

        float x = Common.BoardSize.x, y = Common.BoardSize.y;    
        
        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            GameObject ChessBoardTile =
                Instantiate(GridPrefab, 
                    new Vector3(-7.5f + j * x, 0.1f, -7.45f + i * y), 
                    Quaternion.identity);
           
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
            var idx = Mathf.Abs((int)pair.Key);
            switch (idx)
            {
                case 1:
                {
                    Rock rock = go.AddComponent<Rock>();
                    rock.camp = (int)pair.Key > 0 ? Camp.WHITE : Camp.BLACK;
                    var row = (int)((int)pair.Key / (float)idx * 3.5f + 3.5f);
                    var col = pair.Value.IndexOf(go) * 7;
                    rock.Location = new Vector2Int(col, row);
                    break;
                }
                case 2:
                {
                    Knight knight = go.AddComponent<Knight>();
                    knight.camp = (int)pair.Key > 0 ? Camp.WHITE : Camp.BLACK;
                    var row = (int)((int)pair.Key / (float)idx * 3.5f + 3.5f);
                    var col = pair.Value.IndexOf(go) * 5 + 1;
                    knight.Location = new Vector2Int(col, row);
                    break;
                }
                case 3:
                {
                    Bishop bishop = go.AddComponent<Bishop>();
                    bishop.camp = (int)pair.Key > 0 ? Camp.WHITE : Camp.BLACK;
                    var row = (int)((int)pair.Key / (float)idx * 3.5f + 3.5f);
                    var col = pair.Value.IndexOf(go) * 3 + 2;
                    bishop.Location = new Vector2Int(col, row);
                    break;
                }
                case 4:
                {
                    Queen queen = go.AddComponent<Queen>();
                    queen.camp = (int)pair.Key > 0 ? Camp.WHITE : Camp.BLACK;
                    var row = (int)((int)pair.Key / (float)idx * 3.5f + 3.5f);
                    queen.Location = new Vector2Int(3, row);
                    break;
                }
                case 5:
                {
                    King king = go.AddComponent<King>();
                    king.camp = (int)pair.Key > 0 ? Camp.WHITE : Camp.BLACK;
                    var row = (int)((int)pair.Key / (float)idx * 3.5f + 3.5f);
                    king.Location = new Vector2Int(4, row);
                    break;
                }
                default:
                {
                    Pawn pawn = go.AddComponent<Pawn>();
                    pawn.camp = (int)pair.Key > 0 ? Camp.WHITE : Camp.BLACK;
                    var row = (int)((int)pair.Key / (float)idx * 2.5f + 3.5f);
                    var col = pair.Value.IndexOf(go);
                    pawn.Location = new Vector2Int(col, row);
                    break;
                }
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
}
