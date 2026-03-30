using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_MainMenuController : MonoBehaviour
{
 public string gameSceneName = "Tutorial";

    public void Play()
    {
        Debug.Log("Load scene: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
