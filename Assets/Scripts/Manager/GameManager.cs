using Photon.Pun;

public class GameManager : MonoBehaviourPun,IPunObservable
{
    public static int ready;

    
    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
        }
    }

    public void OnReadyButtonClick()
    {
        photonView.RPC("Ready", RpcTarget.All);
    }

    [PunRPC] public void Ready()
    {
        ready += 1;
        UIController.Instance.UpdateUI();
    }
    


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}