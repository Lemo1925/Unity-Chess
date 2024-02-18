using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static int ready;
    public static GameObject player;
    public static GameModel model;
    public Transform white, black;
    private void Awake()
    {
        var PhotonView = GetComponent<PhotonView>();
        if (model == GameModel.Multiple && PhotonView == null)
        {
            PhotonView view = gameObject.AddComponent<PhotonView>();
            view.ViewID = 1; 
            if (!PhotonNetwork.IsMasterClient) SendMsg("New Player Coming");
        }

        if (model == GameModel.Single && PhotonView != null) Destroy(PhotonView);
    }

    public static Player GetPlayer() => player.GetComponent<Player>();

    public void SendMsg(string msg)
    {
        photonView.RPC("ReceiverMsg", RpcTarget.Others, msg);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " has left the game.");
        if (GameController.State == GameState.Init || GameController.State == GameState.Over) return;
        GameStatus.Instance.DrawOver();
    }

    public void OnReadyButtonClick()
    {
        photonView.RPC("Ready", RpcTarget.All);
        player = PhotonNetwork.IsMasterClient ? 
            PhotonNetwork.Instantiate("Player", white.position, white.rotation) : 
            PhotonNetwork.Instantiate("Player", black.position, black.rotation);
    }

    [PunRPC] public void Ready()
    {
        ready += 1;
        UIController.Instance.readyPanel.UpdateUI();
    }

    [PunRPC] public void SyncReady(int allReady)
    {
        ready = allReady;
        UIController.Instance.readyPanel.UpdateUI();
    }

    [PunRPC] public void ReceiverMsg(string msg)
    {
        if(msg == "New Player Coming")
        {
            photonView.RPC("SyncReady", RpcTarget.All, ready);
        }
    }
}