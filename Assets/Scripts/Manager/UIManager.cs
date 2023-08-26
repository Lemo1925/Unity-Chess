using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Button List")] 
    public Button Single;

    public Button Multiple;
    public Button Exit;

    [Header("背景音乐按钮设置")]
    public AudioSource source;
    public Sprite highlightsSprite;
    public Sprite defaultSprite;


    private void OnEnable()
    {
        Single.onClick.AddListener(SingleBtn_Click);
        Multiple.onClick.AddListener(MultiBtn_Click);
        Exit.onClick.AddListener(ExitBtn_Click);
    }
    
    public void SoundBtn_Click(Button button)
    {
        Image BtnImg = button.GetComponent<Image>();
        if (source.isPlaying)
        {
            BtnImg.sprite = defaultSprite;
            source.Stop();
        }
        else
        {
            BtnImg.sprite = highlightsSprite;
            source.Play();
        }
    }
    
    private void SingleBtn_Click()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Single));
        ScenesManager.instance.Translate("Scenes/UIScene", "Scenes/GameScene");
        GameController.model = GameModel.SINGLE;
    }

    private void MultiBtn_Click()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Multiple));
        Launcher.LauncherON();
    }

    private void ExitBtn_Click()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(Exit));
        Application.Quit();
    }
}
