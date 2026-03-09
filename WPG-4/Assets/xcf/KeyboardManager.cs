using UnityEngine;
using TMPro; // kalau pakai TextMeshPro

public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager Instance;

    public string currentText = "";
    public TextMeshPro textDisplay; // atau TextMeshProUGUI / TextMesh

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddChar(string c)
    {
        currentText += c;
        UpdateDisplay();
    }

    public void Backspace()
    {
        if (currentText.Length > 0)
            currentText = currentText.Substring(0, currentText.Length - 1);

        UpdateDisplay();
    }

    public void Clear()
    {
        currentText = "";
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (textDisplay != null)
            textDisplay.text = currentText;
    }
}
