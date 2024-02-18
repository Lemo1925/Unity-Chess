using Controller.Panel;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIController : SingletonMono<UIController>
{
    public ReadyPanel readyPanel;
    public PausePanel pausePanel;
    public OverPanel overPanel;
    public PromotionPanel promotionPanel;

    [Header("游戏状态")] 
    public GameObject status;

    public void OnEnable()
    {
        EventManager.OnPromotionEvent += promotionPanel.Show;
        EventManager.OnGameOverEvent += overPanel.Show;
    }

    public void OnDisable()
    {
        EventManager.OnPromotionEvent -= promotionPanel.Show;
        EventManager.OnGameOverEvent -= overPanel.Show;
    }
    
    public void SetStatusMessage(string msg) => 
        status.GetComponent<Text>().text = msg;
}
