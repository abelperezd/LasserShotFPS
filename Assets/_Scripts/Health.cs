using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To manage the health of the player and enemies
/// </summary>
public class Health : MonoBehaviour
{
    #region Atributes

    [SerializeField]
    private int maxPlayerHealth = 2;
    [SerializeField]
    private int maxEnemyHealth = 2;

    private int health = 1;

    [SerializeField]
    //after receiving one shoot
    private float invincibleTimeDuration = 1;
    private float invincibleTimeCounter;

    private bool isPlayer;

    private UIController uiController;

    public static event Action OnPlayerHit;
    public static event Action OnPlayerDies;

    private EnemyController _enemyController;

    #endregion

    private void Start()
    {
        if (GetComponent<PlayerController>())
        {
            isPlayer = true;
            uiController = UIController.instance;
            health = maxPlayerHealth;
            uiController.UpdateHealthSlider(maxPlayerHealth, health);
        }
        else
        {
            health = maxEnemyHealth;
            TryGetComponent<EnemyController>(out _enemyController);
        }
    }

    void Update()
    {
        if (!isPlayer || invincibleTimeCounter <= 0) { return; }

        invincibleTimeCounter -= Time.deltaTime;
    }

    public void GetDamage(int damage)
    {
        if (isPlayer)
        {
            if (invincibleTimeCounter > 0) { return; }

            OnPlayerHit?.Invoke();

            health -= damage;
            AudioManager.instance.PlayerHurt();
            invincibleTimeCounter = invincibleTimeDuration;
            if (health <= 0)
            {
                OnPlayerDies?.Invoke();
                gameObject.SetActive(false);
                GameManager.instance.PlayerDied();
                Invoke(nameof(PlayDeathSound), 0.5f);
            }
            uiController.UpdateHealthSlider(maxPlayerHealth, health);
        }
        else
        {
            health -= damage;

            if (_enemyController)
            {
                _enemyController.WasShot();
            }

            if (health <= 0)
            {
                AudioManager.instance.Explosion(transform);
                Destroy(gameObject);
            }
        }
    }

    private void PlayDeathSound()
    {
        AudioManager.instance.PlayerDead();
    }

    public void GetHealth(int val)
    {
        health += val;
        health = Mathf.Min(health, maxPlayerHealth);
        uiController.UpdateHealthSlider(maxPlayerHealth, health);
    }
}
