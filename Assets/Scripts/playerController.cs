using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (characterController))]
public class playerController : MonoBehaviour
{
    public GameObject starRotator;
    float jumpHeight = 2.2f;
    float timeToJumpApex = 0.5f;
    
    int variableJumpIncrements = 6;

    float airAccelerationTime = 0f;
    float groundAccelerationTime = 0f;

    public GameObject starParticles;

    float moveSpeed = 4f;
    int facing = 1;
    bool canBroom;
    bool resetPosition = false;
    bool intangible = false;
    Vector3 lastSafePosition;
    public float resetTime = 10f;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    Vector3 velocitySmoothing;

    characterController controller;
    Animator anim;

    State state = State.Movement;

    enum State
    {
        Wait,
        Movement,
        BroomStart,
        Broom,
        Bonk,
        Reset
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<characterController>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        lastSafePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        controller.checkGrounded(ref velocity);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        switch (state)
        {
            case State.Movement:
                handleMovement();
                break;
            case State.BroomStart:
                handleBroomStart();
                break;
            case State.Broom:
                handleBroom();
                break;
            case State.Bonk:
                bonk();
                break;
            case State.Reset:
                handleReset();
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ResetDamaging") && !intangible)
        {
            startBonk(true);
            return;
        }
    }

    void handleMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (isGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                velocity.y = jumpVelocity;
                SoundManager.Instance.playClip("jump2");
                StartCoroutine(jumpCoroutine());
            }
            canBroom = true;
        } else
        {
            if (canBroom && Input.GetKeyDown(KeyCode.X))
            {
                state = State.BroomStart;
                canBroom = false;
                return;
            }
        }

        float targetVelocityX = input.x * moveSpeed;
        anim.SetBool("isRunning", isGrounded() && (targetVelocityX != 0));
        anim.SetBool("isJumping", !isGrounded() && (velocity.y > 0));
        anim.SetBool("isFalling", !isGrounded() && (velocity.y < -.25f));
        anim.SetBool("isGrounded", isGrounded());

        setFacing(targetVelocityX);
        transform.localScale = new Vector3(facing, 1, 1);
    
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? groundAccelerationTime : airAccelerationTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (velocity.y <= 0 && controller.isSafePosition())
        {
            lastSafePosition = transform.position;
        }
    }

    void resetAnimator()
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
    }

    void handleBroomStart()
    {
        anim.SetTrigger("broomStart");
        SoundManager.Instance.playClip("onBroom");
        state = State.Wait;
    }

    void startBroom()
    {
        resetAnimator();
        state = State.Broom;
        SoundManager.Instance.playClip("broomLaunch");
    }

    void handleBroom()
    {
        int directionX = Mathf.RoundToInt(transform.localScale.x);
        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("broomEnd");
            state = State.Movement;
            return;
        }
        if ((directionX == -1) ? controller.collisions.left : controller.collisions.right) {
            startBonk();
            return;
        }
        velocity.x = moveSpeed * 2 * transform.localScale.x * Time.deltaTime;
        velocity.y = 0;
        controller.Move(velocity);
    }

    void startBonk(bool reset = false)
    {
        if (reset)
        {
            resetPosition = true;
            intangible = true;
            SoundManager.Instance.playClip("hurt2");
        } else { 
            SoundManager.Instance.playClip("bonk");
            createStars(transform.position);
        }

        resetAnimator();
        anim.SetTrigger("bonk");
        state = State.Bonk;
        velocity.y = 4f;
        
        Camera.main.GetComponent<cameraController>().StartShake();
    }

    void bonk()
    {
        velocity.y += gravity * Time.deltaTime;
        if (!isGrounded())
        {
            velocity.x = -moveSpeed/4f * transform.localScale.x;
        }
        
        controller.Move(velocity * Time.deltaTime);
    }

    void handleReset()
    {
        intangible = true;
        starRotator.SetActive(true);
        if (Vector3.SqrMagnitude(lastSafePosition - transform.position) < 0.01f)
        {
            transform.position = lastSafePosition;
            returnToMovement();
            StartCoroutine(flashEffect());
            starRotator.SetActive(false);
        }
        transform.position = Vector3.SmoothDamp(transform.position, lastSafePosition, ref velocitySmoothing, resetTime * Time.deltaTime);
    }

    IEnumerator flashEffect(float duration = 1.0f)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void returnToMovement()
    {
        if (resetPosition)
        {
            resetPosition = false;
            state = State.Reset;
        } else
        {
            intangible = false;
            state = State.Movement;
        }
    }

    void createStars(Vector3 position)
    {
        Instantiate(starParticles, position + Vector3.up * 0.5f, Quaternion.Euler((transform.localScale.x == 1) ? 180 : 0, 90, 0));
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
