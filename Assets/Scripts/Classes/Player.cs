using Photon.Pun;
using Utils;

public class Player : SingletonMonoPun<Player>
{
    public Camp camp;

    protected override void Awake()
    {
        base.Awake();
        camp = PhotonNetwork.IsMasterClient ? Camp.White : Camp.Black;
    }
}
