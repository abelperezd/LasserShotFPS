using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// Script to manage the Player guns
/// </summary>
public class GunManager : MonoBehaviour
{
    #region Atributes

    [SerializeField]
    private List<Gun> guns;
    public List<Gun> disabledGuns;

    [SerializeField]
    private TextMeshProUGUI remainingBulletsText;
    [SerializeField]
    private Image remainingBulletsImage;

    private Transform cameraTransform;

    public Gun activeGun;
    private int currentGun = 0;

    //are we aiming?
    private bool isZoomed;

    CameraController cameraController;

    private bool gamePaused;
    
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        cameraController = cameraTransform.GetComponent<CameraController>();

        UpdateCanvas();

        activeGun = guns[currentGun];

        UIController.OnPauseStateChanged += HandleGamePaused;
    }

    private void OnDestroy()
    {
        UIController.OnPauseStateChanged -= HandleGamePaused;
    }

    private void HandleGamePaused(bool gamePaused)
    {
        this.gamePaused = gamePaused;
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePaused) { return; }

        if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 50f))
            {
                if (Vector3.Distance(cameraTransform.position, hit.point) > 2)
                    activeGun.firePoint.LookAt(hit.point);
            }
            else
            {
                activeGun.firePoint.LookAt(cameraTransform.position + (cameraTransform.forward * 30f));
            }
            FireShot();
        }
        else if (Input.GetMouseButton(0) && activeGun.canAutoFire && activeGun.fireCounter <= 0)
        {
                FireShot();
        }
        else if (activeGun.fireMuzzle.activeSelf)
        {
            activeGun.fireMuzzle.SetActive(false);
        }
        //switch gun
        if (Input.GetMouseButtonDown(2))
        {
            currentGun++;
            SwitchGun(currentGun);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Zoom();
        }
    }

    private void LateUpdate()
    {
        if (cameraController.isZooming)
        {
            InterpolateZoom();
        }
    }

    private void InterpolateZoom()
    {
        if (cameraController.zoomIn)
        {
            activeGun.gunTransform.position = Vector3.MoveTowards(activeGun.gunTransform.position, activeGun.zoomPosition.position, cameraController.zoomSpeed * Time.deltaTime);
        }
        else
        {
            activeGun.gunTransform.position = Vector3.MoveTowards(activeGun.gunTransform.position, activeGun.normalPosition.position, cameraController.zoomSpeed * Time.deltaTime);
        }
    }

    public void FireShot()
    {
        if (activeGun.currentAmmo <= 0) { return; }

        AudioManager.instance.Shoot(activeGun.shootAudio);

        activeGun.fireMuzzle.SetActive(true);
        activeGun.currentAmmo--;
        UpdateCanvas();
        Instantiate(activeGun.bullet, activeGun.firePoint.position, activeGun.firePoint.rotation);
        activeGun.fireCounter = activeGun.fireRate;
    }

    private void UpdateCanvas()
    {
        remainingBulletsText.text = activeGun.currentAmmo.ToString();
        remainingBulletsImage.fillAmount = (float)activeGun.currentAmmo / activeGun.maxAmmo;
    }

    public void IncreaseAmmo(int val)
    {
        activeGun.GetAmmo(val);
        UpdateCanvas();
    }

    private void SwitchGun(int gunIndex)
    {
        activeGun.gameObject.SetActive(false);
        currentGun = gunIndex;
        if (currentGun == guns.Count)
        {
            currentGun = 0;
        }
        activeGun = guns[currentGun];
        activeGun.gameObject.SetActive(true);
        UpdateCanvas();

        if (isZoomed)
        {
            Zoom();
        }
    }

    private void Zoom()
    {
        if (isZoomed)
        {
            Camera.main.transform.GetComponent<CameraController>().Zoom(activeGun.zoomAmount, false);
            isZoomed = false;
        }
        else
        {
            Camera.main.transform.GetComponent<CameraController>().Zoom(activeGun.zoomAmount, true);
            isZoomed = true;
        }
    }

    public void AddGun(int id)
    {
        foreach (Gun g in guns)
        {
            int i = g.transform.GetComponentInChildren<Id>().id;
            if (i == id)
            {
                g.GetAmmo(g.maxAmmo);
                SwitchGun(guns.IndexOf(g));
                return;
            }
        }

        foreach (Gun g in disabledGuns)
        {
            int i = g.transform.GetComponentInChildren<Id>().id;
            if(i == id)
            {
                guns.Add(g);
                disabledGuns.Remove(g);
                SwitchGun(guns.Count - 1);
                return;
            }
        }
    }
}
