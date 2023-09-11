using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject lobbyGameObject;
    public GameObject loginGameObject;
    public Button joinButton;
    public Button backButton;
    public InputField roomName;
    
    
    public static void  LauncherON() => PhotonNetwork.ConnectUsingSettings();

    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnConnectedToMaster()
    {
        lobbyGameObject.SetActive(false);
        
        loginGameObject.SetActive(true);
    }

    public void OnJoinOrCreatButtonClick()
    {
        var options = new RoomOptions { MaxPlayers = 2 };
        if (roomName.text.Length < 2) return;
        StartCoroutine(EffectTool.Instance.ScaleAnimation(joinButton));
        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, default);
        loginGameObject.SetActive(false);
    }

    public void OnBackButtonClick()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(backButton));
        loginGameObject.SetActive(false);
        PhotonNetwork.Disconnect();
        lobbyGameObject.SetActive(true);
    }
    
    public override void OnJoinedRoom()
    {
        PhotonNetwork.MasterClient.NickName = "Master";
        if (PhotonNetwork.PlayerListOthers.Length > 0)
        {
            PhotonNetwork.PlayerListOthers[0].NickName = "Slaves";
        }
        GameManager.model = GameModel.MULTIPLE;
        PhotonNetwork.LoadLevel(2);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        print("Disconnected");
    }
}
