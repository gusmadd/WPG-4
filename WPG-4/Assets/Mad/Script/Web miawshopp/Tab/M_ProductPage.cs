using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ProductPage : MonoBehaviour
{
    [Header("Desktop")]
    public GameObject desktopPage;
    public Collider2D closeCollider;

    [Header("Target Pages")]
    public GameObject homePage;
    public GameObject servicesPage;
    public GameObject foodsPage;
    public GameObject petcarePage;
    public GameObject accessoriesPage;
    public GameObject toysPage;

    [Header("Colliders")]
    public Collider2D homeCollider;
    public Collider2D servicesCollider;
    public Collider2D foodsCollider;
    public Collider2D petcareCollider;
    public Collider2D accessoriesCollider;
    public Collider2D toysCollider;

    [Header ("Navigation")]
    public M_SearchInput homeSearchInput;

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // CLOSE ke desktop
            if (closeCollider != null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                if (homeSearchInput != null) homeSearchInput.ResetToDefault();
                CloseToDesktop();
                return;
            }

            if (homeCollider != null && homeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                    M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenPage(homePage);
                return;
            }

            if (servicesCollider != null && servicesCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenPage(servicesPage);
                return;
            }

            if (foodsCollider != null && foodsCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenPage(foodsPage);
                return;
            }

            if (petcareCollider != null && petcareCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenPage(petcarePage);
                return;
            }

            if (accessoriesCollider != null && accessoriesCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenPage(accessoriesPage);
                return;
            }

            if (toysCollider != null && toysCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenPage(toysPage);
                return;
            }
        }
    }

    void OpenPage(GameObject targetPage)
    {
        gameObject.SetActive(false);

        if (targetPage != null)
            targetPage.SetActive(true);
    }

    void CloseToDesktop()
    {
        gameObject.SetActive(false);

        if (desktopPage != null)
            desktopPage.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
