using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundLoseController : MonoBehaviour
{
    public void OnBackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRetry()
    {
        SceneManager.LoadScene("MainGame");
    }
}
