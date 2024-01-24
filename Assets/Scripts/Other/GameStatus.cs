﻿using Photon.Pun;
using UnityEngine;

public class GameStatus : MonoBehaviourPun
{
    public static GameStatus Instance;
    public GameObject chessboard;
    public static Camp RoundType;

    public static int Count;
    public static bool IsOver, IsPromotion;
    public static string MoveType;

    public Chess selectChess;
    private Vector2 _current, _target;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        MoveType = "Default";
        RoundType = Camp.WHITE;
        IsPromotion = false;
        IsOver = false;
        Count = 0;
    }

    #region Game Loop
    public void GameInit()
    {
        if (GameManager.model == GameModel.MULTIPLE)
        {
            UIController.Instance.ReadyPanel();
            UIController.Instance.SetStatusMessage("Wait");
            if (GameManager.ready == 2)
            {
                UIController.Instance.IsReady();
                GameManager.ready = 0;
                Instantiate(chessboard, transform.localPosition, Quaternion.identity, transform);
                UIController.Instance.InitCameraFlag(GameManager.GetPlayer());
                UIController.Instance.ChangeCameraPos();
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

        if (GameManager.model == GameModel.MULTIPLE)
        {
            while (RoundType == Player.instance.camp)
            {
                GameController.State = GameState.Action;
                break;
            }

        }
        else
        {
            GameController.State = GameState.Action;
        }
        Timer.instance.StartTimer(180.0f);
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
        
        if (GameManager.model == GameModel.SINGLE) UIController.Instance.GamePause();
        if (Chess.IsMoved) GameController.State = GameState.End;
    }
    
    public void End()
    {
        Count++;
        Timer.instance.ResetTimer();
        Chess.IsMoved = false;
        MatchManager.Instance.Checkmate = -1;
        if (GameManager.model == GameModel.SINGLE)
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

    public void OnceAgain() => 
        photonView.RPC("Again", RpcTarget.All);

    public void GameOver()
    {
        Timer.instance.StopTimer();
        UIController.Instance.SetStatusMessage("Over");
        Chess.IsMoved = false;
        IsOver = true;
        EventManager.CallOnGameOver(RoundType == Camp.WHITE ? "White Win" : "Black Win");
    }

    public void DrawOver()
    {
        Timer.instance.StopTimer();
        UIController.Instance.SetStatusMessage("Draw Over");
        Chess.IsMoved = false;
        GameController.State = GameState.Draw;
        EventManager.CallOnGameOver("Rival Offline");
    }

    public void GamePause() => UIController.Instance.GamePause();

    [PunRPC] public void SyncMove(Vector2 current, Vector2 target, string moveType)
    {
        var currentSelection = Grid.GetSelection(new Vector2Int((int)current.x, (int)current.y));
        var targetSelection = Grid.GetSelection(new Vector2Int((int)target.x, (int)target.y));
        
        Chess chess = currentSelection.GetPiece();
        chess.MovePiece(targetSelection.location);
        chess.location = targetSelection.location;

        currentSelection.occupyType = Grid.OccupyGridType.NoneOccupyGrid;
        currentSelection.chessList.Clear();
        currentSelection.chessPiece = null;
        
        targetSelection.occupyType = (Grid.OccupyGridType)chess.camp;
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
        Count++;
        Timer.instance.ResetTimer();
    }
   
    [PunRPC] public void Again()
    {
        GameController.State = GameState.Init;
        PhotonNetwork.LoadLevel(2);
    }
}
