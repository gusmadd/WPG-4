using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class M_TutorialManager : MonoBehaviour
{
    [Header("VN UI")]
    public VNTextController vn;
    public TMP_Text catLineText;

    [Header("Cat Animator")]
    public Animator catAnimator;
    public string nodTrigger = "Nod";
    public string thinkTrigger = "Think";
    public string clickTrigger = "Click";
    public float clickToThinkDelay = 0.12f;

    [Header("Task UI")]
    public GameObject taskPanel;
    public Animator taskAnimator;
    public string taskShowTrigger = "Show";

    [Header("Guide Highlight")]
    public GameObject ghPower;
    public GameObject ghMeawser;
    public GameObject ghHomeSearch;
    public GameObject ghLink;
    public GameObject ghProduct;
    public GameObject ghFood;
    public GameObject ghItem;
    public GameObject ghBuy;

    [Header("Power Button")]
    public Collider2D powerCollider;
    public SpriteRenderer powerSprite;
    public Sprite powerOffSprite;
    public Sprite powerOnSprite;

    [Header("Browser")]
    public Collider2D browserCollider;

    [Header("Search UI")]
    public Collider2D homeSearchCollider;
    public Collider2D resultLinkCollider;
    public M_TutorialSearchField tutorialSearchField;

    [Header("Shop Colliders")]
    public Collider2D productCollider;
    public Collider2D foodCollider;
    public Collider2D itemCollider;

    [Header("Buy")]
    public Collider2D buyCollider;
    public SpriteRenderer buySprite;
    public float holdToBuySeconds = 2f;
    public GameObject buySuccessPage;

    [Header("Buy Success End")]
    [TextArea] public string continueMessage = "You already know what needs to be done. Press Enter to continue the first week's task.";
    public float continueMessageDelay = 1f;
    public string nextSceneName = "InGame";

    [Header("Pages")]
    public GameObject screenOff;
    public GameObject screenOn;
    public GameObject desktopPage;
    public GameObject homeSearchPage;
    public GameObject searchResultPage;
    public GameObject pawshopPage;
    public GameObject productPage;
    public GameObject foodPage;
    public GameObject detailItemPage;

    [Header("Loading")]
    public Animator loadingAnimator;
    public GameObject loadingCircle;
    public float loadingDuration = 2f;
    public float delayAfterOut = 0.1f;

    [Header("Loading Animation (Frame)")]
    public Sprite[] loadingFrames;
    public float frameRate = 10f;

    private SpriteRenderer loadingRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    bool waitingPowerClick = false;
    bool powerClicked = false;

    bool waitingBrowserClick = false;
    bool browserClicked = false;

    bool waitingHomeSearchClick = false;
    bool homeSearchClicked = false;

    bool waitingResultLinkClick = false;
    bool resultLinkClicked = false;

    bool waitingProductClick = false;
    bool productClicked = false;

    bool waitingFoodClick = false;
    bool foodClicked = false;

    bool waitingItemClick = false;
    bool itemClicked = false;

    bool waitingBuyHold = false;
    bool buyCompleted = false;
    bool isHoldingBuy = false;
    float holdTimer = 0f;

    Material runtimeBuyMat;
    static readonly int FillAmountID = Shader.PropertyToID("_FillAmount");

    void Awake()
    {
        if (buySprite != null && buySprite.sharedMaterial != null)
        {
            runtimeBuyMat = new Material(buySprite.sharedMaterial);
            buySprite.material = runtimeBuyMat;
        }
    }

    void Start()
    {
        SetupInitialState();
        StartCoroutine(IntroSequence());
    }

    void OnDestroy()
    {
        if (runtimeBuyMat != null)
            Destroy(runtimeBuyMat);
    }

    void Update()
    {
        if (loadingCircle != null && loadingCircle.activeSelf && loadingFrames != null && loadingFrames.Length > 0)
        {
            frameTimer += Time.deltaTime;

            if (frameRate > 0 && frameTimer >= 1f / frameRate)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % loadingFrames.Length;

                if (loadingRenderer != null)
                    loadingRenderer.sprite = loadingFrames[currentFrame];
            }
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!waitingBuyHold)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (waitingPowerClick)
                {
                    if (powerCollider != null && powerCollider.OverlapPoint(mousePos))
                    {
                        M_AudioManager.Instance?.PlayCursorClick();
                        powerClicked = true;
                        return;
                    }
                }

                if (waitingBrowserClick)
                {
                    if (browserCollider != null && browserCollider.OverlapPoint(mousePos))
                    {
                        M_AudioManager.Instance?.PlayCursorClick();
                        browserClicked = true;
                        return;
                    }
                }

                if (waitingHomeSearchClick)
                {
                    if (homeSearchCollider != null && homeSearchCollider.OverlapPoint(mousePos))
                    {
                        M_AudioManager.Instance?.PlayCursorClick();
                        homeSearchClicked = true;
                        return;
                    }
                }

                if (waitingResultLinkClick)
                {
                    if (resultLinkCollider != null && resultLinkCollider.OverlapPoint(mousePos))
                    {
                        M_AudioManager.Instance?.PlayCursorClick();
                        resultLinkClicked = true;
                        return;
                    }
                }

                if (waitingProductClick)
                {
                    if (productCollider != null && productCollider.OverlapPoint(mousePos))
                    {
                        M_AudioManager.Instance?.PlayCursorClick();
                        productClicked = true;
                        return;
                    }
                }

                if (waitingFoodClick)
                {
                    if (foodCollider != null && foodCollider.OverlapPoint(mousePos))
                    {
                        M_AudioManager.Instance?.PlayCursorClick();
                        foodClicked = true;
                        return;
                    }
                }

                if (waitingItemClick)
                {
                    if (itemCollider != null && itemCollider.OverlapPoint(mousePos))
                    {
                        M_AudioManager.Instance?.PlayCursorClick();
                        itemClicked = true;
                        return;
                    }
                }
            }
        }

        HandleBuyHold(mousePos);
    }

    void HandleBuyHold(Vector2 mousePos)
    {
        if (!waitingBuyHold) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (buyCollider != null && buyCollider.OverlapPoint(mousePos))
            {
                StartHoldBuy();
                return;
            }
        }

        if (Input.GetMouseButton(0) && isHoldingBuy)
        {
            if (buyCollider == null || !buyCollider.OverlapPoint(mousePos))
            {
                CancelHoldBuy();
                return;
            }

            holdTimer += Time.deltaTime;
            UpdateBuyFill();

            if (holdTimer >= holdToBuySeconds)
            {
                CompleteBuy();
                return;
            }
        }

        if (Input.GetMouseButtonUp(0) && isHoldingBuy)
        {
            CancelHoldBuy();
        }
    }

    void SetupInitialState()
    {
        if (taskPanel != null) taskPanel.SetActive(false);

        if (ghPower != null) ghPower.SetActive(false);
        if (ghMeawser != null) ghMeawser.SetActive(false);
        if (ghHomeSearch != null) ghHomeSearch.SetActive(false);
        if (ghLink != null) ghLink.SetActive(false);
        if (ghProduct != null) ghProduct.SetActive(false);
        if (ghFood != null) ghFood.SetActive(false);
        if (ghItem != null) ghItem.SetActive(false);
        if (ghBuy != null) ghBuy.SetActive(false);

        if (screenOff != null) screenOff.SetActive(true);
        if (screenOn != null) screenOn.SetActive(false);

        if (desktopPage != null) desktopPage.SetActive(false);
        if (homeSearchPage != null) homeSearchPage.SetActive(false);
        if (searchResultPage != null) searchResultPage.SetActive(false);
        if (pawshopPage != null) pawshopPage.SetActive(false);
        if (productPage != null) productPage.SetActive(false);
        if (foodPage != null) foodPage.SetActive(false);
        if (detailItemPage != null) detailItemPage.SetActive(false);
        if (buySuccessPage != null) buySuccessPage.SetActive(false);

        if (loadingCircle != null) loadingCircle.SetActive(false);
        if (loadingCircle != null)
            loadingRenderer = loadingCircle.GetComponent<SpriteRenderer>();

        if (powerSprite != null && powerOffSprite != null)
            powerSprite.sprite = powerOffSprite;

        if (catLineText != null)
            catLineText.text = "";

        if (tutorialSearchField != null)
            tutorialSearchField.DeactivateField();

        ResetHoldState();
    }

    IEnumerator IntroSequence()
    {
        PlayCatThink();
        ShowCatLine("(thinking)");
        yield return new WaitForSecondsRealtime(0.4f);

        var lines1 = new List<VNTextController.Line>
        {
            new VNTextController.Line("Narrator", "Feeling abandoned in your owner friend's house? Not getting the good food or toys?")
        };
        yield return PlayVN(lines1);

        var lines2 = new List<VNTextController.Line>
        {
            new VNTextController.Line("Narrator", "Do you have something that you want to get there?")
        };
        yield return PlayVN(lines2);

        var lines3 = new List<VNTextController.Line>
        {
            new VNTextController.Line("Narrator", "I know it's kinda hard thinking with that head. Try to remember this, you'll get reminder each time you got ONE ORDER RIGHT.")
        };
        yield return PlayVN(lines3);
        yield return new WaitForSecondsRealtime(clickToThinkDelay);

        PlayCatNod();
        ShowCatLine("(nodding)");

        yield return ShowTaskAndWaitForSpace();
        yield return PowerGuideSequence();
        yield return BrowserGuideSequence();
        yield return HomeSearchGuideSequence();
        yield return SearchResultGuideSequence();
        yield return ResultLinkGuideSequence();
        yield return ProductGuideSequence();
        yield return FoodGuideSequence();
        yield return ItemGuideSequence();
        yield return BuyGuideSequence();
        yield return BuySuccessEndSequence();
    }

    IEnumerator PlayVN(List<VNTextController.Line> lines)
    {
        if (vn == null) yield break;

        bool done = false;

        PlayCatThink();
        ShowCatLine("(thinking)");

        vn.PlayLines(lines, () => done = true);

        while (!done)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlayCatClick();
                yield return new WaitForSecondsRealtime(clickToThinkDelay);

                if (!done)
                {
                    PlayCatThink();
                    ShowCatLine("(thinking)");
                }
            }

            yield return null;
        }
    }

    IEnumerator ShowTaskAndWaitForSpace()
    {
        if (taskPanel != null)
            taskPanel.SetActive(true);

        if (taskAnimator != null)
            taskAnimator.SetTrigger(taskShowTrigger);

        yield return new WaitForSecondsRealtime(0.1f);

        while (!Input.GetMouseButtonDown(0))
            yield return null;

        if (taskPanel != null)
            taskPanel.SetActive(false);
    }

    IEnumerator PowerGuideSequence()
    {
        if (ghPower != null)
            ghPower.SetActive(true);

        waitingPowerClick = true;
        powerClicked = false;

        yield return new WaitUntil(() => powerClicked);

        waitingPowerClick = false;

        if (ghPower != null)
            ghPower.SetActive(false);

        yield return StartCoroutine(PowerOnSequence());
    }

    IEnumerator PowerOnSequence()
    {
        if (loadingAnimator != null)
            loadingAnimator.SetTrigger("isIn");

        yield return new WaitForSecondsRealtime(0.1f);

        currentFrame = 0;
        frameTimer = 0f;

        if (loadingRenderer != null && loadingFrames != null && loadingFrames.Length > 0)
            loadingRenderer.sprite = loadingFrames[0];

        if (loadingCircle != null)
            loadingCircle.SetActive(true);

        yield return new WaitForSecondsRealtime(loadingDuration);

        if (loadingCircle != null)
            loadingCircle.SetActive(false);
        currentFrame = 0;
        frameTimer = 0f;

        if (loadingAnimator != null)
            loadingAnimator.SetTrigger("isOut");

        yield return new WaitForSecondsRealtime(delayAfterOut);

        if (screenOff != null)
            screenOff.SetActive(false);

        if (screenOn != null)
            screenOn.SetActive(true);

        if (desktopPage != null)
            desktopPage.SetActive(true);

        if (powerSprite != null && powerOnSprite != null)
            powerSprite.sprite = powerOnSprite;
    }

    IEnumerator BrowserGuideSequence()
    {
        if (ghMeawser != null)
            ghMeawser.SetActive(true);

        waitingBrowserClick = true;
        browserClicked = false;

        yield return new WaitUntil(() => browserClicked);

        waitingBrowserClick = false;

        if (ghMeawser != null)
            ghMeawser.SetActive(false);

        OpenBrowserHome();
    }

    void OpenBrowserHome()
    {
        if (desktopPage != null)
            desktopPage.SetActive(false);

        if (homeSearchPage != null)
            homeSearchPage.SetActive(true);
    }

    IEnumerator HomeSearchGuideSequence()
    {
        if (ghHomeSearch != null)
            ghHomeSearch.SetActive(true);

        if (tutorialSearchField != null)
            tutorialSearchField.ActivateField();

        waitingHomeSearchClick = true;
        homeSearchClicked = false;

        yield return new WaitUntil(() => homeSearchClicked);

        waitingHomeSearchClick = false;

        if (ghHomeSearch != null)
            ghHomeSearch.SetActive(false);

        if (tutorialSearchField != null)
            tutorialSearchField.OnFieldClicked();

        yield return new WaitUntil(() =>
            tutorialSearchField != null && tutorialSearchField.isSubmitted
        );
    }

    IEnumerator SearchResultGuideSequence()
    {
        if (homeSearchPage != null)
            homeSearchPage.SetActive(false);

        if (searchResultPage != null)
            searchResultPage.SetActive(true);

        if (ghLink != null)
            ghLink.SetActive(true);

        yield return null;
    }

    IEnumerator ResultLinkGuideSequence()
    {
        waitingResultLinkClick = true;
        resultLinkClicked = false;

        yield return new WaitUntil(() => resultLinkClicked);

        waitingResultLinkClick = false;

        if (ghLink != null)
            ghLink.SetActive(false);

        OpenPawshopPage();

        if (ghProduct != null)
            ghProduct.SetActive(true);
    }

    void OpenPawshopPage()
    {
        if (searchResultPage != null)
            searchResultPage.SetActive(false);

        if (pawshopPage != null)
            pawshopPage.SetActive(true);
    }

    IEnumerator ProductGuideSequence()
    {
        waitingProductClick = true;
        productClicked = false;

        yield return new WaitUntil(() => productClicked);

        waitingProductClick = false;

        if (ghProduct != null)
            ghProduct.SetActive(false);

        OpenProductPage();

        if (ghFood != null)
            ghFood.SetActive(true);
    }

    void OpenProductPage()
    {
        if (pawshopPage != null)
            pawshopPage.SetActive(false);

        if (productPage != null)
            productPage.SetActive(true);
    }

    IEnumerator FoodGuideSequence()
    {
        waitingFoodClick = true;
        foodClicked = false;

        yield return new WaitUntil(() => foodClicked);

        waitingFoodClick = false;

        if (ghFood != null)
            ghFood.SetActive(false);

        OpenFoodPage();

        if (ghItem != null)
            ghItem.SetActive(true);
    }

    void OpenFoodPage()
    {
        if (productPage != null)
            productPage.SetActive(false);

        if (foodPage != null)
            foodPage.SetActive(true);
    }

    IEnumerator ItemGuideSequence()
    {
        waitingItemClick = true;
        itemClicked = false;

        yield return new WaitUntil(() => itemClicked);

        waitingItemClick = false;

        if (ghItem != null)
            ghItem.SetActive(false);

        OpenDetailItemPage();

        if (ghBuy != null)
            ghBuy.SetActive(true);
    }

    void OpenDetailItemPage()
    {
        if (foodPage != null)
            foodPage.SetActive(false);

        if (detailItemPage != null)
            detailItemPage.SetActive(true);
    }

    IEnumerator BuyGuideSequence()
    {
        waitingBuyHold = true;
        buyCompleted = false;
        ResetHoldState();

        yield return new WaitUntil(() => buyCompleted);

        waitingBuyHold = false;

        if (ghBuy != null)
            ghBuy.SetActive(false);

        if (detailItemPage != null)
            detailItemPage.SetActive(false);

        if (buySuccessPage != null)
            buySuccessPage.SetActive(true);
    }

    IEnumerator BuySuccessEndSequence()
    {
        yield return new WaitForSecondsRealtime(continueMessageDelay);

        var endLines = new List<VNTextController.Line>
        {
            new VNTextController.Line("Narrator", continueMessage)
        };

        yield return PlayVN(endLines);

        PlayCatNod();
        ShowCatLine("(nodding)");

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        if (vn != null)
            vn.HideInstant();

        yield return new WaitForSecondsRealtime(1f);

        M_ProgressManager.CompleteTutorial();
        SceneManager.LoadScene(nextSceneName);
    }

    void StartHoldBuy()
    {
        isHoldingBuy = true;
        holdTimer = 0f;
        UpdateBuyFill();
    }

    void CancelHoldBuy()
    {
        ResetHoldState();
    }

    void CompleteBuy()
    {
        buyCompleted = true;
        ResetHoldState();
    }

    void UpdateBuyFill()
    {
        if (runtimeBuyMat == null) return;

        float t = Mathf.Clamp01(holdTimer / Mathf.Max(0.01f, holdToBuySeconds));
        runtimeBuyMat.SetFloat(FillAmountID, t);
    }

    void ResetHoldState()
    {
        isHoldingBuy = false;
        holdTimer = 0f;

        if (runtimeBuyMat != null)
            runtimeBuyMat.SetFloat(FillAmountID, 0f);
    }

    void ShowCatLine(string s)
    {
        if (catLineText != null)
            catLineText.text = s;
    }

    void PlayCatThink()
    {
        if (catAnimator == null) return;

        catAnimator.ResetTrigger(nodTrigger);
        catAnimator.ResetTrigger(clickTrigger);
        catAnimator.SetTrigger(thinkTrigger);
    }

    void PlayCatClick()
    {
        if (catAnimator == null) return;

        catAnimator.ResetTrigger(thinkTrigger);
        catAnimator.ResetTrigger(nodTrigger);
        catAnimator.SetTrigger(clickTrigger);
    }

    void PlayCatNod()
    {
        if (catAnimator == null) return;

        catAnimator.ResetTrigger(thinkTrigger);
        catAnimator.ResetTrigger(clickTrigger);
        catAnimator.SetTrigger(nodTrigger);
    }
}