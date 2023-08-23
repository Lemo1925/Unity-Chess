using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("主要物件")] public GameObject ChessBoard;
    
    [Header("游戏模式")] public static GameModel model = GameModel.SINGLE;

    [Header("玩家棋子")] public static Camp RoundType = Camp.WHITE;

    [Header("游戏回合")] public static int count;

    [Header("玩家")] public static Player master, slave;
    
    private bool selectButtonListener, deselectButtonListener;
    private void OnEnable() => Instantiate(ChessBoard, transform.localPosition, Quaternion.identity);

    private void Update()
    {
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
            // var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // while (true)
            // {
            //     // Accept
            //     var connect = socket.Accept();
            //     // Recv
            //     var readBuff = new byte[1024];
            //     int count = connect.Receive(readBuff);
            //     string s = System.Text.Encoding.UTF8.GetString(readBuff,0,count); 
            //     // Send
            //     byte[] bytes = System.Text.Encoding.Default.GetBytes(s);
            //     connect.Send(bytes);
            // }
        }
    }

    private void FixedUpdate()
    {
        EventManager.CallOnSelectAction(selectButtonListener,deselectButtonListener);
        selectButtonListener = false;
        deselectButtonListener = false;
    }
}