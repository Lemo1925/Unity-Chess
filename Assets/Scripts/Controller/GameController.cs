using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject chessboard;
    
    public static Camp RoundType;
    public static GameState state;
    
    private bool selectButtonListener, deselectButtonListener;
    
    private void OnEnable() => EventManager.OnGameResetEvent += ResetGame;

    private void OnDisable() => EventManager.OnGameResetEvent -= ResetGame;

    private void Start()
    {
        print("Init");
        state = GameState.Init;
        RoundType = Camp.WHITE;
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.Init:
                print("Init"+GameStatus.instance); 
                GameStatus.instance.GameInit(chessboard);
                break;
            case GameState.StandBy:
                print("StandBy"+GameStatus.instance);
                GameStatus.instance.StandBy();
                break;
            case GameState.Action:
                print("Action"+GameStatus.instance);
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
        MatchManager.Instance.currentSelection = null;
        MatchManager.Instance.currentChess = null;
        MatchManager.Instance.checkmate = -1;
    }

    private void OnDestroy()
    {
        GameStatus.instance = null;
    }
}