using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_PetshopPage : MonoBehaviour
{
    [Header("Back To Desktop")]
    public GameObject desktopPage;

    [Header("Search")]
    public GameObject searchPage;
    public M_SearchInput searchField;
    public Collider2D closeButtonCollider;

    [Header("Pages")]
    public GameObject homePage;
    public GameObject productPage;
    public GameObject servicePage;

    [Header("Buttons")]
    public Collider2D productButtonCollider;
    public Collider2D serviceButtonCollider;

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (closeButtonCollider != null && closeButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                if (searchField != null) searchField.ResetToDefault();
                CloseToDesktop();
                return;
            }

            if (productButtonCollider != null && productButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenProductPage();
                return;
            }

            if (serviceButtonCollider != null && serviceButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenServicePage();
                return;
            }
        }
    }

    void OpenProductPage()
    {
        gameObject.SetActive(false);
        if (productPage != null)
            productPage.SetActive(true);
            TrackPageOpen("product_page");
    }

    void OpenServicePage()
    {
        gameObject.SetActive(false);
        if (servicePage != null)
            servicePage.SetActive(true);
            TrackPageOpen("service_page");
    }

    public void Show()
    {
        gameObject.SetActive(true);
        homePage.SetActive(true);
        productPage.SetActive(false);
        servicePage.SetActive(false);

        TrackPageOpen("petshop_home_page");
    }

    void CloseToDesktop()
    {
        gameObject.SetActive(false);

        if (desktopPage != null)
            desktopPage.SetActive(true);
            TrackPageOpen("desktop");
    }
    void TrackPageOpen(string pageName)
    {
        int day = DayManager.Instance != null ? DayManager.Instance.GetCurrentDay() : 1;
        int week = DayManager.Instance != null ? DayManager.Instance.GetCurrentWeek() : 1;

        TelemetryManager.Instance?.SendPageOpen(pageName, day, week);
    }
}
