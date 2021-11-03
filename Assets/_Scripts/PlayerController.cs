using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to control the player moves.
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    enum AudioState
    {
        none,
        walking,
        running,
    }

    #region Atributes

    public static PlayerController instance;

    [HideInInspector]
    public CharacterController _characterController;
    private Animator _animator;

    [SerializeField]
    private float moveSpeed = 1, gravityModifier = 1, jumpPower = 1, runSpeed = 1;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float mouseSensitivity = 1;

    [SerializeField]
    private bool invertX, invertY;

    public bool canJump = false, canDoubleJump;

    [SerializeField]
    private Transform groundCheckPoint;

    [SerializeField]
    LayerMask isGround;

    public Vector3 moveInput;

    public Transform firePoint;

    private bool gamePaused;

    private float bounceAmount;

    private bool bouncing;

    AudioState audioState = AudioState.none;

    #endregion

    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        UIController.OnPauseStateChanged += HandleGamePaused;

        audioState = AudioState.none;
    }

    private void OnDestroy()
    {
        UIController.OnPauseStateChanged -= HandleGamePaused;
    }

    void Update()
    {
        if (gamePaused || GameManager.instance.levelStoppedOrEnded) { return; }

        MovePlayer();

        RotatePlayer();

        _animator.SetFloat("moveSpeed", moveInput.magnitude);
        _animator.SetBool("onGround", canJump);
    }

    private void MovePlayer()
    {
        Vector3 xMovement = transform.right * Input.GetAxis("Horizontal");
        Vector3 zMovement = transform.forward * Input.GetAxis("Vertical");

        float oldY = moveInput.y;

        moveInput = (xMovement + zMovement);
        moveInput.Normalize();

        if (CheckKeyboardInput(moveInput))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!AudioManager.instance._audioSourcePlayerMoves.isPlaying && audioState != AudioState.running)
                {
                    audioState = AudioState.running;
                    AudioManager.instance._audioSourcePlayerMoves.Stop();
                }
                else if (!AudioManager.instance._audioSourcePlayerMoves.isPlaying && _characterController.isGrounded)
                {
                    AudioManager.instance.Run();
                }
                moveInput *= runSpeed;

            }
            else
            {
                if (!AudioManager.instance._audioSourcePlayerMoves.isPlaying && audioState != AudioState.walking)
                {
                    audioState = AudioState.walking;
                    AudioManager.instance._audioSourcePlayerMoves.Stop();
                }
                else if (!AudioManager.instance._audioSourcePlayerMoves.isPlaying && _characterController.isGrounded)
                {
                    AudioManager.instance.Walk();
                }
                moveInput *= moveSpeed;
            }
        }
        else if (audioState != AudioState.none && AudioManager.instance._audioSourcePlayerMoves.isPlaying && canJump)
        {
            AudioManager.instance._audioSourcePlayerMoves.Stop();
            audioState = AudioState.none;
        }

        moveInput.y = oldY;
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (_characterController.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        bool _canJump = canJump;
        //check if our feet (imaginary sphere of radious .25) are touching the floor
        canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, isGround).Length > 0;

        if (!_canJump && canJump)
        {
            audioState = AudioState.none;
            AudioManager.instance.Land();
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            if (AudioManager.instance._audioSourcePlayerMoves.isPlaying)
            {
                AudioManager.instance._audioSourcePlayerMoves.Stop();
            }
            AudioManager.instance.Jump();
            audioState = AudioState.none;
            moveInput.y = jumpPower;
            canDoubleJump = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && canDoubleJump)
        {
            AudioManager.instance.Jump();
            audioState = AudioState.none;

            moveInput.y = jumpPower;
            canDoubleJump = false;
        }

        if (bouncing)
        {
            bouncing = false;
            moveInput.y = bounceAmount;
            canJump = false;
            canDoubleJump = true;
            audioState = AudioState.none;
        }

        _characterController.Move(moveInput * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity * Time.deltaTime;

        if (invertX)
            mouseInput.x = -mouseInput.x;

        if (invertY)
            mouseInput.y = -mouseInput.y;

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + mouseInput.x, 0);


        float xRot = cameraTransform.localEulerAngles.x;
        // Returned angles are in the range 0...360. Map that back to -180...180 for convenience.
        if (xRot > 180f)
            xRot -= 360f;

        // Increment the pitch angle, respecting the clamped range.
        xRot = Mathf.Clamp(xRot - mouseInput.y, -70f, 70f);

        cameraTransform.eulerAngles = new Vector3(xRot, cameraTransform.rotation.eulerAngles.y, cameraTransform.rotation.eulerAngles.z);
    }

    private bool CheckKeyboardInput(Vector3 input)
    {
        if (Mathf.Approximately(input.x, 0) && Mathf.Approximately(input.z, 0))
        {
            return false;
        }
        return true;
    }

    private void HandleGamePaused(bool gamePaused)
    {
        this.gamePaused = gamePaused;
    }

    public void Bounce (float bounceForce)
    {
        bounceAmount = bounceForce;
        bouncing = true;
    }
}
