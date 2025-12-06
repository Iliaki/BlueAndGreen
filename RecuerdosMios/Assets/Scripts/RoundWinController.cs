using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundWinController : MonoBehaviour
{
    public void OnBackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnNextRound() // si luego quieren rondas 2,3...
    {
        SceneManager.LoadScene("MainGame");
    }
}
