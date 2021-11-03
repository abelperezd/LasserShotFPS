using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to move the mobile turrets.
/// </summary>
public class MobileTurret : MonoBehaviour
{
    #region Atributes

    [SerializeField]
    private Transform rail;

    [SerializeField]
    private Turret turret;

    [SerializeField]
    private int velocity;

    #endregion

    void Update()
    {
        if (!turret)
        {
            enabled = false;
            return;
        }
        
        if(turret.transform.localPosition.x > rail.localScale.x/2 - 0.6f)
        {
            turret.transform.localPosition = new Vector3(rail.localScale.x / 2 - 0.6f, turret.transform.localPosition.y, turret.transform.localPosition.z);
            velocity = -velocity;
        }
        else if (turret.transform.localPosition.x < -(rail.localScale.x / 2 - 0.6f))
        {
            turret.transform.localPosition = new Vector3(-(rail.localScale.x / 2 - 0.6f), turret.transform.localPosition.y, turret.transform.localPosition.z);
            velocity = -velocity;
        }

        turret.transform.localPosition += Vector3.right * velocity * Time.deltaTime;
        
    }
}
