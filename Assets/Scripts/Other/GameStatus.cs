using Controller;
using Photon.Pun;
using UnityEngine;
using Utils;

public class GameStatus : SingletonMonoPun<GameStatus>
{
    public GameObject chessboard;
    public static Camp RoundType;

    public static int Count;
    public static bool IsOver, IsPromotion;
    public static string MoveType;

    public Chess selectChess;
    private Vector2 _current, _target;

    private void Start()
    {
        MoveType = "Default";
        RoundType = Camp.White;
        IsPromotion = false;
        IsOver = false;
        Count = 0;
    }

    #region Game Loop
    
    public void GameInit()
    {
        if (GameManager.model == GameModel.Multiple)
        {
            UIController.Instance.readyPanel.Show();
            UIController.Instance.SetStatusMessage("Wait");
            if (GameManager.ready == 2)
            {
                UIController.Instance.readyPanel.Hid();
                GameManager.ready = 0;
                Instantiate(chessboard, transform.localPosition, Quaternion.identity, transform);
                CameraController.Instance.InitCameraFlag(GameManager.GetPlayer());
                CameraController.Instance.ChangeCameraPos();
                GameController.State = GameState.StandBy;
            }
        }
        else
        {
            Instantiate(chessboard, transform.localPosition, Quaternion.identity, transform);
            GameController.State = GameState.StandBy;
        }
    }

    public void StandBy()
    {
        if (IsOver)
        {
            GameController.State = GameState.Over;
            return; 
        }

        RoundType = (Camp)(Count % 2);
        UIController.Instance.SetStatusMessage(RoundType.ToString());

        if (GameManager.model == GameModel.Multiple)
        {
            while (RoundType == Player.Instance.camp)
            {
                GameController.State = GameState.Action;
                break;
            }
        }
        else
        {
            GameController.State = GameState.Action;
        }
        Timer.Instance.StartTimer();
    }

    public void Action(ref bool select, ref bool deselect)
    {
        if (Input.GetMouseButtonDown(0)) select = true;
        if (Input.GetMouseButtonDown(1)) deselect = true;
        if (selectChess != null)
        {
            _current = new Vector2(selectChess.lastLocation.x, selectChess.lastLocation.y);
            _target =  new Vector2(selectChess.location.x, selectChess.location.y);
        }
        
        if (GameManager.model == GameModel.Single) UIController.Instance.pausePanel.GamePause();
        if (Chess.IsMoved) GameController.State = GameState.End;
    }
    
    public void End()
    {
        Count++;
        Timer.Instance.ResetTimer();
        Chess.IsMoved = false;
        MatchManager.Instance.Checkmate = -1;
        if (GameManager.model == GameModel.Single)
        {
            EventManager.CallOnCameraChanged();
        }
        else
        {
            photonView.RPC("SyncMove", RpcTarget.Others, _current, _target, MoveType);
            MoveType = "Default";
        }

        GameController.State = GameState.StandBy;
        EventManager.CallOnTurnEnd(); // checkmate检测
    }
    
    #endregion

    public static void ResetGame()
    {
        MatchManager.CurrentGrid = null;
        MatchManager.CurrentChess = null;
        MatchManager.Instance.Checkmate = -1;
    }

    public void OnceAgain() => photonView.RPC("Again", RpcTarget.All);

    public void GameOver()
    {
        Timer.Instance.StopTimer();
        UIController.Instance.SetStatusMessage("Over");
        Chess.IsMoved = false;
        IsOver = true;
        EventManager.CallOnGameOver(RoundType == Camp.White ? "White Win" : "Black Win");
    }

    public void DrawOver()
    {
        Timer.Instance.StopTimer();
        UIController.Instance.SetStatusMessage("Draw Over");
        Chess.IsMoved = false;
        GameController.State = GameState.Draw;
        EventManager.CallOnGameOver("Rival Offline");
    }

    public void GamePause() => UIController.Instance.pausePanel.GamePause();

    [PunRPC] public void SyncMove(Vector2 current, Vector2 target, string moveType)
    {
        var currentSelection = BoardGrid.GetSelection(new Vector2Int((int)current.x, (int)current.y));
        var targetSelection = BoardGrid.GetSelection(new Vector2Int((int)target.x, (int)target.y));
        
        Chess chess = currentSelection.GetPiece();
        chess.MovePiece(targetSelection.location);
        chess.location = targetSelection.location;

        currentSelection.occupyType = OccupyGridType.NoneOccupyGrid;
        currentSelection.chessList.Clear();
        currentSelection.chessPiece = null;
        
        targetSelection.occupyType = (OccupyGridType)chess.camp;
        targetSelection.chessList.Add(chess);
        targetSelection.chessPiece = chess;
        
        if (chess.GetComponent<Pawn>() != null)
        {
            chess.GetComponent<Pawn>().moveTurn = Count;
            chess.GetComponent<Pawn>().firstMoveStep = Mathf.Abs(targetSelection.location.y - currentSelection.location.y);
        }
       
        switch (moveType)
        {
            case "RookPromotion":
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                chess.GetComponent<Pawn>().PromotionLogic(chess.camp == Camp.White ? 
                    ChessType.WhiteRock : ChessType.BlackRock, true);
                break;
            case "KnightPromotion":
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                chess.GetComponent<Pawn>().PromotionLogic(chess.camp == Camp.White ? 
                    ChessType.WhiteKnight : ChessType.BlackKnight, true);
                break;
            case "BishopPromotion":
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                chess.GetComponent<Pawn>().PromotionLogic(chess.camp == Camp.White ? 
                    ChessType.WhiteBishop : ChessType.BlackBishop, true);
                break;
            case "QueenPromotion":
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                chess.GetComponent<Pawn>().PromotionLogic(chess.camp == Camp.White ? 
                        ChessType.WhiteQueen : ChessType.BlackQueen, true);
                break;
            case "PassBy":
                chess.GetComponent<Pawn>().En_Pass(targetSelection);
                break;
            case "LongCast": 
                chess.GetComponent<King>().Castling(true);
                break;
            case "ShortCast": 
                chess.GetComponent<King>().Castling(false);
                break;
            default:
                if (targetSelection.GetPiece() != null) chess.EatPiece(targetSelection);
                break;
        }
        Count++;
        Timer.Instance.ResetTimer();
    }
   
    [PunRPC] public void Again()
    {
        GameController.State = GameState.Init;
        PhotonNetwork.LoadLevel(2);
    }
}
