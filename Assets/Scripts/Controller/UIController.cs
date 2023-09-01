using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class UIController : MonoBehaviourPunCallbacks
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
    public Button cameraButton;
    public Sprite WhitePic;
    public Sprite BlackPic;
    public Transform WhiteTransform, BlackTransform;
    
    [Header("准备面板")]
    public GameObject waitPanel;
    public Button ReadyButton; 
    
    public bool cameraFlag = true;

    private Button Rook, Knight, Bishop, Queen;


    public static UIController Instance;
    
    
    
    private static Pawn promotionChess { set; get; }


    public override void OnEnable()
    {
        EventManager.OnCameraChangedEvent += ChangeCameraPos;
        EventManager.OnPromotionEvent += setPromotionPanel;
        EventManager.OnGameOverEvent += WinUp;
    }

    public override void OnDisable()
    {
        EventManager.OnCameraChangedEvent -= ChangeCameraPos;
        EventManager.OnPromotionEvent -= setPromotionPanel;
        EventManager.OnGameOverEvent -= WinUp;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;

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
        
        ReadyButton.onClick.AddListener(Ready);
        
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

        if (PhotonNetwork.IsConnected)
        {
            waitPanel.SetActive(true);
        }
    }

   

    private void setPromotionPanel(Pawn chess, bool visible)
    {
        promotionChess = chess;
        promotionPanel.gameObject.SetActive(visible);
    }

    private void RookPromotion()
    {
        GameStatus.instance.moveType = "RookPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Rook));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteRock : ChessType.BlackRock);
        setPromotionPanel(null,false);
    }

    private void KnightPromotion()
    {
        GameStatus.instance.moveType = "KnightPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Knight));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteKnight : ChessType.BlackKnight);
        setPromotionPanel(null,false);
    }

    private void BishopPromotion()
    {
        GameStatus.instance.moveType = "BishopPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Bishop));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteBishop : ChessType.BlackBishop);
        setPromotionPanel(null,false);
    }

    private void QueenPromotion()
    {
        GameStatus.instance.moveType = "QueenPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Queen));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteQueen : ChessType.BlackQueen);
        setPromotionPanel(null,false);
    }

    private void WinUp(string text)
    {
        winText.text = text;
        WinUpPanel.gameObject.SetActive(true);
        GameStatus.instance.isOver = true;
    }

    public void UpdateUI()
    {
        waitPanel.GetComponentInChildren<Text>().text = $"准备玩家：{GameManager.ready}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

    }
    
    private void OnceAgain()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(AgainButton));
        EventManager.CallOnGameReset();
        ScenesManager.instance.Translate("Scenes/GameScene", "Scenes/GameScene");
    }

    private void BackToMenu()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(MenuButton));
        EventManager.CallOnGameReset();
        ScenesManager.instance.Translate("Scenes/GameScene", "Scenes/UIScene");
    }

    private void Ready()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(ReadyButton));
        ReadyButton.GetComponentInChildren<Text>().text = "已准备";
        ReadyButton.enabled = false;
        ReadyButton.interactable = false;
    }

    public void isReady() => waitPanel.SetActive(false);

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
    
    public static void CameraTransition(Transform target)
    {
        Debug.Assert(Camera.main != null, "Camera.main != null");
        LeanTween.move(Camera.main.gameObject, target.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotate(Camera.main.gameObject, target.rotation.eulerAngles, 0.5f).setEase(LeanTweenType.easeInOutQuad);
    }
}
