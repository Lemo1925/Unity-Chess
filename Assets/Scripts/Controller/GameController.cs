using UnityEngine;

public class GameController : MonoBehaviour
{    
    public static GameState State;
    
    private bool _selectButtonListener, _deselectButtonListener;

    private void Awake() => State = GameState.Init;

    private void Update()
    {
        switch (State)
        {
            case GameState.Init:
                GameStatus.Instance.GameInit();
                break;
            case GameState.StandBy:
                GameStatus.Instance.StandBy();
                break;
            case GameState.Action:
                GameStatus.Instance.Action(ref _selectButtonListener, ref _deselectButtonListener);
                break;
            case GameState.End:
                GameStatus.Instance.End();
                break;
            case GameState.Over:
                GameStatus.Instance.GameOver();
                break;
            case GameState.Draw:
                break;
            case GameState.Pause:
                GameStatus.Instance.GamePause();
                break;
        }
    }

    private void FixedUpdate()
    {
        EventManager.CallOnSelectAction(_selectButtonListener,_deselectButtonListener);
        _selectButtonListener = false;
        _deselectButtonListener = false;
    }
}