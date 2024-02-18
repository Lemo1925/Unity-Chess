using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Panel
{
    public abstract class Panel: MonoBehaviourPunCallbacks
    {
        [SerializeField]private Image background;
        
        public void Show() => background.gameObject.SetActive(true);

        public void Hid() => background.gameObject.SetActive(false);

    }
}