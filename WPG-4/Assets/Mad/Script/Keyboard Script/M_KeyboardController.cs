using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_KeyboardController : MonoBehaviour
{
    public Animator animator;
    public float animDuration = 0.25f;

    public bool isCaps = false;

    [Header("Targets")]
    public M_SearchInput gameplaySearchField;
    public M_TutorialSearchField tutorialSearchField;

    public void ShowKeyboard()
    {
        M_AudioManager.Instance?.PlayShowKeyboard();
        gameObject.SetActive(true);

        if (animator != null)
            animator.SetTrigger("isIn");
    }

    public void HideKeyboard()
    {
        StartCoroutine(HideRoutine());
    }

    IEnumerator HideRoutine()
    {
        M_AudioManager.Instance?.PlayHideKeyboard();

        if (animator != null)
            animator.SetTrigger("isOut");

        yield return new WaitForSeconds(animDuration);
        gameObject.SetActive(false);
    }

    public void PressKey(string value)
    {
        if (tutorialSearchField != null && tutorialSearchField.isActive)
        {
            tutorialSearchField.AddCharacter(value);
            return;
        }

        if (gameplaySearchField != null)
        {
            gameplaySearchField.AddCharacter(value);
        }
    }

    public void ToggleCaps()
    {
        isCaps = !isCaps;
    }
}
