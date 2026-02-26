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

    void Awake()
    {
        gameObject.SetActive(false);
    }

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
                CloseToDesktop();
                return;
            }

            if (homeCollider != null && homeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                OpenPage(homePage);
                return;
            }

            if (servicesCollider != null && servicesCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                OpenPage(servicesPage);
                return;
            }

            if (foodsCollider != null && foodsCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                OpenPage(foodsPage);
                return;
            }

            if (petcareCollider != null && petcareCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                OpenPage(petcarePage);
                return;
            }

            if (accessoriesCollider != null && accessoriesCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                OpenPage(accessoriesPage);
                return;
            }

            if (toysCollider != null && toysCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
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
