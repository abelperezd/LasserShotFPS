using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to control the turret behaviour
/// </summary>
public class Turret : MonoBehaviour
{
    #region Atributes

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private float rangeToFire, timeBetweenShots = .5f;

    private float shotCounter;

    public Transform gunBody, firePoint;

    [SerializeField]
    private float idleRotationSpeed = 20;

    #endregion

    void Start()
    {
        shotCounter = timeBetweenShots;
    }

    void Update()
    {
        if ((Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToFire) && !GameManager.instance.levelStoppedOrEnded)
        {
            gunBody.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 1.2f, 0f));

            shotCounter -= Time.deltaTime;

            if (shotCounter > 0) { return; }

            Instantiate(bullet, firePoint.position, firePoint.rotation);
            shotCounter = timeBetweenShots;
        }
        else
        {
            if (shotCounter != timeBetweenShots)
            {
                shotCounter = timeBetweenShots;
                gunBody.transform.rotation = Quaternion.Euler(0, gunBody.transform.rotation.y, 0);
            }
            gunBody.Rotate(Vector3.up * idleRotationSpeed * Time.deltaTime);
        }
    }
}
