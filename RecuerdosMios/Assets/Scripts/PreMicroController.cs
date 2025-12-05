using UnityEngine;
using TMPro;

public class PreMicroController : MonoBehaviour
{
    public TextMeshProUGUI titleText; // asigna desde el inspector

    void Start()
    {
        var gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("PreMicroController: No hay GameManager.Instance");
            return;
        }

        if (titleText != null)
        {
            string sceneName = gm.NextMicrogameScene;

            // Si quieres, aqui puedes mapear a nombres bonitos.
            // Por ahora lo mostramos tal cual:
            titleText.SetText($"Siguiente microjuego: {sceneName}");
        }
    }

    public void OnContinueButton()
    {
        var gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("PreMicroController: No hay GameManager.Instance al presionar continuar");
            return;
        }

        Debug.Log("PreMicroController: Continuar a cargando " + gm.NextMicrogameScene);
        gm.StartSelectedMicrogame();
    }
}
