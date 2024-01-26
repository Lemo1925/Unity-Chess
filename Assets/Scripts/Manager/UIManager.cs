using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Button List")] 
    public Button single;

    public Button multiple;
    public Button exit;

    [Header("背景音乐按钮设置")]
    public AudioSource source;
    public Sprite highlightsSprite;
    public Sprite defaultSprite;


    private void OnEnable()
    {
        single.onClick.AddListener(SingleBtn_Click);
        multiple.onClick.AddListener(MultiBtn_Click);
        exit.onClick.AddListener(ExitBtn_Click);
    }
    
    public void SoundBtn_Click(Button button)
    {
        Image btnImg = button.GetComponent<Image>();
        if (source.isPlaying)
        {
            btnImg.sprite = defaultSprite;
            source.Stop();
        }
        else
        {
            btnImg.sprite = highlightsSprite;
            source.Play();
        }
    }
    
    private void SingleBtn_Click()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(single));
        ScenesManager.Instance.Translate("UIScene", "GameScene");
        GameManager.model = GameModel.SINGLE;
    }

    private void MultiBtn_Click()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(multiple));
        Launcher.LauncherON();
    }

    private void ExitBtn_Click()
    {
        StartCoroutine(EffectTool.Instance.ScaleAnimation(exit));
        Application.Quit();
    }
}
