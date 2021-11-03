using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controlls the damage caused by the explosion of the rockets.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Explosion : MonoBehaviour
{
    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private bool damageEnemy, damagePlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (damageEnemy)
        {
            if (other.gameObject.tag == "Enemy")
            {
                other.GetComponent<Health>().GetDamage(damage);
            }
        }
        else if (other.gameObject.tag == "Player" && damagePlayer)
        {
            other.GetComponent<Health>().GetDamage(damage);
        }
    }
}
