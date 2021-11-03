using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script to controll the canvas.
/// </summary>
public class UIController : MonoBehaviour
{
    #region Atributes

    public static UIController instance;

    [SerializeField]
    GameObject inGamePanel, pausePanel;

    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Image sliderBar;

    [SerializeField]
    private Image damageImage;

    [SerializeField]
    private Animator endingLevelImageAnimator;

    [SerializeField]
    GameObject exitLevelPanel;

    [SerializeField]
    private float damageImageTimer = 0.5f;
    private float damageImageTimerCounter = 0f;

    private bool gamePaused;

    public static event Action<bool> OnPauseStateChanged;

    #endregion

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

    private void Start()
    {
        damageImageTimerCounter = damageImageTimer;

        StartLevelFadeOut();

        Health.OnPlayerHit += HandlePlayerHit;
    }

    private void HandlePlayerHit()
    {
        damageImage.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        Health.OnPlayerHit -= HandlePlayerHit;
    }

    void Update()
    {
        if (damageImage.gameObject.activeSelf)
        {
            FadeDamageImage();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
                ButtonResume();
            else
                PauseGame();
        }

    }

    private void StartLevelFadeOut()
    {
        if (!exitLevelPanel.activeSelf)
            exitLevelPanel.SetActive(true);
        endingLevelImageAnimator.SetFloat("Fade Speed", 1/LevelExit.instance.levelEntranceWaitTime);
    }

    public void ExitLevelFadeIn()
    {
        endingLevelImageAnimator.SetFloat("Fade Speed", 1/LevelExit.instance.levelExitWaitTime);
        endingLevelImageAnimator.SetTrigger("Fade In");
    }


    private void FadeDamageImage()
    {
        damageImageTimerCounter -= Time.deltaTime;

        damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, Mathf.MoveTowards(damageImage.color.a, 0, 1 / damageImageTimer * Time.deltaTime));

        if (damageImageTimerCounter <= 0)
        {
            damageImageTimerCounter = damageImageTimer;
            damageImage.gameObject.SetActive(false);
            damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, 0.25f);
        }
    }

    public void UpdateHealthSlider(int maxHealth, int currentHealth)
    {
        float currHealth = (float)currentHealth / maxHealth;
        healthSlider.value = currHealth;
        sliderBar.color = new Color((1 - currHealth), currHealth, 0);
    }

    private void PauseGame()
    {
        inGamePanel.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        gamePaused = true;
        OnPauseStateChanged?.Invoke(gamePaused);
    }

    #region Buttons

    public void ButtonMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void ButtonResume()
    {
        print("Resume");
        inGamePanel.SetActive(true);
        pausePanel.SetActive(false);
        gamePaused = false;
        Time.timeScale = 1;
        OnPauseStateChanged?.Invoke(gamePaused);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }

    #endregion

}
