using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Panel
{
    public class ReadyPanel : Panel
    {
        [SerializeField]private Text text;
        [SerializeField]private Button readyButton;

        private void Start()
        {
            if (PhotonNetwork.IsConnected) Show();
        }
        
        public void UpdateUI() => text.text = $"准备玩家：{GameManager.ready}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        
        public void Ready()
        {
            StartCoroutine(EffectTool.Instance.ScaleAnimation(readyButton));
            text.text = "已准备";
            readyButton.enabled = false;
            readyButton.interactable = false;
        }
    }
}