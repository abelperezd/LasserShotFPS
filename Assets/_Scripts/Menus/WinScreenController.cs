using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenController : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void ButtonMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
}
