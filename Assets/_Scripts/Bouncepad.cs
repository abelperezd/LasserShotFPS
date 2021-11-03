using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bouncepads make the player jump whenever they are triggered
/// </summary>
public class Bouncepad : MonoBehaviour
{
    [SerializeField]
    private float bounceForce = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.tag == "Player")) { return; }

        PlayerController.instance.Bounce(bounceForce);
        AudioManager.instance.BouncePad(transform);
    }
}
