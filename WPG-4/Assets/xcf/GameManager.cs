using System.Collections;
using UnityEngine;

public class MonitorManager : MonoBehaviour
{
    public enum MonitorState
    {
        Off,
        Booting,
        On
    }

    [Header("Monitor State")]
    public MonitorState currentState = MonitorState.Off;

    [Header("Screens")]
    public GameObject screenOff;
    public GameObject screenOn;

    [Header("Pages")]
    public GameObject desktopPage;
    public GameObject searchPage;
    public GameObject petshopPage;

    [Header("Boot Animation")]
    public Animator loadingAnimator;
    public GameObject loadingCircle;
    public float circleSpinSpeed = 180f;

    [Header("Boot Timing")]
    public float bootDuration = 2f;

    [Header("Power Button")]
    public Collider2D powerCollider;

    void Start()
    {
        Debug.Log("MonitorManager START");

        SetMonitorOff();
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        // loading spin
        if (loadingCircle != null && loadingCircle.activeSelf)
        {
            loadingCircle.transform.Rotate(0, 0, -circleSpinSpeed * Time.deltaTime);
        }

        HandleClick();

        HandleGameState();
    }

    void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (Camera.main == null)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (currentState == MonitorState.Off &&
            powerCollider != null &&
            powerCollider.OverlapPoint(mousePos))
        {
            Debug.Log("Power Button Clicked");

            GameManager.Instance.PowerOn();
        }
    }

    void HandleGameState()
    {
        GameState state = GameManager.Instance.CurrentState;

        switch (state)
        {
            case GameState.FAILED:
                if (currentState != MonitorState.Off)
                    SetMonitorOff();
                break;

            case GameState.BOOTING:
                if (currentState == MonitorState.Off)
                    StartCoroutine(BootSequence());
                break;

            case GameState.DESKTOP:
                ShowDesktop();
                break;

            case GameState.BROWSER:
                ShowBrowser();
                break;
        }
    }

    void SetMonitorOff()
    {
        Debug.Log("Monitor OFF");

        currentState = MonitorState.Off;

        screenOff.SetActive(true);
        screenOn.SetActive(false);

        if (desktopPage) desktopPage.SetActive(false);
        if (searchPage) searchPage.SetActive(false);
        if (petshopPage) petshopPage.SetActive(false);
    }

    IEnumerator BootSequence()
    {
        currentState = MonitorState.Booting;

        Debug.Log("Monitor Booting...");

        screenOff.SetActive(false);
        screenOn.SetActive(true);

        if (loadingAnimator)
            loadingAnimator.SetTrigger("isIn");

        if (loadingCircle)
            loadingCircle.SetActive(true);

        yield return new WaitForSeconds(bootDuration);

        if (loadingCircle)
            loadingCircle.SetActive(false);

        if (loadingAnimator)
            loadingAnimator.SetTrigger("isOut");

        currentState = MonitorState.On;

        Debug.Log("Boot Finished");
    }

    void ShowDesktop()
    {
        if (desktopPage != null)
        {
            desktopPage.SetActive(true);
        }

        if (searchPage) searchPage.SetActive(false);
        if (petshopPage) petshopPage.SetActive(false);
    }

    void ShowBrowser()
    {
        if (desktopPage) desktopPage.SetActive(false);

        if (searchPage)
            searchPage.SetActive(true);
    }

    public void OpenPetshop()
    {
        if (searchPage)
            searchPage.SetActive(false);

        if (petshopPage)
            petshopPage.SetActive(true);
    }
}