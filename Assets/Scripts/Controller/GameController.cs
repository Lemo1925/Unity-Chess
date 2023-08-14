using System;
using UnityEngine;

namespace Controller
{
    public class GameController : MonoBehaviour
    {
        [Header("主要物件")] public GameObject ChessBoard;
        
        [Header("游戏模式")] public static GameModel model = GameModel.SINGLE;

        [Header("玩家棋子")] public static Camp RoundType = Camp.WHITE;

        [Header("游戏回合")] 
        public int count;
        
        private void OnEnable()
        {
            Instantiate(ChessBoard, transform.localPosition, Quaternion.identity);
        }

        private void Update()
        {
            RoundType = (Camp)(count % 2);
            EventManager.CallOnSelectTurn();

        }

        private void FixedUpdate()
        {
        }
    }
}