using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform feetTouchCheck;
    [SerializeField] LayerMask groundMask;
    public WeaponScript weaponScript;

    public bool isGrounded { get; private set; } = false;
    public bool isRunning { get; private set; } = false;
    public bool isJumping { get; private set; } = false;
    public bool isCrouching { get; private set; } = false;
    public bool isGoingForward { get; private set; } = false;
    [System.NonSerialized] public Vector3 jumpVelocity = Vector3.zero;

    [Header("Leaning")]
    public Transform leanPivot;
    private float currentLean;
    private float targetLean;
    public float leanAngle;
    public float leanSmoothing;
    private float leanVelocity;

    [Header("Looking At")]
    [SerializeField] private LayerMask lookIgnore;
    public float lookDistance;

    [Header("Animations")]
    public CharacterController characterController;
    private PlayerStats playerStats;
    public Animator weaponAnimator;
    public Animator animator2;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();

    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(feetTouchCheck.position, 0.4f, groundMask);

        LookingAt();
        HandleMovement();
        HandleJumpInput();
        CalculateLeaning();
    }

    void LookingAt()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 10, ~lookIgnore))
        {
            lookDistance = hit.distance;
        }
        else
        {
            lookDistance = 10;
        }
    }

    void HandleJumpInput()
    {
        bool isTryingToJump = Input.GetKeyDown(KeyCode.Space);

        if (isTryingToJump && isGrounded)
        {
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }

        if (isJumping)
        {
            jumpVelocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * playerStats.gravity);
        }
        jumpVelocity.y += playerStats.gravity * Time.deltaTime;

        characterController.Move(jumpVelocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        isGoingForward = Input.GetKey(KeyCode.W);

        if (isCrouching)
        {
            HandleCrouch();
        }
        else
        {
            HandleStand();
        }

        Vector3 movementVector = Vector3.ClampMagnitude(transform.right * horizontalInput + transform.forward * verticalInput, 1.0f);

        if (isCrouching)
        {
            characterController.Move(movementVector * playerStats.crouchingMovementSpeed * Time.deltaTime);
            weaponAnimator.SetBool("isSprinting", false);
        }
        else if (isRunning && isGoingForward && !weaponScript.isFiring && !weaponScript.isReloading)
        {
            characterController.Move(movementVector * playerStats.runningMovementSpeed * Time.deltaTime);
            animator2.SetBool("isWalking", false);
            weaponAnimator.SetBool("isSprinting", true);
        }
        else
        {
            characterController.Move(movementVector * playerStats.walkingMovementSpeed * Time.deltaTime);
            weaponAnimator.SetBool("isSprinting", false);
        }



        if (movementVector != Vector3.zero && !isRunning)
        {
            animator2.SetBool("isWalking", true);
        }
        else
        {
            animator2.SetBool("isWalking", false);
        }
    }

    void HandleCrouch()
    {
        if (characterController.height > playerStats.crouchHeightY)
        {
            UpdateCharacterHeight(playerStats.crouchHeightY);

            if (characterController.height - 0.05f <= playerStats.crouchHeightY)
            {
                characterController.height = playerStats.crouchHeightY;
            }
        }
    }

    void HandleStand()
    {
        if (characterController.height < playerStats.standingHeightY)
        {
            float lastHeight = characterController.height;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up, out hit, playerStats.standingHeightY))
            {
                if (hit.distance < playerStats.standingHeightY - playerStats.crouchHeightY)
                {
                    isCrouching = true;
                }

            }
            else
            {
                UpdateCharacterHeight(playerStats.standingHeightY);
            }


            if (characterController.height + 0.05f >= playerStats.standingHeightY)
            {
                characterController.height = playerStats.standingHeightY;
            }
            transform.position += new Vector3(0, (characterController.height - lastHeight) / 2, 0);
        }
    }

    void UpdateCharacterHeight(float newHeight)
    {
        characterController.height = Mathf.Lerp(characterController.height, newHeight, playerStats.crouchSpeed * Time.deltaTime);
    }

    void CalculateLeaning()
    {
        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);

        leanPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, currentLean));

        if (Input.GetKey(KeyCode.E) && !isRunning)
        {
            targetLean = -leanAngle;
        }
        else
        {
            targetLean = 0;
        }
        if (Input.GetKey(KeyCode.Q) && !isRunning)
        {
            targetLean = leanAngle;
        }
        
    }
}
