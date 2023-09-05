using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager instance;
    
    public CanvasGroup fadeGroup;

    public float fadeDuration = 0.5f;
    
    private void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded) 
            SceneManager.LoadScene("Scenes/UIScene", LoadSceneMode.Additive);

        if (instance == null) instance = this;
        fadeGroup = GameObject.Find("FadeCanvas").GetComponentInChildren<CanvasGroup>();
    }

    public void Translate(string currentScene, string targetScene)
    {
        StartCoroutine( Transition(currentScene, targetScene) );
    }

    private IEnumerator Transition(string currentScene, string targetScene)
    {
        yield return StartCoroutine(Fade(1));
        if (currentScene != string.Empty) yield return SceneManager.UnloadSceneAsync(currentScene);
        
        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        //激活新场景
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);

        yield return Fade(0);
    }

    private IEnumerator Fade(float tar)
    {
        LeanTween.alphaCanvas(fadeGroup, tar, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
    }
    
}
