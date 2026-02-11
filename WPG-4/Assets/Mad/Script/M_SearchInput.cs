using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // 🔥 TAMBAH INI

public class M_SearchInput : MonoBehaviour
{
    public string currentText = "";
    public int maxCharacter = 20;

    public M_MonitorManager monitorManager;
    public M_KeyboardController keyboard;

    public TextMeshProUGUI textDisplay;   // 🔥 TAMBAH INI

    void OnMouseDown()
    {
        keyboard.ShowKeyboard();
    }

    public void AddCharacter(string c)
    {
        if (c == "BACK")
        {
            if (currentText.Length > 0)
                currentText = currentText.Substring(0, currentText.Length - 1);
        }
        else if (c == "ENTER")
        {
            Submit();
            return;
        }
        else if (currentText.Length < maxCharacter)
        {
            currentText += c;
        }

        UpdateText();   // 🔥 update visual
    }

    void UpdateText()
    {
        textDisplay.text = currentText;
    }

    void Submit()
    {
        monitorManager.HandleSearch(currentText);
        keyboard.HideKeyboard();
    }
}
