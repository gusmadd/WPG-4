using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text narratorText;
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
    public GameObject searchPage;
    public GameObject petshopPage;
    public M_NotFoundController notFoundController;

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

    int dialogIndex = 0;
    bool taskShown = false;
    bool taskClosed = false;
    bool fading = false;

    // Hold power variables
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
        if (searchPage) searchPage.SetActive(false);
        if (petshopPage) petshopPage.SetActive(false);

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        // Dialog navigation
        if (Input.GetMouseButtonDown(0) && !taskShown && !fading)
        {
            NextDialog();
        }

        // Close task
        if (taskShown && !taskClosed && Input.GetKeyDown(KeyCode.LeftShift))
        {
            task.SetActive(false);
            taskClosed = true;
            StartPowerTutorial();
        }

        if (!pcOn)
            HandlePowerHold();
    }

    // ================= Dialog / Task =================
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

    // ================= Power Hold =================
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
            {
                isHoldingPower = true;
            }
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
            {
                PowerOnComplete();
            }
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

        narratorText.text = "PC is ON! Click the desktop icon to continue.";
    }

    // ================= Desktop / Browser / Search =================
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

        narratorText.text = "Browser opened. Type www.miawshopp.com to search!";
        if (browserIcon) browserIcon.SetActive(false);
        if (searchPage) searchPage.SetActive(true);
    }

    public void SearchWebsite(string url)
    {
        string cleanUrl = url.ToLower().Trim();
        if (cleanUrl == "www.miawshopp.com")
        {
            OpenPetshop();
        }
        else
        {
            if (searchPage) searchPage.SetActive(false);
            if (notFoundController) notFoundController.Show();
        }
    }

    void OpenPetshop()
    {
        narratorText.text = "Petshop opened! Click an item to buy it.";
        if (searchPage) searchPage.SetActive(false);
        if (petshopPage) petshopPage.SetActive(true);
    }

    // ================= Buy Hold =================
    public void HoldBuy(float holdDuration = 2f)
    {
        StartCoroutine(BuyHoldRoutine(holdDuration));
    }

    IEnumerator BuyHoldRoutine(float duration)
    {
        narratorText.text = "Hold the buy button until the order completes!";
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        FinishOrder();
    }

    void FinishOrder()
    {
        narratorText.text = "Order SUCCESS! 🎉 Tutorial completed!";
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndTutorial();
        }
    }

    // ================= Fade =================
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