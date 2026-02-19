using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_PetshopPage : MonoBehaviour
{
    [Header("Back To Search")]
    public GameObject searchPage;
    public M_SearchInput searchField;
    public Collider2D closeButtonCollider; // colidder close X

    [Header("Pages")]
    public GameObject homePage;   // kategori utama
    public GameObject foodPage;   // halaman pilih hewan

    [Header("Buttons")]
    public Collider2D foodButtonCollider;   // collider tulisan FOOD

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
                return;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // ❌ tombol X
            if (closeButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                Close();
                return;
            }

            // 🍖 tombol FOOD
            if (foodButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                OpenFoodPage();
            }
        }
    }

    void OpenFoodPage()
    {
        homePage.SetActive(false);
        foodPage.SetActive(true);
    }

    public void BackToHome()
    {
        foodPage.SetActive(false);
        homePage.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        homePage.SetActive(true);
        foodPage.SetActive(false);
    }

    public void Close()
    {
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        gameObject.SetActive(false);

        searchPage.SetActive(true);

        yield return null; // 🔥 tunggu 1 frame

        if (!searchField.gameObject.activeSelf)
            searchField.gameObject.SetActive(true);

        yield return null; // 🔥 tunggu 1 frame lagi

        searchField.ForceTyping();
    }
}
