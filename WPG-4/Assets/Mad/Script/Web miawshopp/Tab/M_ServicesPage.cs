using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ServicesPage : MonoBehaviour
{
    [Header("Desktop")]
    public GameObject desktopPage;
    public Collider2D closeCollider;

    [Header("Targe Pos")]
    public GameObject homePage;
    public GameObject productPage;

    [Header ("Colliders")]
    public Collider2D homeCollider;
    public Collider2D productCollider;

    [Header("Navigation")]
    public M_SearchInput homeSearchInput;

    // Update is called once per frame
    void Update()
    {
        if(!gameObject.activeSelf) return;
        if(M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay) return;

        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Close ke Dekstop
            if(closeCollider !=null && closeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();

                if(desktopPage != null) desktopPage.SetActive(true);
                if(homeSearchInput != null) homeSearchInput.ResetToDefault();

                gameObject.SetActive(false);
                return;
            }
            //ke home page
            if(homeCollider != null && homeCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                if(homePage != null) homePage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            //ke Product page
            if(productCollider != null && productCollider.OverlapPoint(mousePos))
            {
                M_AudioManager.Instance?.PlayCursorClick();
                DayManager.Instance?.TryShowAdsFromPawshoppClick();
                if(productPage != null) productPage.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
        } 
    }
}
