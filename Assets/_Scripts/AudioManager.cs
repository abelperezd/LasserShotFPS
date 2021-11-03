using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is in charge of playing all the game sounds.
/// </summary>

[RequireComponent(typeof(AudioSource), typeof(Animator))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    #region Audio sources

    [SerializeField]
    private AudioSource _audioSourceMusic;

    public AudioSource _audioSourcePlayerMoves;

    [SerializeField]
    private AudioSource _audioSourcePlayerShoots;

    [SerializeField]
    private AudioSource _audioSourceSFX;

    #endregion

    //To fade in and out the music
    private Animator _audioAnimator;

    #region Clips
    [Space(10)]

    [SerializeField]
    private AudioClip music;

    #region Player

    [Space(10)]
    [SerializeField]
    private AudioClip walk;

    [SerializeField]
    private AudioClip run;

    [SerializeField]
    private AudioClip jump;

    [SerializeField]
    private AudioClip land;


    [Space(10)]
    [SerializeField]
    private AudioClip playerHurt;

    [SerializeField]
    private AudioClip playerDead;

    #endregion

    #region SFX

    [Space(10)]
    [SerializeField]
    private AudioClip checkPoint;

    [SerializeField]
    private AudioClip bouncePad;

    [SerializeField]
    private AudioClip levelComplete;

    [Space(10)]
    [SerializeField]
    private AudioClip explosion;

    #endregion

    #endregion

    #region Initializing

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
        Health.OnPlayerDies += HandlePlayerDies;
        UIController.OnPauseStateChanged += HandleGamePaused;

        _audioAnimator = GetComponent<Animator>();
        StartLevelFadeOut();
        _audioSourceMusic.clip = music;
        _audioSourceMusic.Play();
    }

    private void OnDestroy()
    {
        Health.OnPlayerDies -= HandlePlayerDies;
        UIController.OnPauseStateChanged -= HandleGamePaused;
    }

    #endregion

    #region Handlers

    private void HandlePlayerDies()
    {
        _audioSourceMusic.Stop();
    }

    private void HandleGamePaused(bool gamePaused)
    {
        if (gamePaused)
        {
            _audioSourceMusic.Pause();
        }
        else
        {
            _audioSourceMusic.Play();
        }
    }

    #endregion

    #region Player methods
    public void Walk()
    {
        _audioSourcePlayerMoves.PlayOneShot(walk);
    }

    public void Run()
    {
        _audioSourcePlayerMoves.PlayOneShot(run);
    }

    public void Jump()
    {
        _audioSourcePlayerMoves.PlayOneShot(jump);
    }

    public void Land()
    {
        _audioSourcePlayerMoves.PlayOneShot(land);
    }

    public void PlayerHurt()
    {
        _audioSourceSFX.PlayOneShot(playerHurt);
    }

    public void PlayerDead()
    {
        _audioSourceSFX.PlayOneShot(playerDead);
    }

    public void Shoot(AudioClip clip)
    {
        _audioSourcePlayerShoots.PlayOneShot(clip);
    }

    #endregion

    #region SFX methods

    public void CheckPoint()
    {
        _audioSourceSFX.PlayOneShot(checkPoint);
    }

    public void BouncePad(Transform trans)
    {
        AudioSource.PlayClipAtPoint(bouncePad, trans.position);
    }

    public void LevelComplete()
    {
        _audioSourceSFX.PlayOneShot(levelComplete);
    }

    public void SFX(AudioClip clip)
    {
        _audioSourceSFX.PlayOneShot(clip);
    }

    public void PlayClipAtPoint(AudioClip clip, Transform trans)
    {
        AudioSource.PlayClipAtPoint(clip, trans.position);
    }

    public void Explosion(Transform trans)
    {
        AudioSource.PlayClipAtPoint(explosion, trans.position);
    }

    #endregion

    #region Music fades

    /// <summary>
    /// Music fade out played when the scene starts.
    /// </summary>
    private void StartLevelFadeOut()
    {
        _audioAnimator.SetFloat("Fade Speed", 1 / LevelExit.instance.levelEntranceWaitTime);
    }

    /// <summary>
    /// Music fade in played when the scene starts.
    /// </summary>
    public void ExitLevelFadeIn()
    {
        _audioAnimator.SetFloat("Fade Speed", 1 / LevelExit.instance.levelExitWaitTime);
        _audioAnimator.SetTrigger("Fade In");
    }

    #endregion

}
