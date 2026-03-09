using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public TMP_Text narratorText;
    public Image backgroundImage;

    public GameObject task; // 1 object task

    string[] dialogs =
    {
        "Narrator: Feeling abandoned in your owner's house? Not getting good food or toys?",
        "Cat: ...",
        "Narrator: Do you have something that you want to get there?",
        "Cat: ...",
        "Narrator: I know it's kinda hard thinking with that head.",
        "Narrator: Try to remember this.",
        "Narrator: You'll get a reminder each time you get ONE ORDER RIGHT."
    };

    int dialogIndex = 0;
    bool taskShown = false;
    bool taskClosed = false;

    void Start()
    {
        SetAlpha(0);
        narratorText.text = dialogs[dialogIndex];

        task.SetActive(false);

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !taskShown)
        {
            NextDialog();
        }

        if (taskShown && !taskClosed && Input.GetKeyDown(KeyCode.LeftShift))
        {
            task.SetActive(false);
            taskClosed = true;
        }
    }

    void NextDialog()
    {
        dialogIndex++;

        if (dialogIndex < dialogs.Length)
        {
            narratorText.text = dialogs[dialogIndex];
        }
        else
        {
            StartCoroutine(FadeOut());
            ShowTask();
        }
    }

    void ShowTask()
    {
        taskShown = true;
        task.SetActive(true);
    }

    void SetAlpha(float a)
    {
        Color bg = backgroundImage.color;
        bg.a = a;
        backgroundImage.color = bg;

        Color txt = narratorText.color;
        txt.a = a;
        narratorText.color = txt;
    }

    IEnumerator FadeIn()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            SetAlpha(t);
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float t = 1;

        while (t > 0)
        {
            t -= Time.deltaTime;
            SetAlpha(t);
            yield return null;
        }
    }
}