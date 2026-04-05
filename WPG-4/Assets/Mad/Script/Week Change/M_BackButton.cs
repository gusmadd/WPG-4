using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_BackButton : MonoBehaviour
{
    [Header("Menu")]
    public string mainMenuScene= "MainMenu";
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
