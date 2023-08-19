using UnityEngine;

namespace Controller
{
    public class GameController : MonoBehaviour
    {
        [Header("主要物件")] public GameObject ChessBoard;
        
        [Header("游戏模式")] public static GameModel model = GameModel.SINGLE;

        [Header("玩家棋子")] public static Camp RoundType = Camp.WHITE;

        [Header("游戏回合")] public const int count = 0;

        private bool selectButtonListener, deselectButtonListener;
        private void OnEnable()
        {
            Instantiate(ChessBoard, transform.localPosition, Quaternion.identity);
        }

        private void Update()
        {
            RoundType = count % 2;

            #region 执棋阶段
            
            if (Input.GetMouseButtonDown(0)) 
                selectButtonListener = true;

            if (Input.GetMouseButtonDown(1)) 
                deselectButtonListener = true;

            #endregion
        }

        private void FixedUpdate()
        {
            EventManager.CallOnSelectAction(selectButtonListener,deselectButtonListener);
            selectButtonListener = false;
            deselectButtonListener = false;
        }
    }
}