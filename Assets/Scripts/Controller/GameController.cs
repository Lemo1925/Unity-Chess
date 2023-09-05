using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject chessboard;
    
    public static GameModel model;
    public static Camp RoundType = Camp.WHITE;
    public static GameState state;
    

    private bool selectButtonListener, deselectButtonListener;
    
    private void OnEnable() => EventManager.OnGameResetEvent += ResetGame;

    private void OnDisable() => EventManager.OnGameResetEvent -= ResetGame;

    private void Start() => state = GameState.Init;

    private void Update()
    {
        switch (state)
        {
            case GameState.Init:
                GameStatus.instance.GameInit(chessboard);
                break;
            case GameState.StandBy:
                GameStatus.instance.StandBy();
                break;
            case GameState.Action:
                GameStatus.instance.Action(ref selectButtonListener, ref deselectButtonListener);
                break;
            case GameState.End:
                GameStatus.instance.End();
                break;
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
        // GameStatus.instance = null;
        state = GameState.Init;
        MatchManager.Instance.currentSelection = null;
        MatchManager.Instance.currentChess = null;
        MatchManager.Instance.checkmate = -1;

    }
}