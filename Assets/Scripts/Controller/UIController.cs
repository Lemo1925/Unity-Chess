﻿using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class UIController : MonoBehaviourPunCallbacks
{
    [Header("结算面板")] 
    public Image GameOverPanel;
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
    public GameObject readyPanel;
    public Button ReadyButton; 
    
    public bool cameraFlag = true;

    private Button Rook, Knight, Bishop, Queen;

    [Header("计时器")] 
    public GameObject Timer;

    public static UIController Instance;
    private static Pawn promotionChess { set; get; }

    public override void OnEnable()
    {
        EventManager.OnCameraChangedEvent += ChangeCameraPos;
        EventManager.OnPromotionEvent += setPromotionPanel;
        EventManager.OnGameOverEvent += IsGameOver;
    }

    public override void OnDisable()
    {
        EventManager.OnCameraChangedEvent -= ChangeCameraPos;
        EventManager.OnPromotionEvent -= setPromotionPanel;
        EventManager.OnGameOverEvent -= IsGameOver;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        promotionPanel.gameObject.SetActive(false);
        GameOverPanel.gameObject.SetActive(false);
        
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
        
        CameraTransition(WhiteTransform);

        if (PhotonNetwork.IsConnected)
           readyPanel.SetActive(true);
        
    }
    
    #region 升变相关
    private void setPromotionPanel(Pawn chess, bool visible)
    {
        promotionChess = chess;
        promotionPanel.gameObject.SetActive(visible);
    }

    private void RookPromotion()
    {
        GameStatus.moveType = "RookPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Rook));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteRock : ChessType.BlackRock);
        setPromotionPanel(null,false);
    }

    private void KnightPromotion()
    {
        GameStatus.moveType = "KnightPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Knight));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteKnight : ChessType.BlackKnight);
        setPromotionPanel(null,false);
    }

    private void BishopPromotion()
    {
        GameStatus.moveType = "BishopPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Bishop));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteBishop : ChessType.BlackBishop);
        setPromotionPanel(null,false);
    }

    private void QueenPromotion()
    {
        GameStatus.moveType = "QueenPromotion";
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Queen));
        promotionChess.PromotionLogic(promotionChess.camp == Camp.WHITE ? ChessType.WhiteQueen : ChessType.BlackQueen);
        setPromotionPanel(null,false);
    }
    #endregion
    
    private void IsGameOver(string text)
    {
        winText.text = text;
        GameOverPanel.gameObject.SetActive(true);
    }

    public void UpdateUI()
    {
        readyPanel.GetComponentInChildren<Text>().text = $"准备玩家：{GameManager.ready}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    private void OnceAgain()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(AgainButton));
        EventManager.CallOnGameAgain();
    }

    private void BackToMenu()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(MenuButton));
        EventManager.CallOnBackToMenu();
    }

    private void Ready()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(ReadyButton));
        ReadyButton.GetComponentInChildren<Text>().text = "已准备";
        ReadyButton.enabled = false;
        ReadyButton.interactable = false;
    }

    public void ReadyPanel() => readyPanel.SetActive(true);

    public void IsReady() => readyPanel.SetActive(false);

    public void ChangeCameraPos()
    {
        var BtnImg = cameraButton.GetComponent<Image>();
        BtnImg.sprite = cameraFlag ? BlackPic : WhitePic;
        CameraTransition(cameraFlag ? BlackTransform : WhiteTransform);
        cameraFlag = !cameraFlag;
    }

    public void InitCameraFlag(Player player) => cameraFlag = player.camp != Camp.WHITE;

    private static void CameraTransition(Transform target)
    {
        Debug.Assert(Camera.main != null, "Camera.main != null");
        LeanTween.move(Camera.main.gameObject, target.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotate(Camera.main.gameObject, target.rotation.eulerAngles, 0.5f).setEase(LeanTweenType.easeInOutQuad);
    }
}
