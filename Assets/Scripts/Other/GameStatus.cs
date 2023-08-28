using Photon.Pun;
using UnityEngine;

public class GameStatus : MonoBehaviourPun
{
    public static GameStatus instance;
    public bool isOver;
    public int count;

    
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
            print(GameManager.ready);
            if (GameManager.ready == 2)
            {
                UIController.Instance.isReady();
                Instantiate(chessboard, transform.localPosition, Quaternion.identity, transform);
                UIController.CameraTransition(GameManager.player.transform);
                GameController.state++;
            }    
        }
        else
        {
            Instantiate(chessboard, transform.localPosition, Quaternion.identity, transform);
            GameController.state++;
        }
    }

    public void StandBy()
    {
        if (isOver) return;
        GameController.RoundType = (Camp)(count % 2);
    }

    public void Action(ref bool select, ref bool deselect)
    {
        if (Input.GetMouseButtonDown(0)) select = true;
        if (Input.GetMouseButtonDown(1)) deselect = true;
    }
    
    public void End()
    {
        count++; 
        Chess.isMoved = false; 
        MatchManager.Instance.checkmate = -1; 
        EventManager.CallOnTurnEnd();
        EventManager.CallOnCameraChanged();
    }
}
