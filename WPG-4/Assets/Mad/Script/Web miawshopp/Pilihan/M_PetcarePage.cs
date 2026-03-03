using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_PetcarePage : MonoBehaviour
{
    [Header("Dekstop")]
    public GameObject dekstopPage;
    public Collider2D dekstopCollider;

    [Header("Target Page")]
    public GameObject homePage;
    public GameObject servicesPage;
    public GameObject productsPage;

    [Header("Colliders")]
    public Collider2D homeCollider;
    public Collider2D servicesCollider;
    public Collider2D backCollider;

    [Header("Navigation")]
    public M_SearchInput homeSearchInput;

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // close ke dekstop
            if (dekstopCollider != null && dekstopCollider.OverlapPoint(mousepos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (dekstopPage != null) dekstopPage.SetActive(true);
                if (homeSearchInput != null) homeSearchInput.ResetToDefault();
                gameObject.SetActive(false);
                return;
            }
            // ke home page
            if (homeCollider != null && homeCollider.OverlapPoint(mousepos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (homePage != null) homePage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            // ke services page
            if (servicesCollider != null && servicesCollider.OverlapPoint(mousepos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (servicesPage != null) servicesPage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            // kembali ke products page
            if (backCollider != null && backCollider.OverlapPoint(mousepos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (productsPage != null) productsPage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
        } 
    }
}
