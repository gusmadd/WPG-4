using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_MonitorManager : MonoBehaviour
{
    public enum MonitorState
    {
        Off,
        LoadingIn,
        LoadingIdle,
        LoadingOut,
        On
    }

    [Header("State")]
    public MonitorState currentState = MonitorState.Off;

    [Header("References")]
    public Animator loadingAnimator;

    [Header("Circle Spin")]
    public GameObject loadingCircle;
    public Sprite[] loadingFrames;
    public float frameRate = 10f;

    private SpriteRenderer loadingRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    public GameObject screenOff;
    public GameObject screenOn;

    [Header("Power Button")]
    public SpriteRenderer powerSprite;
    public Sprite powerOffSprite;
    public Sprite powerOnSprite;

    [Header("Lights")]
    public GameObject powerLight;     // nyala saat tombol power ON
    public GameObject monitorLight;   // nyala saat monitor sudah benar-benar ON

    [Header("Pages")]
    public GameObject desktopPage;
    public GameObject searchHomePage;
    public GameObject searchResultPage;
    public GameObject petshopPage;
    public M_SearchPage searchPageController;
    public M_SearchInput homeSearchInput;
    public M_SearchInput resultSearchInput;

    [Header("Desktop")]
    public Collider2D browserCollider;
    public Collider2D closeSearchCollider;

    [Header("Timing")]
    public float loadingDuration = 2f;
    public float delayAfterOut = 0.1f;

    [Header("Colliders")]
    public Collider2D powerCollider;

    [Header("Ads")]
    public M_AdsPopup[] adsPopups;

    public M_KeyboardController keyboard;

    void Start()
    {
        currentState = MonitorState.Off;

        if (screenOff != null) screenOff.SetActive(true);
        if (screenOn != null) screenOn.SetActive(false);
        if (loadingCircle != null) loadingCircle.SetActive(false);

        if (powerLight != null) powerLight.SetActive(false);
        if (monitorLight != null) monitorLight.SetActive(false);

        if (loadingCircle != null)
            loadingRenderer = loadingCircle.GetComponent<SpriteRenderer>();

        if (desktopPage != null) desktopPage.SetActive(false);
        if (searchHomePage != null) searchHomePage.SetActive(false);
        if (searchResultPage != null) searchResultPage.SetActive(false);
        if (petshopPage != null) petshopPage.SetActive(false);

        if (powerSprite != null && powerOffSprite != null)
            powerSprite.sprite = powerOffSprite;

        ResetLoadingVisual();
        ResetLoadingAnimator();
        DisableAllAds();
    }

    void Update()
    {
        if (loadingCircle != null && loadingCircle.activeSelf && loadingFrames != null && loadingFrames.Length > 0)
        {
            frameTimer += Time.deltaTime;

            if (frameRate > 0f && frameTimer >= 1f / frameRate)
            {
                frameTimer = 0f;

                currentFrame++;
                if (currentFrame >= loadingFrames.Length)
                    currentFrame = 0;

                if (loadingRenderer != null)
                    loadingRenderer.sprite = loadingFrames[currentFrame];
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (M_GameManager.Instance != null &&
                M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
                return;

            if (Camera.main == null) return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (currentState == MonitorState.Off &&
                powerCollider != null &&
                powerCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayPcButton();
                PowerOn();
                return;
            }

            if (M_GameManager.Instance != null &&
                M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
                return;

            if (currentState == MonitorState.On)
            {
                if ((searchHomePage != null && searchHomePage.activeSelf) ||
                    (searchResultPage != null && searchResultPage.activeSelf))
                {
                    if (closeSearchCollider != null && closeSearchCollider.OverlapPoint(mousePos))
                    {
                        M_AudioManager.Instance?.PlayCursorClick();
                        M_PlayerController.Instance?.PlayTyping();
                        CloseSearch();
                        return;
                    }
                }

                if (browserCollider != null && browserCollider.OverlapPoint(mousePos))
                {
                    M_AudioManager.Instance?.PlayCursorClick();
                    M_PlayerController.Instance?.PlayTyping();
                    OpenBrowser();
                    return;
                }
            }
        }
    }

    public void PowerOn()
    {
        if (currentState != MonitorState.Off) return;

        StopAllCoroutines();
        ResetLoadingVisual();
        ResetLoadingAnimator();

        StartCoroutine(PowerOnSequence());
    }

    IEnumerator PowerOnSequence()
    {
        if (powerSprite != null && powerOnSprite != null)
            powerSprite.sprite = powerOnSprite;

        // Light tombol power langsung nyala setelah sprite power ON
        if (powerLight != null)
            powerLight.SetActive(true);

        currentState = MonitorState.LoadingIn;

        if (loadingAnimator != null)
        {
            loadingAnimator.ResetTrigger("isOut");
            loadingAnimator.ResetTrigger("isIn");
            loadingAnimator.SetTrigger("isIn");
        }

        yield return new WaitForSecondsRealtime(0.1f);

        if (loadingAnimator != null)
        {
            yield return new WaitUntil(() =>
                loadingAnimator.GetCurrentAnimatorStateInfo(0).IsName("in") &&
                loadingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
            );
        }

        currentState = MonitorState.LoadingIdle;
        ResetLoadingVisual();

        if (loadingCircle != null)
            loadingCircle.SetActive(true);

        yield return new WaitForSecondsRealtime(loadingDuration);

        currentState = MonitorState.LoadingOut;

        if (loadingCircle != null)
            loadingCircle.SetActive(false);

        if (loadingAnimator != null)
        {
            loadingAnimator.ResetTrigger("isIn");
            loadingAnimator.ResetTrigger("isOut");
            loadingAnimator.SetTrigger("isOut");
        }

        yield return new WaitForSecondsRealtime(delayAfterOut);

        currentState = MonitorState.On;
        M_AudioManager.Instance?.PlayPcPowerOn();

        if (screenOff != null) screenOff.SetActive(false);
        if (screenOn != null) screenOn.SetActive(true);


        // Light monitor nyala saat monitor benar-benar ON
        if (monitorLight != null)
            monitorLight.SetActive(true);

        if (desktopPage != null) desktopPage.SetActive(true);
        TrackPageOpen("desktop");
        if (searchHomePage != null) searchHomePage.SetActive(false);
        if (searchResultPage != null) searchResultPage.SetActive(false);
        if (petshopPage != null) petshopPage.SetActive(false);
    }

    void OpenBrowser()
    {
        if (desktopPage != null) desktopPage.SetActive(false);
        if (searchHomePage != null) searchHomePage.SetActive(true);
        TrackPageOpen("search_home_page");
        if (searchResultPage != null) searchResultPage.SetActive(false);
    }

    void ShowRandomAds()
    {
        if (M_GameManager.Instance == null) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
            return;

        if (adsPopups == null || adsPopups.Length == 0) return;

        int pick = Random.Range(0, adsPopups.Length);

        for (int i = 0; i < adsPopups.Length; i++)
        {
            if (adsPopups[i] != null)
                adsPopups[i].gameObject.SetActive(false);
        }

        if (adsPopups[pick] != null)
            adsPopups[pick].gameObject.SetActive(true);
    }

    void DisableAllAds()
    {
        if (adsPopups == null) return;

        for (int i = 0; i < adsPopups.Length; i++)
        {
            if (adsPopups[i] != null)
                adsPopups[i].gameObject.SetActive(false);
        }
    }

    void CloseSearch()
    {
        if (keyboard != null)
            keyboard.HideKeyboard();

        if (searchHomePage != null) searchHomePage.SetActive(false);
        if (searchResultPage != null) searchResultPage.SetActive(false);
        if (petshopPage != null) petshopPage.SetActive(false);
        if (desktopPage != null) desktopPage.SetActive(true);
        TrackPageOpen("desktop");

        if (homeSearchInput != null)
            homeSearchInput.ResetToDefault();
    }

    public void HandleSearch(string url)
    {
        string cleanUrl = url.ToLower().Trim();

        if (cleanUrl == "pawshopp" || cleanUrl == "pawshop" || cleanUrl == "petshop" || cleanUrl == "miawshopp")
        {
            if (searchHomePage != null) searchHomePage.SetActive(false);
            if (searchResultPage != null) searchResultPage.SetActive(true);
            TrackPageOpen("search_result_page");

            if (resultSearchInput != null)
                resultSearchInput.SetTextFromExternal(url);

            if (searchPageController != null)
                searchPageController.GenerateResults();
        }
        else
        {
            if (searchHomePage != null) searchHomePage.SetActive(true);
            if (searchResultPage != null) searchResultPage.SetActive(false);

            ShowRandomAds();
        }
    }

    public void ShowRandomAdsFromExternal()
    {
        ShowRandomAds();
    }

    public void OpenPetshopFromResult()
    {
        if (desktopPage != null) desktopPage.SetActive(false);
        if (searchResultPage != null) searchResultPage.SetActive(false);
        if (petshopPage != null) petshopPage.SetActive(true);
        TrackPageOpen("petshop_home_page");
    }

    public void ResetToOff()
    {
        StopAllCoroutines();

        currentState = MonitorState.Off;

        ResetLoadingVisual();
        ResetLoadingAnimator();

        if (screenOff != null) screenOff.SetActive(true);
        if (screenOn != null) screenOn.SetActive(false);
        if (loadingCircle != null) loadingCircle.SetActive(false);

        if (powerLight != null) powerLight.SetActive(false);
        if (monitorLight != null) monitorLight.SetActive(false);

        if (desktopPage != null) desktopPage.SetActive(false);
        if (searchHomePage != null) searchHomePage.SetActive(false);
        if (searchResultPage != null) searchResultPage.SetActive(false);
        if (petshopPage != null) petshopPage.SetActive(false);

        if (homeSearchInput != null) homeSearchInput.ResetToDefault();
        if (resultSearchInput != null) resultSearchInput.ResetToDefault();

        if (keyboard != null)
            keyboard.HideKeyboard();

        DisableAllAds();

        if (powerSprite != null && powerOffSprite != null)
            powerSprite.sprite = powerOffSprite;
    }

    void ResetLoadingVisual()
    {
        currentFrame = 0;
        frameTimer = 0f;

        if (loadingRenderer != null && loadingFrames != null && loadingFrames.Length > 0)
            loadingRenderer.sprite = loadingFrames[0];
    }

    void ResetLoadingAnimator()
    {
        if (loadingAnimator == null) return;

        loadingAnimator.ResetTrigger("isIn");
        loadingAnimator.ResetTrigger("isOut");
        loadingAnimator.Rebind();
        loadingAnimator.Update(0f);
    }

    public void ForceCloseAllAdsForQTE()
    {
        if (adsPopups == null) return;

        for (int i = 0; i < adsPopups.Length; i++)
        {
            if (adsPopups[i] != null && adsPopups[i].gameObject.activeSelf)
                adsPopups[i].ForceCloseAdsInstant();
        }

        if (keyboard != null)
            keyboard.HideKeyboard();
    }
    void TrackPageOpen(string pageName)
    {
        int day = DayManager.Instance != null ? DayManager.Instance.GetCurrentDay() : 1;
        int week = DayManager.Instance != null ? DayManager.Instance.GetCurrentWeek() : 1;

        TelemetryManager.Instance?.SendPageOpen(pageName, day, week);
    }
}