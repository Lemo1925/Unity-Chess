using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("主要物件")] public GameObject ChessBoard;
    
    [Header("游戏模式")] public static GameModel model = GameModel.SINGLE;

    [Header("玩家棋子")] public static Camp RoundType = Camp.WHITE;

    [Header("游戏回合")] public static int count;

    private bool selectButtonListener, deselectButtonListener;
    private void OnEnable() => Instantiate(ChessBoard, transform.localPosition, Quaternion.identity);

    private void Update()
    {
        RoundType = (Camp)(count % 2);
        
        #region 开始阶段
        
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

    private void FixedUpdate()
    {
        EventManager.CallOnSelectAction(selectButtonListener,deselectButtonListener);
        selectButtonListener = false;
        deselectButtonListener = false;
    }
}