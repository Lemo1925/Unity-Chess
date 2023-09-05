using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public static int ready;
    public static GameObject player;
    public static GameModel model;
    public Transform white, black;
    private void Awake()
    {
        var PhotonView = GetComponent<PhotonView>();
        if (model == GameModel.MULTIPLE && PhotonView == null)
        {
            PhotonView view = gameObject.AddComponent<PhotonView>();
            view.ViewID = 1;
        }

        if (model == GameModel.SINGLE && PhotonView != null)
        {
            Destroy(PhotonView);
        }
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
        UIController.Instance.UpdateUI();
    }
    
}