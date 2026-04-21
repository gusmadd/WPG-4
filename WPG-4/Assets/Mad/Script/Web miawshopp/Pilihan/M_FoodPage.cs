using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_FoodPage : MonoBehaviour
{
    [Header("Pages")]
    public GameObject homePage;
    public GameObject servicePage;
    public GameObject productPage;
    public GameObject desktopPage;

    [Header("Item Pages Prefab")]
    public GameObject item1DetailPrefab;
    public GameObject item2DetailPrefab;
    public GameObject item3DetailPrefab;
    public GameObject item4DetailPrefab;
    public GameObject item5DetailPrefab;
    public GameObject item6DetailPrefab;

    [Header("Item Sprites")]
    public SpriteRenderer item1Sprite;
    public SpriteRenderer item2Sprite;
    public SpriteRenderer item3Sprite;
    public SpriteRenderer item4Sprite;
    public SpriteRenderer item5Sprite;
    public SpriteRenderer item6Sprite;

    [Header("Buttons")]
    public Collider2D closeButtonCollider;
    public Collider2D homeButtonCollider;
    public Collider2D serviceButtonCollider;
    public Collider2D backButtonCollider;
    public Collider2D viewButtonCollider;

    [Header("Item Colliders")]
    public Collider2D item1Collider;
    public Collider2D item2Collider;
    public Collider2D item3Collider;
    public Collider2D item4Collider;
    public Collider2D item5Collider;
    public Collider2D item6Collider;

    [Header("View Button")]
    public GameObject viewButton;

    GameObject selectedItemPrefab = null;
    SpriteRenderer selectedSprite = null;
    public M_SearchInput homeSearchInput;

    void Awake()
    {
        if (viewButton != null) viewButton.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Mouse clicked at " + mousePos);
            if (closeButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                if (homeSearchInput != null) homeSearchInput.ResetToDefault();
                CloseToDesktop();
                return;
            }

            if (homeButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                GoToHome();
                return;
            }

            if (serviceButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                GoToService();
                return;
            }

            if (backButtonCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                M_PlayerController.Instance?.PlayTyping();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                BackToProduct();
                return;
            }

            if (viewButtonCollider != null && viewButtonCollider.OverlapPoint(mousePos))
            {
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                OpenSelectedItem();
                return;
            }

            if (item1Collider.OverlapPoint(mousePos)) SelectItem(item1Sprite, item1DetailPrefab);
            else if (item2Collider.OverlapPoint(mousePos)) SelectItem(item2Sprite, item2DetailPrefab);
            else if (item3Collider.OverlapPoint(mousePos)) SelectItem(item3Sprite, item3DetailPrefab);
            else if (item4Collider.OverlapPoint(mousePos)) SelectItem(item4Sprite, item4DetailPrefab);
            else if (item5Collider.OverlapPoint(mousePos)) SelectItem(item5Sprite, item5DetailPrefab);
            else if (item6Collider.OverlapPoint(mousePos)) SelectItem(item6Sprite, item6DetailPrefab);
        }
    }

    void SelectItem(SpriteRenderer sprite, GameObject prefab)
    {
        M_AudioManager.Instance?.PlayCursorClick();

        // Klik item yang sama → toggle warna
        if (selectedSprite == sprite)
        {
            // Kembalikan warna normal
            selectedSprite.color = Color.white;
            selectedSprite = null;
            // Tetap simpan prefab jika ingin view button tetap aktif
            selectedItemPrefab = prefab;
            if (viewButton != null)
                viewButton.SetActive(false); // matikan view button jika item tidak dipilih
            return;
        }

        // Klik item baru → reset warna semua item
        ResetAllItemColors();

        selectedSprite = sprite;
        selectedItemPrefab = prefab;

        if (selectedSprite != null)
            selectedSprite.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        if (viewButton != null)
            viewButton.SetActive(true);
    }

    void ResetAllItemColors()
    {
        item1Sprite.color = Color.white;
        item2Sprite.color = Color.white;
        item3Sprite.color = Color.white;
        item4Sprite.color = Color.white;
        item5Sprite.color = Color.white;
        item6Sprite.color = Color.white;
    }

    void OpenSelectedItem()
    {
        if (selectedItemPrefab == null)
        {
            Debug.LogError("Tidak ada item yang dipilih");
            return;
        }

        M_AudioManager.Instance?.PlayCursorClick();

        // Nonaktifkan halaman utama
        gameObject.SetActive(false);
        ResetAllItemColors();
        selectedSprite = null;

        // Aktifkan detail item di scene
        selectedItemPrefab.SetActive(true);
        TrackPageOpen("detail_food_page");
    }

    void GoToHome()
    {
        gameObject.SetActive(false);
        if (homePage != null) homePage.SetActive(true);
        TrackPageOpen("petshop_home_page");
    }

    void GoToService()
    {
        gameObject.SetActive(false);
        if (servicePage != null)
        {
            servicePage.SetActive(true);
            TrackPageOpen("service_page");
            Debug.Log("servicePage aktif? " + servicePage.activeSelf);
        }
    }

    void BackToProduct()
    {
        if (productPage != null)
        {
            productPage.SetActive(true);
            TrackPageOpen("product_page");
        }

        gameObject.SetActive(false);
    }

    void CloseToDesktop()
    {
        gameObject.SetActive(false);
        if (desktopPage != null) desktopPage.SetActive(true);
        TrackPageOpen("desktop");
    }
    void TrackPageOpen(string pageName)
    {
        int day = DayManager.Instance != null ? DayManager.Instance.GetCurrentDay() : 1;
        int week = DayManager.Instance != null ? DayManager.Instance.GetCurrentWeek() : 1;

        TelemetryManager.Instance?.SendPageOpen(pageName, day, week);
    }
}