using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public static string CHECKPOINT_FORMAT = "_cp";

    /// <summary>
    /// Check if there is a saved checkpoint
    /// </summary>
    void Start()
    {
        string name = SceneManager.GetActiveScene().name + CHECKPOINT_FORMAT;
        //have we arrived to this scene?
        if (PlayerPrefs.HasKey(name))
        {
            //is it the checkpoint saved?
            if (PlayerPrefs.GetString(name) == gameObject.name)
            {
                if (PlayerController.instance._characterController)
                {
                    PlayerController.instance._characterController.enabled = false;
                    PlayerController.instance.transform.position = transform.position;
                    PlayerController.instance._characterController.enabled = true;
                }
                else
                {
                    PlayerController.instance.gameObject.SetActive(false);
                    PlayerController.instance.transform.position = transform.position;
                    PlayerController.instance.gameObject.SetActive(true);
                }
                
            }
        }

        //if this scene has no checkpoint, go to the first one
        if (PlayerPrefs.GetString(name) == (""))
        {
            if (gameObject.name == "Checkpoint")
            {
                PlayerController.instance.gameObject.SetActive(false);
                PlayerController.instance.transform.position = transform.position;
                PlayerController.instance.gameObject.SetActive(true);
            }
        }


    }

    /// <summary>
    /// Save checkpoint when the player enters
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!(other.gameObject.tag == "Player")) { return; }
        AudioManager.instance.CheckPoint();
        string name = SceneManager.GetActiveScene().name + CHECKPOINT_FORMAT;
        PlayerPrefs.SetString(name, gameObject.name);
    }
}
