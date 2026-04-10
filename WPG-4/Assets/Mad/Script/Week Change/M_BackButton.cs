using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_BackButton : MonoBehaviour
{
    [Header("Menu")]
    public string mainMenuScene = "MainMenu";
    public void BackToMainMenu()
    {
        M_AudioManager.Instance?.PlayRandomUi();
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithTransition(mainMenuScene);
    }
}
