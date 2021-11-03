using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField]
    AudioClip clip;

    private int id;

    void Start()
    {
        id = GetComponent<Id>().id;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.tag == "Player")) { return; }

        other.GetComponentInChildren<GunManager>().AddGun(id);

        AudioManager.instance.PlayClipAtPoint(clip, transform);

        Destroy(gameObject);
    }
}
