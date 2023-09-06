using Photon.Pun;
using UnityEngine;

public class GameStatus : MonoBehaviourPun
{
    public static GameStatus instance;
    public GameObject chessboard;

    public bool isOver, isPromotion;
    public int count;
    public Chess selectChess;
    public Vector2 current, target;
    public string moveType;
    
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        moveType = "Default";
        isPromotion = false;
        isOver = false;
        count = 0;
    }

    public void GameInit()
    {
        if (GameManager.model == GameModel.MULTIPLE)
        {
            if (GameManager.ready == 2)
            {
                UIController.Instance.isReady();
                Instantiate(chessboard, transform.localPosition, Quaternion.identity, transform);
                UIController.CameraTransition(GameManager.player.transform);
                GameController.state = GameState.StandBy;
            }    
        }
        else
        {
            Instantiate(chessboard, transform.localPosition, Quaternion.identity, transform);
            GameController.state = GameState.StandBy;
        }
    }

    public void StandBy()
    {
        if (isOver) return;
        GameController.RoundType = (Camp)(count % 2);
        if (GameManager.model == GameModel.MULTIPLE)
        {
            while (GameController.RoundType == Player.instance.camp)
            {
                GameController.state = GameState.Action;
                break;
            }
        }
        else
        {
            GameController.state = GameState.Action;
        }
        Timer.instance.StartTimer(180.0f);
    }

    public void Action(ref bool select, ref bool deselect)
    {
        if (Input.GetMouseButtonDown(0)) select = true;
        if (Input.GetMouseButtonDown(1)) deselect = true;
        if (selectChess != null)
        {
            current = new Vector2(selectChess.lastLocation.x, selectChess.lastLocation.y);
            target =  new Vector2(selectChess.Location.x, selectChess.Location.y);
        }
        
        if (Chess.isMoved)
        {
            GameController.state = GameState.End;
        }
    }
    
    public void End()
    {
        count++;
        Timer.instance.ResetTimer();
        Chess.isMoved = false;
        MatchManager.Instance.checkmate = -1;
        if (GameManager.model == GameModel.SINGLE)
        {
            EventManager.CallOnCameraChanged();
            GameController.state = GameState.StandBy;
        }
        else
        {
            photonView.RPC("SyncMove", RpcTarget.Others, current, target, moveType);
            moveType = "Default";
            GameController.state = GameState.StandBy;
        }
        
        // checkmate检测
        EventManager.CallOnTurnEnd();            
    }
    
    [PunRPC] public void SyncMove(Vector2 current, Vector2 target, string moveType)
    {
        var currentSelection = Selection.GetSelection(new Vector2Int((int)current.x, (int)current.y));
        var targetSelection = Selection.GetSelection(new Vector2Int((int)target.x, (int)target.y));
        
        Chess chess = currentSelection.GetPiece();
        chess.MovePiece(targetSelection.Location);
        chess.Location = targetSelection.Location;

        currentSelection.occupyType = Selection.OccupyGridType.NoneOccupyGrid;
        currentSelection.chessList.Clear();
        currentSelection.chessPiece = null;
        
        targetSelection.occupyType = (Selection.OccupyGridType)chess.camp;
        targetSelection.chessList.Add(chess);
        targetSelection.chessPiece = chess;
        
        if (chess.GetComponent<Pawn>() != null)
        {
            chess.GetComponent<Pawn>().moveTurn = count;
            chess.GetComponent<Pawn>().firstMoveStep = Mathf.Abs(targetSelection.Location.y - currentSelection.Location.y);
        }

        switch (moveType)
        {
            case "RookPromotion":
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                chess.GetComponent<Pawn>().PromotionLogic(chess.camp == Camp.WHITE ? 
                    ChessType.WhiteRock : ChessType.BlackRock, true);
                break;
            case "KnightPromotion":
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                chess.GetComponent<Pawn>().PromotionLogic(chess.camp == Camp.WHITE ? 
                    ChessType.WhiteKnight : ChessType.BlackKnight, true);
                break;
            case "BishopPromotion":
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                chess.GetComponent<Pawn>().PromotionLogic(chess.camp == Camp.WHITE ? 
                    ChessType.WhiteBishop : ChessType.BlackBishop, true);
                break;
            case "QueenPromotion":
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                chess.GetComponent<Pawn>().PromotionLogic(chess.camp == Camp.WHITE ? 
                        ChessType.WhiteQueen : ChessType.BlackQueen, true);
                break;
            case "PassBy":
                chess.GetComponent<Pawn>().En_Pass(targetSelection);
                break;
            case "LongCast": 
                chess.GetComponent<King>().InitChessList();
                chess.GetComponent<King>().LongCastling();
                break;
            case "ShortCast": 
                chess.GetComponent<King>().InitChessList();
                chess.GetComponent<King>().ShortCastling();
                break;
            default:
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                break;
        }
        count++;
        Timer.instance.ResetTimer();
    }
}
