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

    [Header("Focus Move")]
    public Transform moveTarget;
    public float focusDelay = 1f;
    public float focusMoveY = 0.35f;
    public float focusMoveDuration = 0.15f;

    [Header("Quick Links")]
    public GameObject[] quickLinks = new GameObject[3];
    public float linkAppearInterval = 0.12f;

    bool cursorVisible = true;
    bool isFocused = false;

    Vector3 baseLocalPosition;

    Coroutine blinkRoutine;
    Coroutine focusRoutine;
    Coroutine moveRoutine;

    void Start()
    {
        currentText = defaultText;
        UpdateText();

        if (moveTarget == null)
            moveTarget = transform;

        baseLocalPosition = moveTarget.localPosition;

        HideQuickLinksInstant();
        StartBlinkRoutine();
    }

    void OnEnable()
    {
        UpdateText();
        HideQuickLinksInstant();

        if (moveTarget != null)
            moveTarget.localPosition = baseLocalPosition;
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
        FocusSearchField();
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

        UnfocusSearchField();

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

        UnfocusSearchField();
    }

    public void SetTextFromExternal(string newText)
    {
        currentText = newText;
        isFirstInput = false;
        isTyping = false;
        cursorVisible = true;
        UpdateText();

        UnfocusSearchField();
    }

    public void OnQuickLinkClicked(string url)
    {
        currentText = url;
        isFirstInput = false;
        isTyping = false;
        cursorVisible = true;
        UpdateText();

        UnfocusSearchField();

        if (keyboard != null)
            keyboard.HideKeyboard();

        if (monitorManager != null)
            monitorManager.HandleSearch(url);
    }

    void FocusSearchField()
    {
        if (focusRoutine != null)
            StopCoroutine(focusRoutine);

        isFocused = true;
        focusRoutine = StartCoroutine(FocusSequence());
    }

    void UnfocusSearchField()
    {
        isFocused = false;

        if (focusRoutine != null)
        {
            StopCoroutine(focusRoutine);
            focusRoutine = null;
        }

        HideQuickLinksInstant();
        MoveTo(baseLocalPosition);
    }

    IEnumerator FocusSequence()
    {
        HideQuickLinksInstant();

        if (!isFocused) yield break;

        yield return MoveTo(baseLocalPosition + new Vector3(0f, focusMoveY, 0f));
        if (focusDelay > 0f)
            yield return new WaitForSeconds(focusDelay);

        if (!isFocused) yield break;

        for (int i = 0; i < quickLinks.Length; i++)
        {
            if (!isFocused) yield break;

            if (quickLinks[i] != null)
                quickLinks[i].SetActive(true);

            if (i < quickLinks.Length - 1)
                yield return new WaitForSeconds(linkAppearInterval);
        }

        focusRoutine = null;
    }

    IEnumerator MoveTo(Vector3 targetLocalPos)
    {
        if (moveTarget == null) yield break;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        Vector3 start = moveTarget.localPosition;
        float t = 0f;

        while (t < focusMoveDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / Mathf.Max(0.0001f, focusMoveDuration));
            moveTarget.localPosition = Vector3.Lerp(start, targetLocalPos, lerp);
            yield return null;
        }

        moveTarget.localPosition = targetLocalPos;
        moveRoutine = null;
    }

    void HideQuickLinksInstant()
    {
        if (quickLinks == null) return;

        for (int i = 0; i < quickLinks.Length; i++)
        {
            if (quickLinks[i] != null)
                quickLinks[i].SetActive(false);
        }
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
