using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (characterController))]
public class playerController : MonoBehaviour
{
    public float jumpHeight = 2f;
    public float timeToJumpApex = 0.5f;
    
    public int variableJumpIncrements = 4;

    public float airAccelerationTime = 0f;
    public float groundAccelerationTime = 0f;
    float moveSpeed = 4f;
    int facing = 1;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    characterController controller;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<characterController>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    // Update is called once per frame
    void Update()
    {
        controller.checkGrounded(ref velocity);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Z) && isGrounded())
        {
            velocity.y = jumpVelocity;
            StartCoroutine(jumpCoroutine());
        }

        float targetVelocityX = input.x * moveSpeed;
        anim.SetBool("isRunning", isGrounded() && (targetVelocityX != 0));
        anim.SetBool("isJumping", !isGrounded() && (velocity.y > 0));
        anim.SetBool("isFalling", !isGrounded() && (velocity.y < -.25f));
        anim.SetBool("isGrounded", isGrounded());

        setFacing(targetVelocityX);
        transform.localScale = new Vector3(facing, 1, 1);

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?groundAccelerationTime:airAccelerationTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);        
    }


    IEnumerator jumpCoroutine()
    {
        int i = 0;
        while (Input.GetKey(KeyCode.Z) && i < variableJumpIncrements)
        {
            i++;
            yield return new WaitForSeconds(4.0f/60);
        }
        if (i < variableJumpIncrements)
        {
            velocity.y /= 4;
            i = variableJumpIncrements;
        }
    }

    void setFacing(float vel)
    {
        if (vel != 0)
        {
            facing = (int)Mathf.Sign(vel);
        }
    }

    bool isGrounded()
    {
        return controller.collisions.isGrounded;
    }
}
