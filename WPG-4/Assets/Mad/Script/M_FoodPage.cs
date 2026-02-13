using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_FoodPage : MonoBehaviour
{
    [Header("Navigation")]
    public GameObject homePage; //halaman utama
    public GameObject searchPage; //search page
    public GameObject catFoodPage;   // 🔥 halaman makanan kucing

    [Header("Search")]
    public M_SearchInput searchField;

    [Header("Buttons")]
    public Collider2D backButtonCollider; // colider balik ke halam utama
    public Collider2D closeButtonCollider; // colider close
    public Collider2D catButtonCollider;   //  collider gambar kucing

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 🐱 CAT
            if (catButtonCollider != null && catButtonCollider.OverlapPoint(mousePos))
            {
                OpenCatFood();
                return;
            }

            // 🔙 BACK → ke homepage
            if (backButtonCollider != null && backButtonCollider.OverlapPoint(mousePos))
            {
                BackToHome();
                return;
            }

            // ❌ CLOSE → ke search page
            if (closeButtonCollider != null && closeButtonCollider.OverlapPoint(mousePos))
            {
                CloseToSearch();
                return;
            }
        }
    }

    void OpenCatFood()
    {
        gameObject.SetActive(false);

        if (catFoodPage != null)
            catFoodPage.SetActive(true);
    }

    void BackToHome()
    {
        gameObject.SetActive(false);

        if (homePage != null)
            homePage.SetActive(true);
    }

    void CloseToSearch()
    {
        gameObject.SetActive(false);

        if (searchPage != null)
            searchPage.SetActive(true);

        if (searchField != null)
        {
            searchField.gameObject.SetActive(true);
            searchField.ForceTyping();
        }
    }
}
