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

    private void setPromotionPanel(bool visible) => promotionPanel.gameObject.SetActive(visible);

    private void RookPromotion()
    {
        StartCoroutine(ScaleAnimation(Rook));
        MatchManager.Instance.promotionType =
            GameController.RoundType == Camp.WHITE ? ChessType.WhiteRock : ChessType.BlackRock;
    }

    private void KnightPromotion()
    {
        StartCoroutine(ScaleAnimation(Knight));
        MatchManager.Instance.promotionType =
            GameController.RoundType == Camp.WHITE ? ChessType.WhiteKnight : ChessType.BlackKnight;
    }

    private void BishopPromotion()
    {
        StartCoroutine(ScaleAnimation(Bishop));
        MatchManager.Instance.promotionType =
            GameController.RoundType == Camp.WHITE ? ChessType.WhiteBishop : ChessType.BlackBishop;
    }

    private void QueenPromotion()
    {
        StartCoroutine(ScaleAnimation(Queen));
        MatchManager.Instance.promotionType =
            GameController.RoundType == Camp.WHITE ? ChessType.WhiteQueen : ChessType.BlackQueen;
    }
    
    private IEnumerator ScaleAnimation(Button button)
    {
        Vector3 originalScale = button.transform.localScale;
        // 缩小按钮
        LeanTween.scale(button.gameObject, originalScale * scaleAmount, animationDuration);
        // 等待一段时间
        yield return new WaitForSeconds(animationDuration);
        // 恢复按钮原始大小
        LeanTween.scale(button.gameObject, originalScale, animationDuration);
    }
}
