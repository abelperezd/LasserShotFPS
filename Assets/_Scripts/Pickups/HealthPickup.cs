using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField]
    private int health = 1;

    [SerializeField]
    AudioClip clip;

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.tag == "Player")) { return; }

        other.GetComponent<Health>().GetHealth(health);

        AudioManager.instance.PlayClipAtPoint(clip, transform);

        Destroy(gameObject);
    }
}
