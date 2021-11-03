using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public static LevelExit instance;

    [SerializeField]
    private string nextLevel;

    public float levelEntranceWaitTime;
    public float levelExitWaitTime;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") { return; }

        GameManager.instance.levelStoppedOrEnded = true;

        PlayerPrefs.SetString(nextLevel + Checkpoint.CHECKPOINT_FORMAT, "");

        UIController.instance.ExitLevelFadeIn();
        AudioManager.instance.ExitLevelFadeIn();

        AudioManager.instance.LevelComplete();

        Invoke(nameof(LevelEnded), levelExitWaitTime);
    }

    private void LevelEnded()
    {
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
    }
}
