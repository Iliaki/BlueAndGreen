using UnityEngine;
using UnityEngine.UI;

public class SmashBarController : MonoBehaviour
{
    [Header("Referencia UI")]
    public Slider bar;                  // Arrastra aquí el Slider

    [Header("Parámetros de juego")]
    public float maxValue = 100f;       // Valor máximo de la barra
    public float addPerPress = 10f;     // Cuánto sube por cada golpe de espacio
    public float decayPerSecond = 5f;   // Cuánto baja por segundo

    [Header("Estado")]
    public bool stopWhenFull = true;    // Si quieres que el minijuego termine al llenar

    private bool isFull = false;        // Para no disparar la victoria muchas veces

    void Start()
    {
        if (bar == null)
        {
            bar = GetComponent<Slider>();   // Por si el script está en el mismo objeto
        }

        bar.minValue = 0f;
        bar.maxValue = maxValue;
        bar.value = 0f;
    }

    void Update()
    {
        if (isFull) return;  // Ya ganaste, no hacemos nada más

        // 1. Subir la barra al presionar Espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bar.value += addPerPress;
        }

        // 2. Asegurar límites antes de checar victoria
        bar.value = Mathf.Clamp(bar.value, 0f, maxValue);

        // 3. Checar si ya se llenó
        if (bar.value >= maxValue && stopWhenFull)
        {
            bar.value = maxValue; // por si se pasó un poquito
            OnBarFull();
            return;              // Importante: no aplicamos decaimiento este frame
        }

        // 4. Si no se ha llenado, aplicamos decaimiento
        if (bar.value > 0f)
        {
            bar.value -= decayPerSecond * Time.deltaTime;
            bar.value = Mathf.Clamp(bar.value, 0f, maxValue);
        }
    }

    void OnBarFull()
    {
        isFull = true;
        Debug.Log("¡Ganaste el minijuego!");

        // Aquí puedes:
        // - desactivar inputs
        // - mostrar un panel de victoria
        // - cambiar de escena, etc.
        // Ejemplo:
        // SceneManager.LoadScene("SiguienteEscena");
    }
}
