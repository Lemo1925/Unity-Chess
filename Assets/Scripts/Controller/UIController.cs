using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("结算面板")] 
    public Image WinUpPanel;
    public Text winText;
    public Button AgainButton;
    public Button MenuButton;

    [Header("升变面板")]
    public Image promotionPanel;
    public List<Button> buttons;
    
    [Header("相机控制")] 
    public new GameObject camera;
    public Button cameraButton;
    public Sprite WhitePic;
    public Sprite BlackPic;
    public Transform WhiteTransform, BlackTransform;

    private bool cameraFlag = true;

    private Button Rook, Knight, Bishop, Queen;
    

    private static Pawn promotionChess { set; get; }
    
    [Header("按钮特效")]
    public float scaleAmount = 0.8f;
    public float animationDuration = 0.2f;
    
    private void OnEnable()
    {
        EventManager.OnTurnEndEvent += ChangeCameraPos;
        EventManager.OnPromotionEvent += setPromotionPanel;
        EventManager.OnGameOverEvent += WinUp;
    }

    private void OnDisable()
    {
        EventManager.OnTurnEndEvent -= ChangeCameraPos;
        EventManager.OnPromotionEvent -= setPromotionPanel;
        EventManager.OnGameOverEvent -= WinUp;
    }

    private void Awake()
    {
        Scene targetScene = SceneManager.GetSceneByBuildIndex(0);
        foreach (var rootGameObject in targetScene.GetRootGameObjects())
        {
            if (rootGameObject.CompareTag("MainCamera"))
            {
                camera = rootGameObject;
            }
        }

        promotionPanel.gameObject.SetActive(false);
        WinUpPanel.gameObject.SetActive(false);
        
        Rook = buttons[0];
        Knight = buttons[1];
        Bishop = buttons[2];
        Queen = buttons[3];
    }  

    private void Start()
    {
        cameraButton.onClick.AddListener(ChangeCameraPos);
        
        AgainButton.onClick.AddListener(OnceAgain);
        MenuButton.onClick.AddListener(BackToMenu);
        
        Rook.onClick.AddListener(RookPromotion);
        Knight.onClick.AddListener(KnightPromotion);
        Bishop.onClick.AddListener(BishopPromotion);
        Queen.onClick.AddListener(QueenPromotion);
        
        if (GameController.RoundType == Camp.WHITE)
        {
            cameraFlag = true;
            CameraTransition(WhiteTransform);
        }
        else
        {
            cameraFlag = false;
            CameraTransition(BlackTransform);
        }
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

    private void WinUp(string text)
    {
        winText.text = text;
        WinUpPanel.gameObject.SetActive(true);
        GameController.isOver = true;
    }

    // TODO: Reset The Game 
    private void OnceAgain()
    {
        StartCoroutine(ScaleAnimation(AgainButton));
        ScenesManager.instance.Translate("Scenes/GameScene", "Scenes/GameScene");
    }

    private void BackToMenu()
    {
        StartCoroutine(ScaleAnimation(MenuButton));
        EventManager.CallOnGameReset();
        ScenesManager.instance.Translate("Scenes/GameScene", "Scenes/UIScene");
    }
    
    private void ChangeCameraPos()
    {
        var BtnImg = cameraButton.GetComponent<Image>();
        if (cameraFlag)
        {
            BtnImg.sprite = BlackPic;
            CameraTransition(BlackTransform);
        }
        else
        {
            BtnImg.sprite = WhitePic;
            CameraTransition(WhiteTransform);
        }
        cameraFlag = !cameraFlag;
    }
    
    private void CameraTransition(Transform target)
    {
        LeanTween.move(camera, target.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotate(camera, target.rotation.eulerAngles, 0.5f).setEase(LeanTweenType.easeInOutQuad);
    }
}
