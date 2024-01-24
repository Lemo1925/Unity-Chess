using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;
    
    public CanvasGroup fadeGroup;

    public float fadeDuration = 0.5f;
    
    private void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded) 
            SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);

        if (Instance == null) Instance = this;
        fadeGroup = GameObject.Find("FadeCanvas").GetComponentInChildren<CanvasGroup>();
    }

    public void Translate(string currentScene, string targetScene)
    {
        StartCoroutine( Transition(currentScene, targetScene) );
    }

    private IEnumerator Transition(string currentScene, string targetScene)
    {
        yield return StartCoroutine(Fade(1));
        //卸载旧场景
        if (currentScene != string.Empty) yield return SceneManager.UnloadSceneAsync(currentScene);
        
        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        //等待新场景加载完成
        SceneManager.sceneLoaded += OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //取消注册SceneManager.sceneLoaded事件
            SceneManager.sceneLoaded -= OnSceneLoaded;

            //设置新场景为活动场景
            SceneManager.SetActiveScene(scene);
        }

        yield return StartCoroutine(Fade(0));
        //激活新场景
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);

        yield return StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float tar)
    {
        LeanTween.alphaCanvas(fadeGroup, tar, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
    }
    
}
