using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_CatFoodPage : MonoBehaviour
{
    [Header("Navigation")]
    public GameObject foodPage;          // 🔙 back ke food
    public GameObject petshopHomePage;   // 🏠 home petshop
    public GameObject searchPage;        // ❌ close ke search

    public M_SearchInput searchField;

    [Header("Buttons")]
    public Collider2D backCollider;      // 🔙
    public Collider2D homeCollider;      // 🏠
    public Collider2D closeCollider;     // ❌
    public Collider2D item1Collider;     // 🍗 item makanan

    [Header("Detail")]
    public GameObject detailPrefab;
    private GameObject currentDetail;

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

            // 🔥 kalau detail kebuka, jangan klik page
            if (currentDetail != null)
                return;

            // 🍗 ITEM
            if (item1Collider != null && item1Collider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                OpenDetail();
                return;
            }

            // 🔙 BACK
            if (backCollider != null && backCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                BackToFood();
                return;
            }

            // 🏠 HOME
            if (homeCollider != null && homeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                GoHome();
                return;
            }

            // ❌ CLOSE
            if (closeCollider != null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                CloseToSearch();
                return;
            }
        }
    }

    void OpenDetail()
    {
        if (detailPrefab != null)
        {
            currentDetail = Instantiate(detailPrefab);

            M_DetailFoodPage detailScript = currentDetail.GetComponent<M_DetailFoodPage>();

            detailScript.catFoodPage = gameObject;
            detailScript.petshopHomePage = petshopHomePage;
            detailScript.searchPage = searchPage;
            detailScript.searchField = searchField;

            gameObject.SetActive(false);
        }
    }


    public void CloseDetail()
    {
        if (currentDetail != null)
        {
            Destroy(currentDetail);
            currentDetail = null;
        }
    }

    void BackToFood()
    {
        gameObject.SetActive(false);
        if (foodPage != null)
            foodPage.SetActive(true);
    }

    void GoHome()
    {
        gameObject.SetActive(false);
        if (petshopHomePage != null)
            petshopHomePage.SetActive(true);
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
