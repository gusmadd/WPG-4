using UnityEngine;
using TMPro;
using System.Collections;

public class InputController : MonoBehaviour
{
    [Header("Settings")]
    public int maxCharacters = 20;
    public string defaultText = "Search...";

    [Header("References")]
    public TMP_Text searchText;
    public M_KeyboardController M_KeyboardController;

    [Header("Cursor")]
    public float blinkSpeed = 0.5f;
    private bool isTyping = false;
    private bool cursorVisible = true;

    private string currentText = "";
    private bool firstInput = true;

    void Start()
    {
        if (searchText == null)
        {
            Debug.LogWarning("InputController: TMP_Text not assigned!");
            return;
        }

        currentText = defaultText;
        UpdateText();
        StartCoroutine(CursorBlink());
    }

    void OnMouseDown()
    {
        if (M_KeyboardController != null)
            M_KeyboardController.ShowKeyboard();

        ForceTyping();
    }

    public void AddCharacter(string c)
    {
        // BACKSPACE
        if (c == "BACK")
        {
            if (!firstInput && currentText.Length > 0)
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
            Submit();
            return;
        }

        // SPACE
        if (c == "SPACE")
        {
            if (firstInput)
            {
                currentText = "";
                firstInput = false;
            }

            if (currentText.Length < maxCharacters)
                currentText += " ";

            UpdateText();
            return;
        }

        // HURUF
        if (currentText.Length < maxCharacters)
        {
            if (firstInput)
            {
                currentText = "";
                firstInput = false;
            }

            bool caps = M_KeyboardController != null ? M_KeyboardController.isCaps : false;
            currentText += caps ? c.ToUpper() : c.ToLower();
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
            searchText.text = currentText + "|";
        else
            searchText.text = currentText;
    }

    public void ForceTyping()
    {
        StopAllCoroutines();
        isTyping = true;
        cursorVisible = true;
        UpdateText();
        StartCoroutine(CursorBlink());
    }

    public void Submit()
    {
        isTyping = false;
        UpdateText();

        if (M_KeyboardController != null)
        {
            M_KeyboardController.HideKeyboard();
        }
    }

    public void ResetToDefault()
    {
        currentText = defaultText;
        firstInput = true;
        UpdateText();
    }
}