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
    public float circleSpinSpeed = 180f;
    public GameObject loadingCircle;

    public GameObject screenOff;
    public GameObject screenOn;

    [Header("Power Button")]
    public SpriteRenderer powerSprite;
    public Sprite powerOffSprite;
    public Sprite powerOnSprite;

    [Header("Pages")]
    public GameObject desktopPage;
    public GameObject searchHomePage;
    public GameObject searchResultPage;
    public GameObject petshopPage;
    public M_NotFoundController notFoundController;
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

    void Start()
    {
        currentState = MonitorState.Off;

        screenOff.SetActive(true);
        screenOn.SetActive(false);
        loadingCircle.SetActive(false);

        desktopPage.SetActive(false);
        searchHomePage.SetActive(false);
        searchResultPage.SetActive(false);
        petshopPage.SetActive(false);

        if (powerSprite != null && powerOffSprite != null)
            powerSprite.sprite = powerOffSprite;
    }

    void Update()
    {
        if (loadingCircle.activeSelf)
        {
            loadingCircle.transform.Rotate(0, 0, -circleSpinSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
                return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 🔥 POWER BUTTON
            if (currentState == MonitorState.Off &&
                powerCollider != null &&
                powerCollider.OverlapPoint(mousePos))
            {
                PowerOn();
                return;
            }

            // 🔥 DESKTOP → OPEN BROWSER
            if (currentState == MonitorState.On)
            {
                // 🔥 CLOSE SEARCH PAGE
                if ((searchHomePage.activeSelf || searchResultPage.activeSelf) &&
                    closeSearchCollider != null &&
                    closeSearchCollider.OverlapPoint(mousePos))
                {
                    M_AudioManager.Instance?.PlayCursorClick();
                    CloseSearch();
                    return;
                }
                if (browserCollider != null &&
                    browserCollider.OverlapPoint(mousePos))
                {
                    M_AudioManager.Instance?.PlayCursorClick();
                    OpenBrowser();
                }
            }
        }
    }

    public void PowerOn()
    {
        if (currentState != MonitorState.Off) return;
        StartCoroutine(PowerOnSequence());
    }

    IEnumerator PowerOnSequence()
    {
        currentState = MonitorState.LoadingIn;
        loadingAnimator.SetTrigger("isIn");

        yield return new WaitUntil(() =>
            loadingAnimator.GetCurrentAnimatorStateInfo(0).IsName("in") &&
            loadingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        yield return new WaitForSeconds(0.1f);

        currentState = MonitorState.LoadingIdle;
        loadingCircle.SetActive(true);

        yield return new WaitForSeconds(loadingDuration);

        currentState = MonitorState.LoadingOut;
        loadingCircle.SetActive(false);
        loadingAnimator.SetTrigger("isOut");

        yield return new WaitForSeconds(delayAfterOut);

        // 🔥 MONITOR ON
        currentState = MonitorState.On;

        screenOff.SetActive(false);
        screenOn.SetActive(true);

        desktopPage.SetActive(true);
        searchHomePage.SetActive(false);
        searchResultPage.SetActive(false);

        // 🔥 GANTI SPRITE POWER
        if (powerSprite != null && powerOnSprite != null)
            powerSprite.sprite = powerOnSprite;
    }

    void OpenBrowser()
    {
        desktopPage.SetActive(false);
        searchHomePage.SetActive(true);
        searchResultPage.SetActive(false);
    }

    void CloseSearch()
    {
        searchHomePage.SetActive(false);
        searchResultPage.SetActive(false);
        petshopPage.SetActive(false);
        desktopPage.SetActive(true);

        if (homeSearchInput != null)
            homeSearchInput.ResetToDefault();
    }

    public void HandleSearch(string url)
    {
        string cleanUrl = url.ToLower().Trim();

        if (cleanUrl == "pawshopp" || cleanUrl == "pawshop" || cleanUrl == "petshop" || cleanUrl == "miawshopp")
        {
            searchHomePage.SetActive(false);
            searchResultPage.SetActive(true);

            if (resultSearchInput != null)
                resultSearchInput.SetTextFromExternal(url);

            if (searchPageController != null)
                searchPageController.GenerateResults();
        }
        else
        {
            searchHomePage.SetActive(false);
            searchResultPage.SetActive(false);
            notFoundController.Show();
        }
    }
    public void OpenPetshopFromResult()
    {
        searchResultPage.SetActive(false);
        petshopPage.SetActive(true);
    }
}
