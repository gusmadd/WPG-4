using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_DetailFoodPage : MonoBehaviour
{
    [Header("Navigation")]
    public GameObject catFoodPage;
    public GameObject petshopHomePage;
    public GameObject searchPage;
    public M_SearchInput searchField;

    [Header("Buttons")]
    public Collider2D backCollider;
    public Collider2D homeCollider;
    public Collider2D closeCollider;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
                return;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 🔙 Back
            if (backCollider != null && backCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                catFoodPage.SetActive(true);
                Destroy(gameObject);
                return;
            }

            // 🏠 Home
            if (homeCollider != null && homeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                petshopHomePage.SetActive(true);
                Destroy(gameObject);
                return;
            }

            // ❌ Close
            if (closeCollider != null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                searchPage.SetActive(true);

                if (searchField != null)
                {
                    searchField.gameObject.SetActive(true);
                    searchField.ForceTyping();
                }

                Destroy(gameObject);
                return;
            }
        }
    }
}