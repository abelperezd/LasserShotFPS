using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Atributes

    //to follow the player
    [SerializeField]
    Transform target;

    //to zoom in and out when aiming
    private float startFov, newFov;

    //are we zooming in or out?
    [HideInInspector]
    public bool isZooming;

    //to distinguish between zooming in and out
    [HideInInspector]
    public bool zoomIn;

    public float zoomSpeed = 1f;

    #endregion

    private void Start()
    {
        startFov = Camera.main.fieldOfView;
    }

    void Update()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        
    }

    /// <summary>
    /// Triggered from Gun Manager to zoom in or out
    /// </summary>
    /// <param name="value"> amount to zoom </param>
    /// <param name="zoomIn"> in or out? </param>
    public void Zoom(int value, bool zoomIn)
    {
        this.zoomIn = zoomIn;
        newFov = value;
        isZooming = true;
    }

    private void LateUpdate()
    {
        if (isZooming)
        {
            InterpolateZoom();
        }
    }

    /// <summary>
    /// Interpolate camera position depending on if it is zooming in or out
    /// </summary>
    private void InterpolateZoom()
    {
        if (zoomIn)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, startFov - newFov, zoomSpeed * Time.deltaTime);
            if (Camera.main.fieldOfView == newFov)
            {
                newFov = 0;
                isZooming = false;
            }
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, startFov, zoomSpeed * Time.deltaTime);
            if (Camera.main.fieldOfView == startFov)
            {
                newFov = 0;
                isZooming = false;
            }
        }
        
    }
}
