using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_KeyboardController : MonoBehaviour
{
    public Animator animator;
    public float animDuration = 0.25f;

    public bool isCaps = false;

    public void ShowKeyboard()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("isIn");
    }

    public void HideKeyboard()
    {
        StartCoroutine(HideRoutine());
    }

    IEnumerator HideRoutine()
    {
        animator.SetTrigger("isOut");
        yield return new WaitForSeconds(animDuration);
        gameObject.SetActive(false);
    }

    public void ToggleCaps()
    {
        isCaps = !isCaps;
    }
}
