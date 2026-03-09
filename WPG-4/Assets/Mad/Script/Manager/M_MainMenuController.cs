using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_MainMenuController : MonoBehaviour
{
 public string gameSceneName = "InGame";

    public void Play()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
