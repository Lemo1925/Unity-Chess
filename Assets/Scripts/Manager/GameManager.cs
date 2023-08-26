using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject ChessBoard;
    public GameObject waitPanel;
    public Button readyButton;
    public int ready; 
    private void Start()
    {
        print("Start");
        if (GameController.model == GameModel.MULTIPLE)
        {
            Time.timeScale = 0;
            waitPanel.SetActive(true);
        }
        Instantiate(ChessBoard, transform.localPosition, Quaternion.identity, transform);
    }

    public void OnReadyButtonClick()
    {
        print("ready");
        StartCoroutine(EffectTool.Instance.ScaleAnimation(readyButton));
        readyButton.GetComponentInChildren<Text>().text = "已准备";
        readyButton.enabled = false;
        readyButton.interactable = false;
        ready++;
    }
    
    private void Update()
    {
        if (GameController.model == GameModel.MULTIPLE )
        {
            if (ready > 1)
            {
                waitPanel.SetActive(false);
                // todo ready
                // Time.timeScale = 1;
            }
            else
            {
                waitPanel.GetComponentInChildren<Text>().text = $"准备玩家：{ready}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
            }
        }
    }
}