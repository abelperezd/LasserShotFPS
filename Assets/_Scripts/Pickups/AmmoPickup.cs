using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField]
    AudioClip clip;

    [SerializeField]
    private int bullets = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.tag == "Player")) { return; }

        other.GetComponentInChildren<GunManager>().IncreaseAmmo(bullets);

        AudioManager.instance.PlayClipAtPoint(clip, transform);

        Destroy(gameObject);
    }
}
