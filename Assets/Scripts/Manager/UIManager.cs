using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("按钮特效")]
    public float scaleAmount = 0.8f;
    public float animationDuration = 0.2f;

    private AudioSource source;

    [Header("按钮列表")]
    public List<Button> buttons = new List<Button>();
    public List<GameObject> tableList = new List<GameObject>();

    [Header("背景音乐按钮设置")]
    public Sprite highlightsSprite;
    public Sprite defaultSprite;


    private void OnEnable()
    {
        source = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        source.loop = true;
        
        buttons[0].onClick.AddListener(SingleBtn_Click);
        buttons[1].onClick.AddListener(MultiBtn_Click);
        buttons[2].onClick.AddListener(ExitBtn_Click);
        buttons[3].onClick.AddListener(MasterBtn_Click);
        buttons[4].onClick.AddListener(SlavesBtn_Click);
        buttons[5].onClick.AddListener(tb1BackBtn_Click);
        buttons[6].onClick.AddListener(WhiteBtn_Click);
        buttons[7].onClick.AddListener(BlackBtn_Click);
        buttons[8].onClick.AddListener(tb2BackBtn_Click);
        buttons[9].onClick.AddListener(JoinBtn_Click);
        buttons[10].onClick.AddListener(tb3BackBtn_Click);
        
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
        StartCoroutine(ScaleAnimation(buttons[0]));
        ScenesManager.instance.Translate("Scenes/UIScene", "Scenes/GameScene");
        GameController.model = GameModel.SINGLE;
    }

    private void ExitBtn_Click()
    {
        StartCoroutine(ScaleAnimation(buttons[2]));
        Application.Quit();
    }

    private void MultiBtn_Click() => StartCoroutine(ChangeList(buttons[1], tableList[0],tableList[1]));
    private void MasterBtn_Click() => StartCoroutine(ChangeList(buttons[3],tableList[1],tableList[2]));
    private void SlavesBtn_Click() => StartCoroutine(ChangeList(buttons[4], tableList[1], tableList[3]));
    private void tb1BackBtn_Click() => StartCoroutine(ChangeList(buttons[5], tableList[1], tableList[0]));
    private void tb2BackBtn_Click() => StartCoroutine(ChangeList(buttons[8], tableList[2], tableList[1]));
    private void tb3BackBtn_Click() => StartCoroutine(ChangeList(buttons[10],tableList[3],tableList[1]));

    private void WhiteBtn_Click()
    {
        StartCoroutine(ScaleAnimation(buttons[6]));
        ScenesManager.instance.Translate("Scenes/UIScene", "Scenes/GameScene");
        GameController.model = GameModel.MULTIPLE;
        GameController.master.camp = Camp.WHITE;
    }
    
    private void BlackBtn_Click()
    {
        StartCoroutine(ScaleAnimation(buttons[7]));
        ScenesManager.instance.Translate("Scenes/UIScene", "Scenes/GameScene");
        GameController.model = GameModel.MULTIPLE;
        GameController.master.camp = Camp.BLACK;
    }

    private void JoinBtn_Click()
    {
        StartCoroutine(ScaleAnimation(buttons[9]));
        ScenesManager.instance.Translate("Scenes/UIScene", "Scenes/GameScene");
        GameController.model = GameModel.MULTIPLE;
        
    }

    private IEnumerator ChangeList(Button button, GameObject tb1, GameObject tb2)
    {

        StartCoroutine(ScaleAnimation(button));

        yield return new WaitForSeconds(animationDuration * 2);

        tb1.SetActive(false);
        tb2.SetActive(true);
    }

    private IEnumerator ScaleAnimation(Button button)
    {
        Vector3 originalScale = button.transform.localScale;

        // 缩小按钮
        LeanTween.scale(button.gameObject, originalScale * scaleAmount, animationDuration);

        // 等待一段时间
        yield return new WaitForSeconds(animationDuration);

        // 恢复按钮原始大小
        LeanTween.scale(button.gameObject, originalScale, animationDuration);
    }
}
