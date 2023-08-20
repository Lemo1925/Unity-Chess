using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("按钮特效")]
    public float scaleAmount = 0.8f;
    public float animationDuration = 0.2f;

    private AudioSource source;
    [Header("背景音乐按钮设置")]
    public Sprite highlightsSprite;
    public Sprite defaultSprite;

    
    private void OnEnable()
    {
        source = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        source.loop = true;
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

    public void SingleBtn_Click(Button button)
    {
        StartCoroutine(ScaleAnimation(button));
        ScenesManager.instance.Translate("Scenes/UIScene", "Scenes/GameScene");
    }

    public void MultiBtn_Click(Button button)
    {
        StartCoroutine(ScaleAnimation(button));
    }

    public void ExitBtn_Click(Button button)
    {
        StartCoroutine(ScaleAnimation(button));
        Application.Quit();
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
