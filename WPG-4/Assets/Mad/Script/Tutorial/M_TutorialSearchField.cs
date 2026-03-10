using UnityEngine;
using TMPro;
using System.Collections;

public class M_TutorialSearchField : MonoBehaviour
{
    [Header("Display")]
    public TMP_Text baseTextDisplay;
    public TMP_Text typedTextDisplay;

    [Header("Keyboard")]
    public M_KeyboardController keyboard;

    [Header("Typing")]
    public string targetText = "pawshopp";
    public string typedText = "";

    [Header("Cursor")]
    public float cursorBlinkSpeed = 0.5f;

    [Header("State")]
    public bool isActive = false;
    public bool isFinished = false;
    public bool isSubmitted = false;

    bool cursorVisible = true;

    void Start()
    {
        RefreshVisual();
        StartCoroutine(CursorBlink());
    }

    public void ActivateField()
    {
        isActive = true;
        isFinished = false;
        isSubmitted = false;
        typedText = "";
        RefreshVisual();
    }

    public void DeactivateField()
    {
        isActive = false;
        isFinished = false;
        isSubmitted = false;
        typedText = "";
        RefreshVisual();
    }

    public void OnFieldClicked()
    {
        if (!isActive) return;

        if (keyboard != null)
            keyboard.ShowKeyboard();
    }

    public void AddCharacter(string c)
    {
        if (!isActive) return;

        if (c == "CAPS")
        {
            if (keyboard != null)
                keyboard.ToggleCaps();
            return;
        }

        if (c == "BACK")
        {
            if (typedText.Length > 0)
                typedText = typedText.Substring(0, typedText.Length - 1);

            CheckFinish();
            RefreshVisual();
            return;
        }

        if (c == "ENTER")
        {
            if (isFinished)
            {
                isSubmitted = true;

                if (keyboard != null)
                    keyboard.HideKeyboard();
            }
            return;
        }

        if (c == "SPACE")
        {
            AddRawCharacter(" ");
            return;
        }

        if (keyboard != null && keyboard.isCaps)
            AddRawCharacter(c.ToUpper());
        else
            AddRawCharacter(c.ToLower());
    }

    void AddRawCharacter(string c)
    {
        if (typedText.Length >= targetText.Length)
            return;

        typedText += c;
        CheckFinish();
        RefreshVisual();
    }

    void CheckFinish()
    {
        isFinished = typedText.ToLower() == targetText.ToLower();
    }

    void RefreshVisual()
    {
        if (baseTextDisplay != null)
            baseTextDisplay.text = targetText;

        if (typedTextDisplay != null)
        {
            if (isActive && !isSubmitted && cursorVisible)
                typedTextDisplay.text = typedText + "|";
            else
                typedTextDisplay.text = typedText;
        }
    }

    IEnumerator CursorBlink()
    {
        while (true)
        {
            cursorVisible = !cursorVisible;
            RefreshVisual();
            yield return new WaitForSecondsRealtime(cursorBlinkSpeed);
        }
    }
}