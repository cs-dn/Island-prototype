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

        if(Input.GetButtonDown("Jump")/* && groundedPlayer */)
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
