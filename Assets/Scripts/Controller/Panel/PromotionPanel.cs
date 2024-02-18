using UnityEngine;
using UnityEngine.UI;

namespace Controller.Panel
{
    public class PromotionPanel:Panel
    {
        [SerializeField]private Button rook;
        [SerializeField]private Button knight;
        [SerializeField]private Button bishop;
        [SerializeField]private Button queen;
        private static Pawn PromotionChess { set; get; }

        public void Show(Pawn chess)
        {
            PromotionChess = chess;
            Show();
        }

        public void RookPromotion()
        {
            GameStatus.MoveType = "RookPromotion";
            StartCoroutine(EffectTool.Instance.ScaleAnimation(rook));
            PromotionChess.PromotionLogic(PromotionChess.camp == Camp.White ? ChessType.WhiteRook : ChessType.BlackRook);
            Hid();
        }

        public void KnightPromotion()
        {
            GameStatus.MoveType = "KnightPromotion";
            StartCoroutine(EffectTool.Instance.ScaleAnimation(knight));
            PromotionChess.PromotionLogic(PromotionChess.camp == Camp.White ? ChessType.WhiteKnight : ChessType.BlackKnight);
            Hid();
        }

        public void BishopPromotion()
        {
            GameStatus.MoveType = "BishopPromotion";
            StartCoroutine(EffectTool.Instance.ScaleAnimation(bishop));
            PromotionChess.PromotionLogic(PromotionChess.camp == Camp.White ? ChessType.WhiteBishop : ChessType.BlackBishop);
            Hid();
        }

        public void QueenPromotion()
        {
            GameStatus.MoveType = "QueenPromotion";
            StartCoroutine(EffectTool.Instance.ScaleAnimation(queen));
            PromotionChess.PromotionLogic(PromotionChess.camp == Camp.White ? ChessType.WhiteQueen : ChessType.BlackQueen);
            Hid();
        }
    }
}