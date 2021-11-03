using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void ButtonStart()
    {
        SceneManager.LoadScene("Level 1");
    }

    /// <summary>
    /// Load the last level that was played
    /// </summary>
    public void ButtonContinue()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("CurrentLevel"));
        }
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }

}
