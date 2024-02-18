using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Controller
{
    public class CameraController : SingletonMono<CameraController>
    {
        public Button cameraButton;
        public Sprite whitePic;
        public Sprite blackPic;
        public Transform whiteTransform, blackTransform;

        private bool CameraFlag { get; set; } = true;

        private void OnEnable() => EventManager.OnCameraChangedEvent += ChangeCameraPos;

        private void OnDisable() => EventManager.OnCameraChangedEvent -= ChangeCameraPos;

        private void Start() => CameraTransition(whiteTransform);

        #region Camera相关

        public void ChangeCameraPos()
        {
            var btnImg = cameraButton.GetComponent<Image>();
            btnImg.sprite = CameraFlag ? blackPic : whitePic;
            CameraTransition(CameraFlag ? blackTransform : whiteTransform);
            CameraFlag = !CameraFlag;
        }

        public void InitCameraFlag(Player player) => CameraFlag = player.camp != Camp.White;

        private static void CameraTransition(Transform target)
        {
            LeanTween.move(Camera.main!.gameObject, target.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.rotate(Camera.main!.gameObject, target.rotation.eulerAngles, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }

        #endregion
    }
}