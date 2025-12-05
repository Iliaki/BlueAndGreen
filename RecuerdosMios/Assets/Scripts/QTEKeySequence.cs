using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEKeySequence : MonoBehaviour
{
    [Header("UI")]
    public Image keyImage;            // Imagen de la tecla actual
    public RectTransform spawnArea;   // Área donde puede aparecer la tecla

    [Header("Sprites por tecla")]
    public Sprite TeclaM;
    public Sprite TeclaC;
    public Sprite TeclaF;
    public Sprite TeclaP;
    public Sprite TeclaL;

    [Header("Configuración")]
    [Tooltip("Número de teclas en la secuencia (en tu caso 5).")]
    public int sequenceLength = 5;

    [Tooltip("Si es true, cualquier tecla incorrecta se considera fallo inmediato. Si es false, se ignoran las teclas incorrectas.")]
    public bool loseOnWrongKey = false;

    // Estado interno
    private List<KeyCode> sequence = new List<KeyCode>();
    private int currentIndex = 0;
    private bool finished = false;

    void Start()
    {
        GenerateRandomSequence();
        currentIndex = 0;
        UpdateKeyIcon();
    }

    void Update()
    {
        if (finished) return;

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.M)) HandleKeyPress(KeyCode.M);
            else if (Input.GetKeyDown(KeyCode.C)) HandleKeyPress(KeyCode.C);
            else if (Input.GetKeyDown(KeyCode.F)) HandleKeyPress(KeyCode.F);
            else if (Input.GetKeyDown(KeyCode.P)) HandleKeyPress(KeyCode.P);
            else if (Input.GetKeyDown(KeyCode.L)) HandleKeyPress(KeyCode.L);
            else if (loseOnWrongKey)
            {
                Fail();
            }
        }
    }

    void HandleKeyPress(KeyCode pressed)
    {
        if (finished) return;

        KeyCode expected = sequence[currentIndex];

        if (pressed == expected)
        {
            currentIndex++;

            if (currentIndex >= sequence.Count)
            {
                Win();
            }
            else
            {
                UpdateKeyIcon();
            }
        }
        else
        {
            if (loseOnWrongKey)
            {
                Fail();
            }
        }
    }

    void GenerateRandomSequence()
    {
        var pool = new List<KeyCode>
        {
            KeyCode.M,
            KeyCode.C,
            KeyCode.F,
            KeyCode.P,
            KeyCode.L
        };

        // Barajar (Fisher–Yates)
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = pool[i];
            pool[i] = pool[j];
            pool[j] = temp;
        }

        sequence.Clear();
        int length = Mathf.Min(sequenceLength, pool.Count);
        for (int i = 0; i < length; i++)
        {
            sequence.Add(pool[i]);
        }

#if UNITY_EDITOR
        string debugSequence = "Secuencia QTE: ";
        foreach (var k in sequence)
            debugSequence += k + " ";
        Debug.Log(debugSequence);
#endif
    }

    void UpdateKeyIcon()
    {
        if (keyImage == null)
        {
            Debug.LogWarning("No se ha asignado el Image 'keyImage' en el inspector.");
            return;
        }

        keyImage.sprite = GetSpriteForKey(sequence[currentIndex]);

        // Cada vez que cambiamos de tecla, la movemos a una posición aleatoria
        RandomizeKeyPosition();
    }

    Sprite GetSpriteForKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.M: return TeclaM;
            case KeyCode.C: return TeclaC;
            case KeyCode.F: return TeclaF;
            case KeyCode.P: return TeclaP;
            case KeyCode.L: return TeclaL;
            default: return null;
        }
    }

    void RandomizeKeyPosition()
    {
        if (keyImage == null)
            return;

        RectTransform keyRect = keyImage.rectTransform;

        // Si no asignaste spawnArea, usamos el padre del icono como área
        RectTransform area = spawnArea != null
            ? spawnArea
            : keyRect.parent as RectTransform;

        if (area == null)
        {
            Debug.LogWarning("No hay spawnArea ni padre RectTransform para posicionar la tecla.");
            return;
        }

        // Trabajamos en coordenadas locales de 'area'
        Rect rect = area.rect; // rect.xMin/xMax/yMin/yMax, con origen en el pivot

        float x = Random.Range(rect.xMin, rect.xMax);
        float y = Random.Range(rect.yMin, rect.yMax);

        // Si el keyImage es hijo directo de 'area', usamos anchoredPosition
        keyRect.anchoredPosition = new Vector2(x, y);
    }

    void Win()
    {
        finished = true;
        Debug.Log("QTE completado: todas las teclas correctas.");
    }

    void Fail()
    {
        finished = true;
        Debug.Log("QTE fallado: tecla incorrecta.");
    }

    public void ResetQTE()
    {
        finished = false;
        currentIndex = 0;
        GenerateRandomSequence();
        UpdateKeyIcon();
    }
}
