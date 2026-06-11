using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float topClamp = 70f;
    [SerializeField] private float botClamp = -30f;


    [Header("Speed")]
    [SerializeField] private float lookSpeed = 10f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float acceleration = 8f;


    [Header("Grounded")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jumping")]
    [SerializeField] private float jumpStrength = 7f;
    [SerializeField] private float jumpCD = 1f;


    private Vector2 move;
    private Vector2 look;
    private bool isGrounded = true;
    private bool canJump = true;
    private float yaw;
    private float pitch;
    private float currSpeed;


    private const string speedParamName = "Speed";
    private const string jumpParamName = "Jump";
    private const string groundedParamName = "Grounded";
    private const string fallingParamName = "Falling";
    private const float lookThreshold = 0.01f;


    private Rigidbody body;
    private Animator animator;

    
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        groundCheck();
    }

    private void LateUpdate()
    {
        Look();
    }

    private void FixedUpdate() {
        Move();
    }


    private void Jump()
    {
        if(!isGrounded || !canJump)
        {
            return;
        }

        body.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        canJump = false;
        StartCoroutine(JumpDowntime());
        animator.SetTrigger(jumpParamName);
    }

    private IEnumerator JumpDowntime()
    {
        yield return new WaitForSeconds(0.25f);
        var waitForGrounded = new WaitUntil(()=> isGrounded);
        yield return waitForGrounded;
        yield return new WaitForSeconds(jumpCD);
        canJump = true;
    }

    private void Move()
    {
        float targetSpeed = moveSpeed * move.magnitude;
        currSpeed = Mathf.Lerp(currSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);

        Vector3 forward = cameraTarget.forward;
        Vector3 right = cameraTarget.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * move.y + right * move.x).normalized;

        if(moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            Vector3 currVelocity = body.linearVelocity;
            body.linearVelocity = new Vector3(moveDir.x * currSpeed, currVelocity.y, moveDir.z*currSpeed);
        }
        else
        {
            Vector3 currVelocity = body.linearVelocity;
            body.linearVelocity = new Vector3(0, currVelocity.y, 0);
        }

        float normalizeAnimSpeed = currSpeed/(moveSpeed*2f);
        animator.SetFloat(speedParamName, normalizeAnimSpeed);
        animator.SetBool(fallingParamName, !isGrounded && body.linearVelocity.y < -0.1f);
    }

    private void Look()
    {
        if(look.sqrMagnitude >= lookThreshold)
        {
            float deltaTimeMultiplier = Time.deltaTime * lookSpeed;
            yaw += look.x * deltaTimeMultiplier;
            pitch -= look.y * deltaTimeMultiplier;
        }

        yaw = ClampCam(yaw, float.MinValue, float.MaxValue);
        pitch = ClampCam(pitch, botClamp, topClamp);

        cameraTarget.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private float ClampCam(float lfAngle, float lfMin, float lfMax)
    {
        if(lfAngle < -360f)
        {
            lfAngle += 360f;
        }

        if(lfAngle > 360f)
        {
            lfAngle -= 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void groundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        animator.SetBool(groundedParamName, isGrounded);
    }
    
    private void OnMove(InputValue inputValue){
        move = inputValue.Get<Vector2>();
    }

    private void OnJump()
    {
        Jump();
    }

    private void OnLook(InputValue inputValue)
    {
        look = inputValue.Get<Vector2>();
    }
}
