using Photon.Pun;
using UnityEngine;

public class GameStatus : MonoBehaviourPun
{
    public static GameStatus instance;
    public bool isOver, sync;
    public int count;
    public Chess selectChess;
    public Vector2 current, target;
    public string movetype;
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        isOver = false;
        count = 0;
    }

    public void GameInit(GameObject chessboard)
    {
        if (GameController.model == GameModel.MULTIPLE)
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
        if (GameController.model == GameModel.MULTIPLE)
        {
            while (GameController.RoundType == Player.instance.camp)
            {
                GameController.state = GameState.Action;
                break;
            }
        }
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
        Chess.isMoved = false;
        MatchManager.Instance.checkmate = -1;

        if (GameController.model == GameModel.SINGLE)
        {
            EventManager.CallOnCameraChanged();
            GameController.state = GameState.StandBy;
        }
        else
        {
            photonView.RPC("SyncMove", RpcTarget.Others, current, target);
            GameController.state = GameState.StandBy;
        }
        
        // checkmate检测
        EventManager.CallOnTurnEnd();
    }
    
    [PunRPC] public void SyncMove(Vector2 current, Vector2 target)
    {
        var currentSelection = Selection.GetSelection(new Vector2Int((int)current.x, (int)current.y));
        var targetSelection = Selection.GetSelection(new Vector2Int((int)target.x, (int)target.y));
        
        Chess chess = currentSelection.GetPiece();
        chess.MovePiece(targetSelection.Location);
        chess.Location = targetSelection.Location;
        
        
        count++;
    }
}
