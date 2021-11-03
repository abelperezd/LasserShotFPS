using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controlls the behaviour of the bullets (all types)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour
{
    #region Atributes

    private Rigidbody _rigidbody;

    [SerializeField]
    private float moveSpeed = 1, lifetime = 5;

    //VFX to play when collided
    [SerializeField]
    private GameObject impactEffect;

    [SerializeField]
    private int damage = 1;

    //can it hurt the enemies? And the player?
    [SerializeField]
    private bool damageEnemy, damagePlayer;
    
    #endregion

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = transform.forward * moveSpeed;
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damageEnemy)
        {
            if (other.gameObject.tag == "Enemy")
            {
                other.GetComponent<Health>().GetDamage(damage);
            }

            if (other.gameObject.tag == "Headshot")
            {
                other.transform.parent.GetComponent<Health>().GetDamage(damage * 2);
            }
        }
        else if (other.gameObject.tag == "Player" && damagePlayer)
        {
            other.GetComponent<Health>().GetDamage(damage);
        }
        else if (other.gameObject.tag == "Enemy" && damagePlayer)
            return;

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
        Instantiate(impactEffect, transform.position - transform.forward * moveSpeed * Time.deltaTime, transform.rotation);
    }
}
