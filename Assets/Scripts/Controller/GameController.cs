using Photon.Pun;
using UnityEngine;

public class GameController : MonoBehaviourPun
{
    
    [Header("游戏模式")] 
    public static GameModel model;
    public static bool isOver;

    [Header("玩家棋子")] public static Camp RoundType = Camp.WHITE;

    [Header("游戏回合")] public static int count;

    
    private bool selectButtonListener, deselectButtonListener;

    private void OnEnable() => EventManager.OnGameResetEvent += ResetGame;

    private void OnDisable() => EventManager.OnGameResetEvent -= ResetGame;

    private void Awake()
    {
        isOver = false;
        count = 0;
    }
    
    private void Update()
    {
        if (isOver) return;
        if (model == GameModel.SINGLE)
        {
            #region 开始阶段

            RoundType = (Camp)(count % 2);

            #endregion

            #region 移动阶段

            if (Input.GetMouseButtonDown(0)) selectButtonListener = true;
            if (Input.GetMouseButtonDown(1)) deselectButtonListener = true;

            #endregion

            #region 结束阶段

            if (Chess.isMoved)
            {
                count++;
                Chess.isMoved = false;
                MatchManager.Instance.checkmate = -1;
                EventManager.CallOnTurnEnd();
            }

            #endregion
        }
        else
        {
            // TODO : 多人模块   
            
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
        count = 0;
    }
}