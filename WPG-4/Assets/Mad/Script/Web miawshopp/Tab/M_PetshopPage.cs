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

            if (closeButtonCollider != null && closeButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                CloseToDesktop();
                return;
            }

            if (productButtonCollider != null && productButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                OpenProductPage();
                return;
            }

            if (serviceButtonCollider != null && serviceButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
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
    }

    void OpenServicePage()
    {
        gameObject.SetActive(false);
        if (servicePage != null)
            servicePage.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        homePage.SetActive(true);
        productPage.SetActive(false);
        servicePage.SetActive(false);
    }

    void CloseToDesktop()
    {
        gameObject.SetActive(false);

        if (desktopPage != null)
            desktopPage.SetActive(true);
    }
}
