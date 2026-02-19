using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // 🔥 TAMBAH INI

public class M_SearchInput : MonoBehaviour
{
    [Header("Settings")]
    public string currentText = "";
    public int maxCharacter = 20;

    [Header("Default")]
    public string defaultText = "Search...";
    bool isFirstInput = true;

    [Header("References")]
    public M_MonitorManager monitorManager;
    public M_KeyboardController keyboard;
    public TextMeshPro textDisplay;

    [Header("Cursor")]
    public bool isTyping = false;
    public float blinkSpeed = 0.5f;

    bool cursorVisible = true;

    void Start()
    {
        currentText = defaultText;   // 🔥 Set default
        UpdateText();
        StartCoroutine(CursorBlink());
    }

    void OnMouseDown()
    {
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
            return;
        M_AudioManager.Instance?.PlayCursorClick();
        keyboard.ShowKeyboard();
        ForceTyping();
    }

    public void AddCharacter(string c)
    {
        // CAPS
        if (c == "CAPS")
        {
            keyboard.ToggleCaps();
            return;
        }

        // BACKSPACE
        if (c == "BACK")
        {
            M_AudioManager.Instance?.PlayKeyboardClick();
            if (!isFirstInput && currentText.Length > 0)
            {
                currentText = currentText.Substring(0, currentText.Length - 1);

                if (currentText.Length == 0)
                {
                    ResetToDefault();
                    return;
                }
            }

            UpdateText();
            return;
        }

        // ENTER
        if (c == "ENTER")
        {
            M_AudioManager.Instance?.PlayKeyboardClick();
            Submit();
            return;
        }

        // SPACE
        if (c == "SPACE")
        {
            M_AudioManager.Instance?.PlaySpacebar();
            if (isFirstInput)
            {
                currentText = "";
                isFirstInput = false;
            }

            if (currentText.Length < maxCharacter)
                currentText += " ";

            UpdateText();
            return;
        }

        // HURUF
        if (currentText.Length < maxCharacter)
        {
            M_AudioManager.Instance?.PlayKeyboardClick();
            // 🔥 Hapus default saat huruf pertama diketik
            if (isFirstInput)
            {
                currentText = "";
                isFirstInput = false;
            }

            if (keyboard.isCaps)
                currentText += c.ToUpper();
            else
                currentText += c.ToLower();
        }

        UpdateText();
    }

    IEnumerator CursorBlink()
    {
        while (true)
        {
            if (isTyping)
            {
                cursorVisible = !cursorVisible;
                UpdateText();
            }

            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    void UpdateText()
    {
        if (isTyping && cursorVisible)
            textDisplay.text = currentText + "|";
        else
            textDisplay.text = currentText;
    }

    void Submit()
    {
        isTyping = false;
        UpdateText();
        monitorManager.HandleSearch(currentText);
        keyboard.HideKeyboard();
    }

    void OnEnable()
    {
        UpdateText();
    }

    public void ForceTyping()
    {
        if (!gameObject.activeInHierarchy) return; // 🔥 safety guard
        StopAllCoroutines();
        isTyping = true;
        cursorVisible = true;
        UpdateText();
        StartCoroutine(CursorBlink());
    }

    public void ResetToDefault()
    {
        currentText = defaultText;
        isFirstInput = true;
        UpdateText();
    }
}
