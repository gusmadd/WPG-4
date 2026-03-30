using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VNTextController : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text speakerText;
    public TMP_Text messageText;

    [Header("Typing")]
    public float charDelay = 0.02f;

    Queue<Line> queue = new Queue<Line>();
    Coroutine typingCo;

    Line current;
    bool isTyping = false;
    bool lineFullyShown = false;
    public float clickCooldown = 0.12f;
    float clickBlockTimer = 0f;

    System.Action onQueueFinished;

    [System.Serializable]
    public struct Line
    {
        public string speaker;
        [TextArea] public string text;

        public Line(string speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }
    }

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (speakerText != null) speakerText.text = "";
        if (messageText != null) messageText.text = "";
    }

    void Update()
    {
        if (clickBlockTimer > 0f)
        {
            clickBlockTimer -= Time.unscaledDeltaTime;
            return;
        }
        if (panel == null || !panel.activeSelf) return;
        if (!Input.GetMouseButtonDown(0)) return;

        if (isTyping)
        {
            SkipTyping();
            clickBlockTimer = clickCooldown;
            return;
        }

        if (lineFullyShown)
        {
            ShowNextLine();
            return;
        }
    }

    public void PlayLines(List<Line> lines, System.Action onFinished = null)
    {
        if (lines == null || lines.Count == 0) return;

        onQueueFinished = onFinished;

        queue.Clear();
        for (int i = 0; i < lines.Count; i++)
            queue.Enqueue(lines[i]);

        if (panel != null) panel.SetActive(true);

        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }

        if (queue.Count == 0)
        {
            lineFullyShown = false;
            isTyping = false;

            if (panel != null) panel.SetActive(false);

            var cb = onQueueFinished;
            onQueueFinished = null;
            cb?.Invoke();
            return;
        }

        current = queue.Dequeue();

        if (speakerText != null) speakerText.text = current.speaker;
        if (messageText != null) messageText.text = "";

        isTyping = true;
        lineFullyShown = false;

        typingCo = StartCoroutine(TypeRoutine(current.text));
    }

    IEnumerator TypeRoutine(string line)
    {
        for (int i = 0; i < line.Length; i++)
        {
            if (messageText != null)
                messageText.text += line[i];

            yield return new WaitForSecondsRealtime(charDelay);
        }

        isTyping = false;
        lineFullyShown = true;
        typingCo = null;
    }

    void SkipTyping()
    {
        if (!isTyping) return;

        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }

        if (messageText != null)
            messageText.text = current.text;

        isTyping = false;
        lineFullyShown = true;
    }

    public void HideInstant()
    {
        if (typingCo != null) StopCoroutine(typingCo);

        typingCo = null;
        queue.Clear();
        isTyping = false;
        lineFullyShown = false;

        if (speakerText != null) speakerText.text = "";
        if (messageText != null) messageText.text = "";
        if (panel != null) panel.SetActive(false);

        onQueueFinished = null;
    }
}

