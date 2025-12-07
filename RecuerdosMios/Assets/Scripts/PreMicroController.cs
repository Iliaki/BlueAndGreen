using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PreMicroController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI titleText;

    [Header("Icono de tipo de entrada")]
    public Image inputIcon;

    // Sprites para cada tipo de control
    public Sprite spriteMouse;
    public Sprite spriteSpacebar;
    public Sprite spriteFullKeyboard;
    public Sprite spriteWASD;
    public Sprite spriteMixed; // opcional, por si quieres algo generico

    void Start()
    {
        var gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("PreMicroController: No hay GameManager.Instance");
            return;
        }

        string sceneName = gm.NextMicrogameScene;

        // 1) Texto del microjuego
        if (titleText != null)
        {
            string niceName = GetNiceNameForScene(sceneName);
            titleText.SetText("Siguiente microjuego: " + niceName);
        }

        // 2) Icono segun tipo de entrada
        if (inputIcon != null)
        {
            inputIcon.sprite = GetInputSpriteForScene(sceneName);
            inputIcon.enabled = (inputIcon.sprite != null);
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

        Debug.Log("PreMicroController: Continuar -> cargando " + gm.NextMicrogameScene);
        gm.StartSelectedMicrogame();
    }

    // ----------------- Helpers -----------------

    // Nombre de escena -> nombre "bonito" para mostrar
    string GetNiceNameForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "loteria":
                return "Loteria";
            case "ruleta":
                return "Ruleta rusa";
            case "perroCereal":
                return "Aporrea al perro";
            case "MarcasCigarros":
                return "Di 5 marcas de cigarros";
            case "encuentraGato":
                return "Encuentra al michi";
            default:
                return sceneName;
        }
    }

    // Nombre de escena -> sprite de tipo de input
    Sprite GetInputSpriteForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "loteria":
                // Click rapido en la carta -> mouse
                return spriteMouse;

            case "ruleta":
                // Ruleta rusa -> barra espaciadora
                return spriteSpacebar;

            case "perroCereal":
                // Golpear barra muchas veces -> barra espaciadora
                return spriteSpacebar;

            case "MarcasCigarros":
                // QTE de teclas -> teclado completo
                return spriteFullKeyboard;

            case "encuentraGato":
                // Buscar al gato con click -> mouse
                return spriteMouse;

            // Ejemplo de microjuego WASD:
            // case "NombreEscenaWASD":
            //     return spriteWASD;

            default:
                // Icono generico si no sabemos
                return spriteMixed;
        }
    }
}
