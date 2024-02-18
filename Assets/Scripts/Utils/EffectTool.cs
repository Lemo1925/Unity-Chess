using System.Collections;
using UnityEngine;


public class EffectTool
{
    private static EffectTool _instance;

    public static EffectTool Instance => _instance ??= new EffectTool();

    private const float ScaleAmount = 0.8f;
    private const float AnimationDuration = 0.2f;

    // UI按钮缩放效果
    public IEnumerator ScaleAnimation(Component button)
    {
        Vector3 originalScale = button.transform.localScale;
        // 缩小按钮
        LeanTween.scale(button.gameObject, originalScale * ScaleAmount, AnimationDuration);
        // 等待一段时间
        yield return new WaitForSeconds(AnimationDuration);
        // 恢复按钮原始大小
        LeanTween.scale(button.gameObject, originalScale, AnimationDuration);
    }
}
