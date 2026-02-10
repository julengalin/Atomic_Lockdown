using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class TMPTypeSoundPerKey : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typeSound;
    public AudioClip deleteSound; // opcional (puede ser el mismo)

    [Tooltip("Tiempo mínimo entre sonidos (segundos)")]
    public float cooldown = 0.03f;

    TMP_InputField input;
    float lastPlayTime;
    string lastText = "";

    // Para no romper validaciones que ya tengas
    TMP_InputField.OnValidateInput previousValidate;

    void Awake()
    {
        input = GetComponent<TMP_InputField>();
        lastText = input.text;

        // Hook por carácter tecleado
        previousValidate = input.onValidateInput;
        input.onValidateInput = ValidateAndSound;

        // Hook para detectar borrado (backspace no siempre pasa por Validate)
        input.onValueChanged.AddListener(OnValueChanged);
    }

    private char ValidateAndSound(string text, int charIndex, char addedChar)
    {
        // Llama a la validación previa si existía
        if (previousValidate != null)
        {
            addedChar = previousValidate(text, charIndex, addedChar);
            if (addedChar == '\0') return '\0';
        }

        Play(typeSound);
        return addedChar;
    }

    private void OnValueChanged(string newText)
    {
        // Si ha disminuido longitud => borrado/backspace
        if (newText.Length < lastText.Length)
        {
            Play(deleteSound ? deleteSound : typeSound);
        }

        lastText = newText;
    }

    private void Play(AudioClip clip)
    {
        if (!audioSource || !clip) return;

        if (Time.unscaledTime - lastPlayTime < cooldown) return;
        lastPlayTime = Time.unscaledTime;

        // Reinicia para que sea "click" instantáneo aunque el clip sea algo largo
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.time = 0f;
        audioSource.Play();
    }

    void OnDestroy()
    {
        if (input)
        {
            input.onValueChanged.RemoveListener(OnValueChanged);

            // Restaura validate anterior si este script se destruye
            input.onValidateInput = previousValidate;
        }
    }
}
