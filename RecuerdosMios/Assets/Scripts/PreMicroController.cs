using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PreMicroController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI titleText;
    public Image inputIcon;

    [Header("Timer visual")]
    public Image timerFill;       // Image (Filled)
    public float preDuration = 3f;

    // Sprites para cada tipo de control
    public Sprite spriteMouse;
    public Sprite spriteSpacebar;
    public Sprite spriteFullKeyboard;
    public Sprite spriteWASD;
    public Sprite spriteMixed;

    private float currentTime;
    private bool advanced = false;

    void Start()
    {
        var gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("PreMicroController: No hay GameManager.Instance");
            return;
        }

        string sceneName = gm.NextMicrogameScene;

        // Texto del microjuego
        if (titleText != null)
        {
            string niceName = GetNiceNameForScene(sceneName);
            titleText.SetText("Siguiente microjuego: " + niceName);
        }

        // Icono de tipo de entrada
        if (inputIcon != null)
        {
            inputIcon.sprite = GetInputSpriteForScene(sceneName);
            inputIcon.enabled = (inputIcon.sprite != null);
        }

        // Timer
        currentTime = preDuration;
        if (timerFill != null)
        {
            timerFill.type = Image.Type.Filled;
            timerFill.fillAmount = 1f;
        }
    }

    void Update()
    {
        if (advanced) return;

        currentTime -= Time.deltaTime;

        if (timerFill != null && preDuration > 0f)
        {
            float t = Mathf.Clamp01(currentTime / preDuration);
            timerFill.fillAmount = t;
        }

        if (currentTime <= 0f)
        {
            AdvanceToMicrogame();
        }
    }

    void AdvanceToMicrogame()
    {
        if (advanced) return;
        advanced = true;

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("PreMicroController: No hay GameManager.Instance al avanzar");
            return;
        }

        Debug.Log("PreMicroController: tiempo agotado, cargando " + gm.NextMicrogameScene);
        gm.StartSelectedMicrogame();
    }

    // -------- Helpers --------

    string GetNiceNameForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "loteria":
                return "Loteria";
            case "ruleta":
                return "Ruleta rusa";
            case "perroCereal":
                return "Perro y cereal";
            case "MarcasCigarros":
                return "Marcas de cigarros";
            case "encuentraGato":
                return "Encuentra al michi";
            default:
                return sceneName;
        }
    }

    Sprite GetInputSpriteForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "loteria":
                return spriteMouse;
            case "ruleta":
                return spriteSpacebar;
            case "perroCereal":
                return spriteSpacebar;
            case "MarcasCigarros":
                return spriteFullKeyboard;
            case "encuentraGato":
                return spriteMouse;
            default:
                return spriteMixed;
        }
    }
}
