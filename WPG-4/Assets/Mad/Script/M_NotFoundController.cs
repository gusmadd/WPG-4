using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class M_NotFoundController : MonoBehaviour
{
    public Animator animator;
    public float animDuration = 0.25f;

    public GameObject searchPage;
    public M_SearchInput searchField;
    public Collider2D closeButtonCollider;   // 🔥 collider tombol X

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

            if (closeButtonCollider.OverlapPoint(mousePos))
            {
                Close();
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        animator.ResetTrigger("isOut");
        animator.SetTrigger("isIn");
    }

    public void Close()
    {
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        animator.ResetTrigger("isIn");
        animator.SetTrigger("isOut");

        yield return new WaitForSeconds(animDuration);

        gameObject.SetActive(false);

        // Aktifkan kembali search page
        searchPage.SetActive(true);

        // Pastikan search field aktif
        if (!searchField.gameObject.activeSelf)
            searchField.gameObject.SetActive(true);

        yield return null;

        searchField.ForceTyping();
    }
}
