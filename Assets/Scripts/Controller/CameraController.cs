using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Camera gameCamera;
    // 相机位置表示，true 代表白方， black代表黑方；
    private bool cameraFlag = true;
    private float duration = 0.5f;

    [Header("相机控制按钮")] 
    public Button button;
    public Sprite WhitePic;
    public Sprite BlackPic;
    
    [Header("相机位置")]
    public Transform WhiteTransform;
    public Transform BlackTransform;

    private void OnEnable()
    {
        EventManager.OnGameSwitchedEvent += CameraBtn_Click;
    }

    private void OnDisable()
    {
        EventManager.OnGameSwitchedEvent -= CameraBtn_Click;
    }

    private void Awake()
    {
        if (GameController.RoundType == Camp.WHITE)
        {
            cameraFlag = true;
            CameraTransition(WhiteTransform);
        }
        else
        {
            cameraFlag = false;
            CameraTransition(BlackTransform);
        }
    }

    public void CameraBtn_Click()
    {
        var BtnImg = button.GetComponent<Image>();
        if (cameraFlag)
        {
            BtnImg.sprite = BlackPic;
            CameraTransition(BlackTransform);
        }
        else
        {
            BtnImg.sprite = WhitePic;
            CameraTransition(WhiteTransform);
        }
        cameraFlag = !cameraFlag;
    }
    
    private void CameraTransition(Transform target)
    {
        LeanTween.move(gameCamera.gameObject, target.position, duration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotate(gameCamera.gameObject, target.rotation.eulerAngles, duration).setEase(LeanTweenType.easeInOutQuad);
    }
}
