using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class ScenesManager : SingletonMono<ScenesManager>
{
    
    public CanvasGroup fadeGroup;

    public float fadeDuration = 0.5f;
    
    protected override void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded) 
            SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
        
        base.Awake();
        fadeGroup = GameObject.Find("FadeCanvas").GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("Fade").gameObject);
    }

    public void Translate(string currentScene, string targetScene)
    {
        StartCoroutine( Transition(currentScene, targetScene) );
    }

    private IEnumerator Transition(string currentScene, string targetScene)
    {
        yield return StartCoroutine(Fade(1));
        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        //等待新场景加载完成
        SceneManager.sceneLoaded += OnSceneLoaded;
        //卸载旧场景
        if (currentScene != string.Empty) 
            yield return SceneManager.UnloadSceneAsync(currentScene);

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
