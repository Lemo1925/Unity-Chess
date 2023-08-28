using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject ChessBoard;
    
    public static GameModel model;
    public static Camp RoundType = Camp.WHITE;
    public static GameState state;
    

    private bool selectButtonListener, deselectButtonListener;
    
    private void OnEnable() => EventManager.OnGameResetEvent += ResetGame;

    private void OnDisable() => EventManager.OnGameResetEvent -= ResetGame;

    private void Start()
    {
        state = GameState.Init;
    }
    
    private void Update()
    {
        state = (GameState)((int)state % 3);
        switch (state)
        {
            case GameState.Init:
                GameStatus.instance.GameInit(ChessBoard);
                break;
            case GameState.StandBy:
                GameStatus.instance.StandBy();
                state++; break;
            case GameState.Action:
                GameStatus.instance.Action(ref selectButtonListener, ref deselectButtonListener);
                if (Chess.isMoved) state++;
                break;
            case GameState.End:
                GameStatus.instance.End();
                state++; break;
        }
    }

    private void FixedUpdate()
    {
        EventManager.CallOnSelectAction(selectButtonListener,deselectButtonListener);
        selectButtonListener = false;
        deselectButtonListener = false;
    }

    private void ResetGame()
    {
        MatchManager.Instance.currentSelection = null;
        MatchManager.Instance.currentChess = null;
        MatchManager.Instance.checkmate = -1;
        state = GameState.Init;
        GameStatus.instance.count = 0;
    }
}