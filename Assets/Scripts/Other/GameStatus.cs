using Photon.Pun;
using UnityEngine;

public class GameStatus : MonoBehaviourPun
{
    public static GameStatus instance;
    public GameObject chessboard;
    public static Camp RoundType;

    public static int count;
    public static bool isOver, isPromotion;
    public static string moveType;

    public Chess selectChess;
    private Vector2 current, target;

    private void OnEnable()
    {
        EventManager.OnGameAgainEvent += AgainGame;
        EventManager.OnBackToMenuEvent += BackMenu;
    }

    private void OnDisable()
    {
        EventManager.OnGameAgainEvent -= AgainGame;
        EventManager.OnBackToMenuEvent += BackMenu;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        moveType = "Default";
        RoundType = Camp.WHITE;
        isPromotion = false;
        isOver = false;
        count = 0;
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
        if (isOver)
        {
            GameController.state = GameState.Over;
            return; 
        }

        RoundType = (Camp)(count % 2);
        UIController.Instance.SetStatusMessage(RoundType.ToString());

        if (GameManager.model == GameModel.MULTIPLE)
        {
            while (RoundType == Player.instance.camp)
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
        
        if (GameManager.model == GameModel.SINGLE) UIController.Instance.GamePause();
        if (Chess.isMoved) GameController.state = GameState.End;
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
        }
        else
        {
            photonView.RPC("SyncMove", RpcTarget.Others, current, target, moveType);
            moveType = "Default";
        }

        GameController.state = GameState.StandBy;
        EventManager.CallOnTurnEnd(); // checkmate检测
    }
    #endregion

    private void ResetGame()
    {
        MatchManager.currentSelection = null;
        MatchManager.currentChess = null;
        MatchManager.Instance.checkmate = -1;
    }

    private void AgainGame()
    {
        ResetGame();
        if (GameManager.model == GameModel.MULTIPLE) photonView.RPC("Again", RpcTarget.All);
        if (GameManager.model == GameModel.SINGLE) ScenesManager.instance.Translate("Scenes/GameScene", "Scenes/GameScene");
    }

    public void GameOver()
    {
        Timer.instance.StopTimer();
        UIController.Instance.SetStatusMessage("Over");
        Chess.isMoved = false;
        isOver = true;
        EventManager.CallOnGameOver(RoundType == Camp.WHITE ? "White Win" : "Black Win");
    }

    public void DrawOver()
    {
        Timer.instance.StopTimer();
        UIController.Instance.SetStatusMessage("Draw Over");
        Chess.isMoved = false;
        GameController.state = GameState.Draw;
        EventManager.CallOnGameOver("Rival Offline");
    }

    public void GamePause() => UIController.Instance.GamePause();

    private void BackMenu()
    {
        ResetGame();
        if (GameManager.model == GameModel.MULTIPLE) PhotonNetwork.LoadLevel(1);
        if (GameManager.model == GameModel.SINGLE) ScenesManager.instance.Translate("Scenes/GameScene", "Scenes/UIScene");
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

    [PunRPC] public void Again()
    {
        GameController.state = GameState.Init;
        PhotonNetwork.LoadLevel(2);
    }
}
