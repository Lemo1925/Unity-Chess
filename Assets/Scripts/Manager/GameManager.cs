using UnityEngine.SceneManagement;

public class GameManager : UnityEngine.MonoBehaviour
{
    private void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            SceneManager.LoadScene("Scenes/UIScene", LoadSceneMode.Additive);
        }
    }
}