using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image promotionPanel;
    public List<Button> buttons;
    private Button Rook, Knight, Bishop, Queen;

    private static Pawn promotionChess { set; get; }
    
    [Header("按钮特效")]
    public float scaleAmount = 0.8f;
    public float animationDuration = 0.2f;
    
    private void OnEnable() => EventManager.OnPromotionEvent += setPromotionPanel;
    private void OnDisable() => EventManager.OnPromotionEvent -= setPromotionPanel;

    private void Awake()
    {
        Rook = buttons[0];
        Knight = buttons[1];
        Bishop = buttons[2];
        Queen = buttons[3];
        
        promotionPanel.gameObject.SetActive(false);
    }

    private void Start()
    {
        Rook.onClick.AddListener(RookPromotion);
        Knight.onClick.AddListener(KnightPromotion);
        Bishop.onClick.AddListener(BishopPromotion);
        Queen.onClick.AddListener(QueenPromotion);
    }

    private IEnumerator ScaleAnimation(Component button)
    {
        Vector3 originalScale = button.transform.localScale;
        // 缩小按钮
        LeanTween.scale(button.gameObject, originalScale * scaleAmount, animationDuration);
        // 等待一段时间
        yield return new WaitForSeconds(animationDuration);
        // 恢复按钮原始大小
        LeanTween.scale(button.gameObject, originalScale, animationDuration);
    }

    private void setPromotionPanel(Pawn chess, bool visible)
    {
        promotionChess = chess;
        promotionPanel.gameObject.SetActive(visible);
    }

    private void RookPromotion()
    {
        StartCoroutine(ScaleAnimation(Rook));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteRock : ChessType.BlackRock);
        setPromotionPanel(null,false);
    }

    private void KnightPromotion()
    {
        StartCoroutine(ScaleAnimation(Knight));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteKnight : ChessType.BlackKnight);
        setPromotionPanel(null,false);
    }

    private void BishopPromotion()
    {
        StartCoroutine(ScaleAnimation(Bishop));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteBishop : ChessType.BlackBishop);
        setPromotionPanel(null,false);
    }

    private void QueenPromotion()
    {
        StartCoroutine(ScaleAnimation(Queen));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteQueen : ChessType.BlackQueen);
        setPromotionPanel(null,false);
    } 
}
