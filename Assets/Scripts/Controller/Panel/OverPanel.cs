using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Panel
{
    public class OverPanel:Panel
    {
        [SerializeField]private Text text;
        [SerializeField]private Button againButton;
        [SerializeField]private Button menuButton;

        public void Show(string txt)
        {
            text.text = txt;
            Show();
        }
        
        public void OnceAgain()
        {
            StartCoroutine(EffectTool.Instance.ScaleAnimation(againButton));
            GameStatus.ResetGame();
            switch (GameManager.model)
            {
                case GameModel.Multiple:
                    GameStatus.Instance.OnceAgain();
                    break;
                case GameModel.Single:
                    ScenesManager.Instance.Translate("GameScene", "GameScene");
                    break;
            }
        }
        
        public void BackToMenu()
        {
            StartCoroutine(EffectTool.Instance.ScaleAnimation(menuButton));
            GameStatus.ResetGame();
            switch (GameManager.model)
            {
                case GameModel.Multiple:
                    PhotonNetwork.LoadLevel(1);
                    break;
                case GameModel.Single:
                    ScenesManager.Instance.Translate("GameScene", "UIScene");
                    break;
            }
        }
    }
}