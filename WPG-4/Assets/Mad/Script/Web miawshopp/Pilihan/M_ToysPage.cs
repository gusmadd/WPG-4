using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ToysPage : MonoBehaviour
{
    [Header("Dekstop")]
    public GameObject desktopPage;
    public Collider2D closeCollider;

    [Header("Target Page")]
    public GameObject homePage;
    public GameObject servicePage;
    public GameObject productPage;

    [Header("Colliders")]
    public Collider2D homeCollider;
    public Collider2D serviceCollider;
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
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // close ke dekstop
            if (closeCollider != null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (desktopPage != null) 
                    desktopPage.SetActive(true);
                if (homeSearchInput != null) 
                    homeSearchInput.ResetToDefault();
                gameObject.SetActive(false);
                return;
            }
            // ke home page
            if (homeCollider !=null && homeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (homePage !=null) homePage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            // ke services page
            if (serviceCollider !=null && serviceCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (servicePage !=null) servicePage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            // kembali ke product page
            if (backCollider !=null && backCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                if (productPage !=null) productPage.SetActive(true);
                gameObject.SetActive(false);
                return;           
            }
        }
    }
}
