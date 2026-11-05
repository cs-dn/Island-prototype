/*
using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Vector3 playerVelocity;
    [SerializeField] bool groundedPlayer;
    [SerializeField] float playerSpeed;
    [SerializeField] float gravityValue;
    [SerializeField] GameObject activeChar;
    [SerializeField] float moveHorizontal;
    [SerializeField] float moveVertical;
    [SerializeField] float speed = 5;
    [SerializeField] float rotateSpeed = 1;
    [SerializeField] float jumpHeight = 20;
    [SerializeField] bool isJumping;
    [SerializeField] bool isRunning;


    void Start()
    {
        playerSpeed = 4;
        gravityValue = -20;
        groundedPlayer = true;
        isRunning = false;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if(groundedPlayer)
        {
            playerVelocity.y = 0;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(speed < 20)
            {
                speed = speed + 0.1f;
            }
            else
            {
                speed = 20;
            }
        }
        else
        {
            speed = 5;
        }

        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float curSpeed = speed * Input.GetAxis("Vertical");
        controller.SimpleMove(forward * curSpeed);

        if(Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            activeChar.GetComponent<Animator>().Play("Jump");
            playerVelocity.y += jumpHeight;
            playerVelocity.y += gravityValue * Time.deltaTime;
            StartCoroutine(ResetJump());
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        this.gameObject.GetComponent<CharacterController>().minMoveDistance = 0.001f;

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            this.gameObject.GetComponent<CharacterController>().minMoveDistance = 0.001f;
            if(isJumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Run");
            }
        }

        else
        {
            this.gameObject.GetComponent<CharacterController>().minMoveDistance = 0.901f;
            if(isJumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Idle");
                playerVelocity.y += gravityValue * Time.deltaTime;
            }
        }
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(1.5f);
        isJumping = false;
    }
}
*/

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float turnSpeed = 12f;

    [Header("Jumping")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.5f;

    private CharacterController controller;
    private float verticalSpeed;
    private float currentSpeed;
    private bool jumpingThisFrame;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("PlayerControls: Assign the camera Transform in the Inspector.", this);
            return;
        }

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 input = Vector3.ClampMagnitude(
            new Vector3(inputX, 0f, inputY),
            1f
        );

        Vector3 cameraForward = Vector3.ProjectOnPlane(
            cameraTransform.forward,
            Vector3.up
        ).normalized;

        Vector3 cameraRight = Vector3.ProjectOnPlane(
            cameraTransform.right,
            Vector3.up
        ).normalized;

        Vector3 moveDirection = cameraForward * input.z + cameraRight * input.x;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }

        bool grounded = controller.isGrounded;
        jumpingThisFrame = grounded && Input.GetButtonDown("Jump");

        if (grounded && verticalSpeed < 0f)
            verticalSpeed = -2f;

        if (jumpingThisFrame)
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetTrigger("Jump");
        }

        verticalSpeed += gravity * Time.deltaTime;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isRunning ? runSpeed : moveSpeed;

        if (input.magnitude == 0f)
            targetSpeed = 0f;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * Time.deltaTime
        );

        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalSpeed;

        controller.Move(velocity * Time.deltaTime);

        if (animator != null)
        {
            float animationSpeed = runSpeed > 0f
                ? currentSpeed / runSpeed
                : 0f;

            animator.SetFloat("Speed", animationSpeed, 0.1f, Time.deltaTime);
            animator.SetBool("Grounded", controller.isGrounded && !jumpingThisFrame);
        }
    }
}