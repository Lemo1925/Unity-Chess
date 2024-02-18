using UnityEngine;

namespace Controller.Panel
{
    public class PausePanel:OverPanel
    {
        private bool PauseFlag { get; set; } = true;
        public void GamePause()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PauseFlag)
                {
                    Show("Game Pause");
                    Timer.Instance.StopTimer();
                    GameController.State = GameState.Pause;
                }
                else
                {
                    Hid();
                    Timer.Instance.GoAhead();
                    GameController.State = GameState.Action;
                }
                PauseFlag = !PauseFlag;
            }
        }
    }
}