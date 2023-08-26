using System.Collections;
using UnityEngine;


public class EffectTool
{
    private static EffectTool instance;

    public static EffectTool Instance
        => instance ??= new EffectTool();

    private float scaleAmount = 0.8f;
    private float animationDuration = 0.2f;
    
    public IEnumerator ScaleAnimation(Component button)
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
