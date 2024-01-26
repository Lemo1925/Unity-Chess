using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviourPunCallbacks
{
    [Header("结算面板")] 
    public Image panel;
    public Text text;
    public Button againButton;
    public Button menuButton;

    [Header("升变面板")]
    public Image promotionPanel;
    public List<Button> buttons;
    
    [Header("相机控制")] 
    public Button cameraButton;
    public Sprite whitePic;
    public Sprite blackPic;
    public Transform whiteTransform, blackTransform;
    
    [Header("准备面板")]
    public GameObject readyPanel;
    public Button readyButton; 
    
    public bool cameraFlag = true, pauseFlag = true;

    private Button _rook, _knight, _bishop, _queen;

    [Header("游戏状态")]
    public GameObject status;

    public static UIController Instance;
    private static Pawn PromotionChess { set; get; }

    public override void OnEnable()
    {
        EventManager.OnCameraChangedEvent += ChangeCameraPos;
        EventManager.OnPromotionEvent += SetPromotionPanel;
        EventManager.OnGameOverEvent += ShowPanel;
    }

    public override void OnDisable()
    {
        EventManager.OnCameraChangedEvent -= ChangeCameraPos;
        EventManager.OnPromotionEvent -= SetPromotionPanel;
        EventManager.OnGameOverEvent -= ShowPanel;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        promotionPanel.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
        
        _rook = buttons[0];
        _knight = buttons[1];
        _bishop = buttons[2];
        _queen = buttons[3];
    }  

    private void Start()
    {
        cameraButton.onClick.AddListener(ChangeCameraPos);
        
        againButton.onClick.AddListener(OnceAgain);
        menuButton.onClick.AddListener(BackToMenu);
        
        readyButton.onClick.AddListener(Ready);
        
        _rook.onClick.AddListener(RookPromotion);
        _knight.onClick.AddListener(KnightPromotion);
        _bishop.onClick.AddListener(BishopPromotion);
        _queen.onClick.AddListener(QueenPromotion);
        
        CameraTransition(whiteTransform);

        if (PhotonNetwork.IsConnected) readyPanel.SetActive(true);
        
    }

    public void GamePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseFlag)
            {
                ShowPanel("Game Pause");
                Timer.instance.StopTimer();
                GameController.State = GameState.Pause;
            }
            else
            {
                HidPanel();
                Timer.instance.GoAhead();
                GameController.State = GameState.Action;
            }
            pauseFlag = !pauseFlag;
        }
    }

    public void SetStatusMessage(string msg)
    {
        status.GetComponent<Text>().text = msg;
    }
   
    private void ShowPanel(string txt)
    {
        text.text = txt;
        panel.gameObject.SetActive(true);
    }

    private void HidPanel() => panel.gameObject.SetActive(false);

    private void OnceAgain()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(againButton));
        GameStatus.ResetGame();
        if (GameManager.model == GameModel.Multiple) GameStatus.Instance.OnceAgain();
        if (GameManager.model == GameModel.Single) ScenesManager.Instance.Translate("GameScene", "GameScene");
    }

    private void BackToMenu()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(menuButton));
        GameStatus.ResetGame();
        if (GameManager.model == GameModel.Multiple) PhotonNetwork.LoadLevel(1);
        if (GameManager.model == GameModel.Single) ScenesManager.Instance.Translate("GameScene", "UIScene");
    }


    #region 升变相关
    private void SetPromotionPanel(Pawn chess, bool visible)
    {
        PromotionChess = chess;
        promotionPanel.gameObject.SetActive(visible);
    }

    private void RookPromotion()
    {
        GameStatus.MoveType = "RookPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(_rook));
        PromotionChess.PromotionLogic(PromotionChess.camp == Camp.White ? ChessType.WhiteRock : ChessType.BlackRock);
        SetPromotionPanel(null,false);
    }

    private void KnightPromotion()
    {
        GameStatus.MoveType = "KnightPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(_knight));
        PromotionChess.PromotionLogic(PromotionChess.camp == Camp.White ? ChessType.WhiteKnight : ChessType.BlackKnight);
        SetPromotionPanel(null,false);
    }

    private void BishopPromotion()
    {
        GameStatus.MoveType = "BishopPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(_bishop));
        PromotionChess.PromotionLogic(PromotionChess.camp == Camp.White ? ChessType.WhiteBishop : ChessType.BlackBishop);
        SetPromotionPanel(null,false);
    }

    private void QueenPromotion()
    {
        GameStatus.MoveType = "QueenPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(_queen));
        PromotionChess.PromotionLogic(PromotionChess.camp == Camp.White ? ChessType.WhiteQueen : ChessType.BlackQueen);
        SetPromotionPanel(null,false);
    }
    #endregion

    private void Ready()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(readyButton));
        readyButton.GetComponentInChildren<Text>().text = "已准备";
        readyButton.enabled = false;
        readyButton.interactable = false;
    }

    public void UpdateUI() => readyPanel.GetComponentInChildren<Text>().text = $"准备玩家：{GameManager.ready}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    public void ReadyPanel() => readyPanel.SetActive(true);
    public void IsReady() => readyPanel.SetActive(false);

    #region Camera相关

    public void ChangeCameraPos()
    {
        var btnImg = cameraButton.GetComponent<Image>();
        btnImg.sprite = cameraFlag ? blackPic : whitePic;
        CameraTransition(cameraFlag ? blackTransform : whiteTransform);
        cameraFlag = !cameraFlag;
    }

    public void InitCameraFlag(Player player) => cameraFlag = player.camp != Camp.White;

    private static void CameraTransition(Transform target)
    {
        LeanTween.move(Camera.main!.gameObject, target.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotate(Camera.main!.gameObject, target.rotation.eulerAngles, 0.5f).setEase(LeanTweenType.easeInOutQuad);
    }

    #endregion
}
