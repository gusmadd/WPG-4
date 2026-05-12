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
    public string defaultText = "pawshopp";
    bool isFirstInput = true;

    [Header("References")]
    public M_MonitorManager monitorManager;
    public M_KeyboardController keyboard;
    public TextMeshPro textDisplay;

    [Header("Cursor")]
    public bool isTyping = false;
    public float blinkSpeed = 0.5f;

    [Header("Text Color")]
    public Color32 defaultGuideColor = new Color32(128, 128, 128, 255);
    public Color activeTextColor = Color.black;

    bool cursorVisible = true;
    Coroutine blinkRoutine;

    void Start()
    {
        currentText = defaultText;
        UpdateText();
        StartBlinkRoutine();
    }

    void OnEnable()
    {
        UpdateText();
    }

    void OnMouseDown()
    {
        if (M_GameManager.Instance == null) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
            return;

        M_AudioManager.Instance?.PlayCursorClick();
        M_PlayerController.Instance?.PlayTyping();

        keyboard.ShowKeyboard();
        ForceTyping();
    }

    public void AddCharacter(string c)
    {
        if (c == "CAPS")
        {
            M_AudioManager.Instance?.PlayKeyboardClick();
            M_PlayerController.Instance?.PlayTyping();
            keyboard.ToggleCaps();
            return;
        }

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

        if (c == "ENTER")
        {
            M_AudioManager.Instance?.PlayKeyboardClick();
            M_PlayerController.Instance?.PlayTyping();
            Submit();
            return;
        }

        if (c == "SPACE")
        {
            M_AudioManager.Instance?.PlaySpacebar();
            M_PlayerController.Instance?.PlayTyping();

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

        if (currentText.Length < maxCharacter)
        {
            M_AudioManager.Instance?.PlayKeyboardClick();
            M_PlayerController.Instance?.PlayTyping();

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

    void UpdateText()
    {
        if (textDisplay == null) return;

        textDisplay.color = isFirstInput ? defaultGuideColor : activeTextColor;

        if (isTyping && cursorVisible)
            textDisplay.text = currentText + "|";
        else
            textDisplay.text = currentText;
    }

    void Submit()
    {
        isTyping = false;
        cursorVisible = true;
        UpdateText();

        if (monitorManager != null)
            monitorManager.HandleSearch(currentText);

        if (keyboard != null)
            keyboard.HideKeyboard();
    }

    public void ForceTyping()
    {
        if (!gameObject.activeInHierarchy) return;

        isTyping = true;
        cursorVisible = true;
        UpdateText();

        StartBlinkRoutine();
    }

    public void ResetToDefault()
    {
        currentText = defaultText;
        isFirstInput = true;
        isTyping = false;
        cursorVisible = true;
        UpdateText();
    }

    public void SetTextFromExternal(string newText)
    {
        currentText = newText;
        isFirstInput = false;
        isTyping = false;
        cursorVisible = true;
        UpdateText();
    }

    public void OnQuickLinkClicked(string url)
    {
        currentText = url;
        isFirstInput = false;
        isTyping = false;
        cursorVisible = true;
        UpdateText();

        if (keyboard != null)
            keyboard.HideKeyboard();

        if (monitorManager != null)
            monitorManager.HandleSearch(url);
    }

    void StartBlinkRoutine()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(CursorBlink());
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
            else
            {
                cursorVisible = true;
            }

            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
