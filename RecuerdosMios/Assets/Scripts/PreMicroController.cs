using UnityEngine;
using TMPro;

public class PreMicroController : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    void Start()
    {
        if (GameManager.Instance != null && titleText != null)
        {
            string sceneName = GameManager.Instance.NextMicrogameScene;
            // Aquí puedes mapear nombres feos de escena a textos bonitos
            // Por ahora, lo mostramos directo:
            titleText.SetText($"Siguiente microjuego: {sceneName}");
        }
    }

    public void OnContinueButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartSelectedMicrogame();
        }
    }
}
