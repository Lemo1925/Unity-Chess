using Photon.Pun;

public class Player : MonoBehaviourPun
{
    public static Player instance;
    public Camp camp;

    private void Awake()
    {
        if (instance == null) instance = this;

        camp = PhotonNetwork.IsMasterClient ? Camp.White : Camp.Black;
    }
}
