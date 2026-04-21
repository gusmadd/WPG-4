using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_BuySucces : MonoBehaviour
{
    [Header("Navigation")]
    public GameObject petshopHomePage;
    public GameObject servicesPage;
    public GameObject desktopPage;

    [Header("Buttons")]
    public Collider2D closeCollider;
    public Collider2D homeCollider;
    public Collider2D servicesCollider;
    public Collider2D backCollider;

    [Header("Optional")]
    public M_SearchInput homeSearchInput;
    public GameObject backTargetPage; // halaman yang dibuka saat Back, kalau kosong hanya close page ini

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // CLOSE -> desktop
            if (closeCollider != null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();

                if (desktopPage != null)
                    desktopPage.SetActive(true);
                    TrackPageOpen("desktop");

                if (homeSearchInput != null)
                    homeSearchInput.ResetToDefault();

                gameObject.SetActive(false);
                return;
            }

            // HOME -> petshop home
            if (homeCollider != null && homeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                if (petshopHomePage != null)
                    petshopHomePage.SetActive(true);
                    TrackPageOpen("petshop_home_page");

                gameObject.SetActive(false);
                return;
            }

            // SERVICES -> services page
            if (servicesCollider != null && servicesCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                if (servicesPage != null)
                    servicesPage.SetActive(true);
                    TrackPageOpen("services_page");

                gameObject.SetActive(false);
                return;
            }

            // BACK -> kembali ke halaman sebelumnya (opsional)
            if (backCollider != null && backCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                if (backTargetPage != null)
                    backTargetPage.SetActive(true);
                    TrackPageOpen("product_page");

                gameObject.SetActive(false);
                return;
            }
        }
    }
    void TrackPageOpen(string pageName)
    {
        int day = DayManager.Instance != null ? DayManager.Instance.GetCurrentDay() : 1;
        int week = DayManager.Instance != null ? DayManager.Instance.GetCurrentWeek() : 1;

        TelemetryManager.Instance?.SendPageOpen(pageName, day, week);
    }
}
