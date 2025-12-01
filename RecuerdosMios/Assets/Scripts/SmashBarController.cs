using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SmashBarController : MonoBehaviour
{
    [Header("Referencia UI - Barra principal")]
    public Slider bar;                  // Slider tipo "bomba de aire"

    [Header("Referencia UI - Timer y textos")]
    public Image timerFill;             // Image (Filled)
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;

    [Header("Parámetros de juego")]
    public float maxValue = 100f;
    public float addPerPress = 10f;
    public float decayPerSecond = 5f;
    public float timeLimit = 5f;

    [Header("Mascota")]
    public Image petImage;
    public Sprite petNormalSprite;
    public Sprite petHitSprite;
    public float petHitDuration = 0.1f;

    [Header("Periódico")]
    public RectTransform newspaper;
    public float newspaperHitOffset = 50f;
    public float newspaperReturnSpeed = 10f;

    [Header("Comportamiento")]
    public bool stopWhenFull = true;

    [Header("Integración futura")]
    public UnityEvent<bool> onMicrogameEnd;

    private bool isFinished = false;
    private float currentTime;
    private float petHitTimer = 0f;

    private Vector3 newspaperBasePos;
    private bool hasNewspaperBasePos = false;

    void Start()
    {
        // Obtener slider si no lo asignaron
        if (bar == null)
            bar = GetComponent<Slider>();

        bar.minValue = 0f;
        bar.maxValue = maxValue;
        bar.value = 0f;

        currentTime = timeLimit;

        if (timerFill != null)
            timerFill.fillAmount = 1f;

        if (instructionText != null)
            instructionText.SetText("Golpea la barra con ESPACIO para llenarla");

        if (resultText != null)
            resultText.SetText("");

        // Mascota en sprite normal
        if (petImage != null && petNormalSprite != null)
            petImage.sprite = petNormalSprite;

        // Periódico
        if (newspaper != null)
        {
            newspaperBasePos = newspaper.anchoredPosition;
            hasNewspaperBasePos = true;
        }
    }

    void Update()
    {
        if (isFinished) return;

        // Golpe con espacio
        if (Input.GetKeyDown(KeyCode.Space))
            HandleSpacePress();

        HandleBarDecay();
        HandleTimer();
        HandlePetSprite();
        HandleNewspaperReturn();
    }

    private void HandleSpacePress()
    {
        // Subir barra
        bar.value += addPerPress;
        bar.value = Mathf.Clamp(bar.value, 0, maxValue);

        // Mascota animación hit
        if (petImage != null && petHitSprite != null)
        {
            petImage.sprite = petHitSprite;
            petHitTimer = petHitDuration;
        }

        // Periódico animación
        if (newspaper != null && hasNewspaperBasePos)
        {
            Vector2 pos = newspaper.anchoredPosition;
            pos.y = newspaperBasePos.y + newspaperHitOffset;
            newspaper.anchoredPosition = pos;
        }

        // Checar victoria
        if (bar.value >= maxValue && stopWhenFull)
        {
            bar.value = maxValue;
            Win();
        }
    }

    private void HandleBarDecay()
    {
        if (bar.value > 0f)
        {
            bar.value -= decayPerSecond * Time.deltaTime;
            bar.value = Mathf.Clamp(bar.value, 0f, maxValue);
        }
    }

    private void HandleTimer()
    {
        currentTime -= Time.deltaTime;

        if (timerFill != null)
            timerFill.fillAmount = Mathf.Clamp01(currentTime / timeLimit);

        if (currentTime <= 0f && !isFinished)
        {
            if (bar.value >= maxValue)
                Win();
            else
                Lose();
        }
    }

    private void HandlePetSprite()
    {
        if (petHitTimer > 0f)
        {
            petHitTimer -= Time.deltaTime;
            if (petHitTimer <= 0f && petNormalSprite != null)
                petImage.sprite = petNormalSprite;
        }
    }

    private void HandleNewspaperReturn()
    {
        if (newspaper != null)
        {
            newspaper.anchoredPosition = Vector2.Lerp(
                newspaper.anchoredPosition,
                newspaperBasePos,
                newspaperReturnSpeed * Time.deltaTime
            );
        }
    }

    private void Win()
    {
        if (isFinished) return;
        isFinished = true;

        // UI / mensajes
        // ...

        // Avisar al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMicrogameEnd(true);
        }

        // (Opcional) seguir usando el UnityEvent si quieres
        onMicrogameEnd?.Invoke(true);
    }



    private void Lose()
    {
        if (isFinished) return;
        isFinished = true;

        // UI / mensajes
        // ...

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMicrogameEnd(false);
        }

        onMicrogameEnd?.Invoke(false);
    }

}

