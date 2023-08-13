using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public static ChessBoard instance;
    public List<GameObject> ChessPosition, Chess;
    public readonly GameObject[,] ChessBoardTiles = new GameObject[8, 8];
    public readonly Selection[,] ChessSelections = new Selection[8, 8];
    public List<Material> materials;
    public GameObject tile;
    private Dictionary<ChessType, List<GameObject>> chessGO;
    private Material white, black, selected, defaultMaterial;

    private void OnEnable()
    {
        InitChessGO();
        white = materials[0];
        black = materials[1]; 
        selected = materials[2];

        foreach (var PosGameObject in ChessPosition)
        {
            foreach (var pair in chessGO)
            {
                if(!PosGameObject.name.Contains(pair.Key.ToString())) continue;

                GameObject chessInstance = Instantiate(
                    Chess[Mathf.Abs((int)pair.Key) - 1], 
                    PosGameObject.transform.position,
                    PosGameObject.transform.rotation);
                
                chessInstance.GetComponentInChildren<Renderer>().material = (int)pair.Key > 0 ? white : black;
                pair.Value.Add(chessInstance);
            }
        }

        float x = Common.BoardSize.x, y = Common.BoardSize.y;    
        
        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            GameObject ChessBoardTile =
                Instantiate(tile, 
                    new Vector3(-7.5f + j * x, 0.1f, -7.45f + i * y), 
                    Quaternion.identity);
           
            ChessBoardTiles[i, j] = ChessBoardTile;
            ChessSelections[i, j] = ChessBoardTile.GetComponent<Selection>();
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

    public void SelectPiece(GameObject piece)
    {
        piece.GetComponent<Chess>().isSelected = true;
        MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();
        defaultMaterial = renderers.material;
        renderers.material = selected;
    }

    public void DeselectPiece(GameObject piece)
    {
        piece.GetComponent<Chess>().isSelected = false;
        MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();
        renderers.material = defaultMaterial;
    }

    public void MovePiece(GameObject piece, Vector2 gridPoint)
    {
        var pos = new Vector3(
            piece.transform.position.x,
            piece.transform.position.y,
            piece.transform.position.z);
        print("cur Pos"+pos);
        var tarPos = Geometry.PointFromGrid(gridPoint);
        print("tar Pos"+tarPos);
        print("SUB" + (tarPos - pos));
        piece.transform.position = tarPos;
    }
}
