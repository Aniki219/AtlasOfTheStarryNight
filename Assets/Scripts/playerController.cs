using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (characterController))]
public class playerController : MonoBehaviour
{
    float jumpHeight = 2.2f;
    float timeToJumpApex = 0.5f;
    
    int variableJumpIncrements = 6;

    float airAccelerationTime = 0.1f;
    float groundAccelerationTime = 0f;

    public GameObject starParticles;

    float moveSpeed = 4f;
    int facing = 1;
    bool canBroom;
    public bool resetPosition = false;
    public bool intangible = false;
    Vector3 lastSafePosition;
    public float resetTime = 10f;

    bool isWallSliding = false;
    public float wallSlideModifier = 0.5f;
    float wallJumpVelocity = 0;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    Vector3 velocitySmoothing;

    characterController controller;
    Animator anim;

    GameObject starRotator;

    public State state = State.Movement;

    public enum State
    {
        Wait,
        Movement,
        BroomStart,
        Broom,
        Bonk,
        Reset,
        WallJumpInit,
        WallJump
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
            case State.WallJumpInit:
                wallJumpInit();
                break;
            case State.WallJump:
                handleWallJump();
                break;
            default:
                break;
        }

        //Debug.DrawLine(lastSafePosition, lastSafePosition + Vector3.up, Color.red);
    }

    void wallJumpInit()
    {
        StartCoroutine(WallJumpCoroutine());
    }

    void handleWallJump()
    {
    }

    IEnumerator WallJumpCoroutine()
    {
        state = State.WallJump;
        SoundManager.Instance.playClip("wallBlast");
        anim.SetBool("isJumping", false);
        anim.SetBool("isFalling", false);
        anim.SetBool("wallSlide", false);
        anim.SetBool("wallBlast", true);

        yield return new WaitForSeconds(0.2f);

        anim.SetBool("isJumping", true);
        anim.SetBool("wallBlast", false);

        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);

        velocity.y = jumpVelocity / 1.5f;

        float startTime = Time.time;
        while (Time.time - startTime < 0.1f)
        {
            velocity.x = jumpVelocity / 1.5f * transform.localScale.x;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.X) && canBroom)
            {
                state = State.BroomStart;
                canBroom = false;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        anim.SetBool("wallBlast", false);
        returnToMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Danger") && !intangible)
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
            isWallSliding = false;
        } else
        {
            if (canBroom && Input.GetKeyDown(KeyCode.X))
            {
                state = State.BroomStart;
                canBroom = false;
                return;
            }
            isWallSliding = checkWallSliding();

            if (isWallSliding && Input.GetKeyDown(KeyCode.Z))
            {
                state = State.WallJumpInit;
                return;
            }
        }

        float targetVelocityX = input.x * moveSpeed;
        anim.SetBool("isRunning", isGrounded() && (targetVelocityX != 0));
        anim.SetBool("isJumping", !isGrounded() && (velocity.y > 0));
        anim.SetBool("isFalling", !isGrounded() && (velocity.y < -.25f));
        anim.SetBool("isGrounded", isGrounded());
        anim.SetBool("wallSlide", isWallSliding);

        setFacing(velocity.x);
        transform.localScale = new Vector3(facing, 1, 1);
    
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? groundAccelerationTime : airAccelerationTime);

        float yAcc = gravity + (isWallSliding ? wallSlideModifier : 1);
        velocity.y += yAcc * Time.deltaTime;

        if (velocity.y < -15.0f) { velocity.y = -15.0f; }

        controller.Move(velocity * Time.deltaTime);

        if (velocity.y <= 0 && controller.isSafePosition())
        {
            lastSafePosition = transform.position;
        }
    }

    bool checkWallSliding()
    {
        if (isGrounded()) { return false; }
        float hdir = Input.GetAxisRaw("Horizontal");

        if (controller.collisions.left && hdir == -1 || controller.collisions.right && hdir == 1) {
            return true;
        }
        return false;
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

    void handleReset(bool isSafe = false)
    {
        intangible = true;
        anim.SetBool("resetSpin", true);
        if (!starRotator)
        {
            starRotator = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarRotator"), transform);
        }
        if (Vector3.SqrMagnitude(lastSafePosition - transform.position) < 0.01f)
        {
            transform.position = lastSafePosition;
            velocity = Vector3.zero;
            returnToMovement();
            anim.SetBool("resetSpin", false);
            StartCoroutine(flashEffect());
            Destroy(starRotator);
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
        GetComponent<SpriteRenderer>().enabled = true;
    }

    void returnToMovement()
    {
        if (resetPosition && !controller.isSafePosition())
        {
            state = State.Reset;
        } else
        {
            intangible = false;
            state = State.Movement;
        }
        resetPosition = false;
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
        if (velocity.y != 0) { return false; }
        return controller.collisions.isGrounded;
    }
}
