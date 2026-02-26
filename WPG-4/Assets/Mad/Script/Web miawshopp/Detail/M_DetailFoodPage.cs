using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_DetailFoodPage : MonoBehaviour
{
    [Header("Navigation")]
    public GameObject catFoodPage;
    public GameObject petshopHomePage;
    public GameObject desktopPage;
    public GameObject buySuccessPage;

    [Header("Buttons")]
    public Collider2D backCollider;
    public Collider2D homeCollider;
    public Collider2D closeCollider;
    public Collider2D buyCollider;

    void Update()
    {
        if (!gameObject.activeSelf) return;
        Debug.Log("M_FoodPage Update running");
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // BUY
            if (buyCollider != null && buyCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();

                if (buySuccessPage != null)
                    buySuccessPage.SetActive(true);

                gameObject.SetActive(false); // nonaktifkan halaman detail
                return;
            }

            // BACK → kembali ke list food
            if (backCollider != null && backCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();

                if (catFoodPage != null)
                    catFoodPage.SetActive(true);

                gameObject.SetActive(false); // nonaktifkan halaman detail
                return;
            }

            // HOME → ke homepage petshop
            if (homeCollider != null && homeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();

                if (petshopHomePage != null)
                    petshopHomePage.SetActive(true);
                gameObject.SetActive(false); // nonaktifkan halaman detail
                return;
            }

            // CLOSE → langsung ke desktop
            if (closeCollider != null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();

                if (desktopPage != null)
                    desktopPage.SetActive(true);

                gameObject.SetActive(false);
                return;
            }
        }
    }
}