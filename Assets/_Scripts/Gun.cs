using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to manage behaviour of a gun.
/// </summary>
public class Gun : MonoBehaviour
{
    #region Atributes

    public GameObject bullet;
    public Transform firePoint;
    public bool canAutoFire;
    public int zoomAmount;
    public GameObject fireMuzzle;

    [Range(0.1f, 1)]
    public float fireRate;

    [HideInInspector]
    public float fireCounter;

    public int maxAmmo = 10;
    public int currentAmmo;

    [SerializeField]
    public Transform gunTransform, normalPosition, zoomPosition;

    public AudioClip shootAudio;

    #endregion

    void Update()
    {
        if (fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }
    }

    public void GetAmmo(int val)
    {
        currentAmmo += val;
        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);
    }
}
