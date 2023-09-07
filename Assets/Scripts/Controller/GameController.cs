using UnityEngine;

public class GameController : MonoBehaviour
{    
    public static Camp RoundType;
    public static GameState state;
    
    private bool selectButtonListener, deselectButtonListener;
   
    private void Awake()
    {
        state = GameState.Init;
        RoundType = Camp.WHITE;
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.Init:
                GameStatus.instance.GameInit();
                break;
            case GameState.StandBy:
                GameStatus.instance.StandBy();
                break;
            case GameState.Action:
                GameStatus.instance.Action(
                    ref selectButtonListener, 
                    ref deselectButtonListener);
                break;
            case GameState.End:
                GameStatus.instance.End();
                break;
        }
    }

    private void FixedUpdate()
    {
        EventManager.CallOnSelectAction(
            selectButtonListener,
            deselectButtonListener);
        selectButtonListener = false;
        deselectButtonListener = false;
    }

    private void OnDestroy()
    {
        UIController.Instance = null;
        GameStatus.instance = null;
    }
}