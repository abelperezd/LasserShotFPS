using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool levelStoppedOrEnded = false;

    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Set this level as the last one
        PlayerPrefs.SetString("CurrentLevel", SceneManager.GetActiveScene().name);

        UIController.OnPauseStateChanged += HandleGamePaused;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleGamePaused(bool gamePaused)
    {
        if (gamePaused)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void PlayerDied()
    {
        levelStoppedOrEnded = true;
        Invoke(nameof(ReloadScene), 2f);
        
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
