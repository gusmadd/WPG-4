using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public TMPro.TMP_Text narratorText;
    public Image backgroundImage;
    public GameObject task;

    [Header("Power Button")]
    public Collider2D powerCollider;
    public Image powerProgressFill;
    public float holdTimeRequired = 2f;

    [Header("Pages & Screens")]
    public GameObject screenOff;
    public GameObject screenOn;
    public GameObject desktopIcon;
    public GameObject browserIcon;

    [Header("Dialog Settings")]
    public string[] dialogs = new string[]
    {
        "Narrator: Feeling abandoned in your owner's house? Not getting good food or toys?",
        "Cat: ...",
        "Narrator: Do you have something that you want to get there?",
        "Cat: ...",
        "Narrator: I know it's kinda hard thinking with that head.",
        "Narrator: Try to remember this.",
        "Narrator: You'll get a reminder each time you get ONE ORDER RIGHT."
    };

    [Header("Timing")]
    public float fadeSpeed = 1f;

    private int dialogIndex = 0;
    private bool taskShown = false;
    private bool taskClosed = false;
    private bool fading = false;
    private bool browserOpened = false;
    private bool isHoldingPower = false;
    private float holdTimer = 0f;
    private bool pcOn = false;

    void Start()
    {
        SetAlpha(0);
        narratorText.text = dialogs[dialogIndex];
        task.SetActive(false);

        if (screenOff) screenOff.SetActive(true);
        if (screenOn) screenOn.SetActive(false);
        if (desktopIcon) desktopIcon.SetActive(false);
        if (browserIcon) browserIcon.SetActive(false);

        StartCoroutine(FadeIn());
    }

    void Update()
{
    // 🔹 kalau browser sudah dibuka, tutorial stop total
    if (browserOpened)
        return;

    if (Input.GetMouseButtonDown(0))
    {
        if (!taskShown && !fading)
            NextDialog();

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject == desktopIcon)
                ClickDesktop();
            else if (hit.collider.gameObject == browserIcon)
                ClickBrowser();
        }
    }

    if (taskShown && !taskClosed && Input.GetKeyDown(KeyCode.LeftShift))
    {
        task.SetActive(false);
        taskClosed = true;
        StartPowerTutorial();
    }

    if (!pcOn)
        HandlePowerHold();
}
    void NextDialog()
    {
        dialogIndex++;
        if (dialogIndex < dialogs.Length)
            narratorText.text = dialogs[dialogIndex];
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

    void StartPowerTutorial()
    {
        narratorText.text = "Hold the power button to turn on the PC!";
        if (screenOff) screenOff.SetActive(true);
    }

    void HandlePowerHold()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (powerCollider != null && powerCollider.OverlapPoint(mousePos))
                isHoldingPower = true;
        }
        else
        {
            isHoldingPower = false;
            holdTimer = 0f;
            if (powerProgressFill) powerProgressFill.fillAmount = 0f;
        }

        if (isHoldingPower)
        {
            holdTimer += Time.deltaTime;
            if (powerProgressFill) powerProgressFill.fillAmount = Mathf.Clamp01(holdTimer / holdTimeRequired);

            if (holdTimer >= holdTimeRequired)
                PowerOnComplete();
        }
    }

    void PowerOnComplete()
    {
        pcOn = true;
        isHoldingPower = false;
        holdTimer = 0f;
        if (powerProgressFill) powerProgressFill.fillAmount = 1f;

        if (screenOff) screenOff.SetActive(false);
        if (screenOn) screenOn.SetActive(true);

        if (desktopIcon) desktopIcon.SetActive(true);
        if (browserIcon) browserIcon.SetActive(true);

        narratorText.text = "PC is ON! Desktop is ready!";
    }

    public void ClickDesktop()
    {
        if (!pcOn) return;

        narratorText.text = "Desktop opened. Click the browser to search!";
        if (desktopIcon) desktopIcon.SetActive(false);
        if (browserIcon) browserIcon.SetActive(true);
    }

   public void ClickBrowser()
{
    if (!pcOn) return;

    narratorText.text = "Browser opened. Click the search field to type!";
    if (browserIcon) browserIcon.SetActive(false);

    if (screenOn) screenOn.SetActive(false);

    browserOpened = true; // 🔹 STOP tutorial click detection
}

    // 🔹 FUNCTION BARU UNTUK SEARCH SUCCESS
    public void OnSearchSuccess()
    {
        narratorText.text = "Nice! You found PawShop. Let's buy something!";
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
        fading = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            SetAlpha(t);
            yield return null;
        }
        SetAlpha(1);
        fading = false;
    }

    IEnumerator FadeOut()
    {
        fading = true;
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * fadeSpeed;
            SetAlpha(t);
            yield return null;
        }
        SetAlpha(0);
        fading = false;
    }
}