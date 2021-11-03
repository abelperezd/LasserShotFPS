using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This script controlls all the enemy movements.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    #region Atributes

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private GameObject gunModel;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    //shots per second
    private float fireRate;

    [SerializeField]
    //time we have to shoot before stoping
    private float timeToShoot = 1f;

    [SerializeField]
    //time we have to wait to shoot again
    private float waitBetweenShots = 2f;

    private float fireCount;
    private float waitBetweenShotsCounter;
    private float timeToShotCounter;

    [SerializeField]
    private float distanceToChase = 10;
    [SerializeField]
    private float distanceToLose = 15f;

    private bool inChasingRange, isShooting;

    private Vector3 startPoint;

    [SerializeField]
    private float keepinChasingRangeTime = 3;

    //time we wait to go to the start point if we loose the player
    private float keepinChasingRangeTimeCounter = 0;

    [SerializeField]
    private AudioClip shootAudio;

    private bool wasShot;

    #endregion

    void Start()
    {
        startPoint = transform.position;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        timeToShotCounter = timeToShoot;
        waitBetweenShotsCounter = waitBetweenShots;

        Health.OnPlayerDies += HandlePlayerDies;
    }

    private void HandlePlayerDies()
    {
        _navMeshAgent.ResetPath();
        _navMeshAgent.isStopped = true;
        _animator.SetBool("isMoving", false);

        Health.OnPlayerDies -= HandlePlayerDies;
    }

    private Vector3 targetPoint;

    void Update()
    {
        if (GameManager.instance.levelStoppedOrEnded) { return; }

        //point to follow
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;

        if (!inChasingRange)
        {
            //if we enter the range to chase
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                inChasingRange = true;
                _navMeshAgent.isStopped = false;

                _navMeshAgent.destination = targetPoint;
                _animator.SetBool("isMoving", true);

                timeToShotCounter = timeToShoot;
                waitBetweenShotsCounter = waitBetweenShots;
                return;
            }
            else if (keepinChasingRangeTimeCounter > 0 )
            {
                //if we recently lost our target
                keepinChasingRangeTimeCounter -= Time.deltaTime;
                if (keepinChasingRangeTimeCounter <= 0)
                {
                    //if the time to keep chasing after losing the player ends
                    _navMeshAgent.isStopped = false;
                    _navMeshAgent.destination = startPoint;
                    _animator.SetBool("isMoving", true);
                    return;
                }
            }

            //go back to starting position
            if(_navMeshAgent.isStopped == false)
            {
                if (_navMeshAgent.remainingDistance <= (_navMeshAgent.stoppingDistance + 0.5f))
                {
                    _navMeshAgent.isStopped = true;
                    //_navMeshAgent.ResetPath();
                    _animator.SetBool("isMoving", false);
                    return;
                }
            }
            
        }
        else
        {
            //follow the player
            if (Vector3.Distance(transform.position, targetPoint) <= distanceToLose)
            {
                _navMeshAgent.destination = targetPoint;
                wasShot = false;
            }
            //start following the player if we were out of range but he shot us
            else if (wasShot)
            {
                _navMeshAgent.destination = targetPoint;
            }
            //we lost the player
            else
            {
                TargetLost();
                return;
            }
            //wait before start shooting again
            if (waitBetweenShotsCounter > 0)
            {
                WaitBetweenShots();
            }
            else
            {
                TryToShoot();
            }
            
        }
    }

    private void TargetLost()
    {
        _navMeshAgent.isStopped = true;
        inChasingRange = false;
        keepinChasingRangeTimeCounter = keepinChasingRangeTime;
        _animator.SetBool("isMoving", false);
    }

    /// <summary>
    /// We shoot in bursts. This controlls the timer between bursts.
    /// </summary>
    private void WaitBetweenShots()
    {
        if (isShooting) { return; }

        waitBetweenShotsCounter -= Time.deltaTime;

        //start shooting
        if (waitBetweenShotsCounter <= 0)
        {
            timeToShotCounter = timeToShoot;
        }
        else if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + 0.5f)
        {
            if(_navMeshAgent.isStopped == false)
            {
                _navMeshAgent.isStopped = true;
                _animator.SetBool("isMoving", false);
            }
        }
        else if (_navMeshAgent.isStopped == true)
        {
            _navMeshAgent.isStopped = false;
            _animator.SetBool("isMoving", true);
            _navMeshAgent.destination = targetPoint;
        }
        
    }

    private void TryToShoot()
    {
        timeToShotCounter -= Time.deltaTime;

        if (timeToShotCounter <= 0)
        {
            waitBetweenShotsCounter = waitBetweenShots;
            return;
        }

        fireCount -= Time.deltaTime;
        if (fireCount > 0) { return; }

        fireCount = fireRate;
        
        //stop to shoot
        if(_navMeshAgent.isStopped == false)
        {
            _navMeshAgent.isStopped = true;
            _animator.SetBool("isMoving", false);
        }


        firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 1.2f, 0f));

        //check angle to the player
        Vector3 targetDir = PlayerController.instance.transform.position - transform.position;

        //diference between the direction of the player and the place where the enemy is pointing at
        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        if (Mathf.Abs(angle) < 30f)
        {
            isShooting = true;
            _animator.SetTrigger("fireShot");

            AudioManager.instance.PlayClipAtPoint(shootAudio, transform);

            EnableGunModel();

            Instantiate(bullet, firePoint.position, firePoint.rotation);
        }
        else
        {
            waitBetweenShotsCounter = waitBetweenShots;
        }
    }

    public void WasShot()
    {
        inChasingRange = true;
        wasShot = true;
    }

    private void ShootAnimationEnded()
    {
        if (GameManager.instance.levelStoppedOrEnded) { return; }
        isShooting = false;
        _navMeshAgent.isStopped = false;
        _animator.SetBool("isMoving", true);
    }

    private void OnDestroy()
    {
        Health.OnPlayerDies -= HandlePlayerDies;
    }

    public void EnableGunModel()
    {
        gunModel.SetActive(true);
    }

    public void DisableGunModel()
    {
        gunModel.SetActive(false);
    }
}
