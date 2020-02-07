using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (characterController))]
public class playerController : MonoBehaviour
{
    float jumpHeight = 2.6f;
    float timeToJumpApex = 0.5f;
    
    int variableJumpIncrements = 6;

    float airAccelerationTime = 0.1f;
    float groundAccelerationTime = 0f;

    public GameObject starParticles;

    float moveSpeed = 4f;
    public int facing = 1;
    bool canBroom;
    bool resetPosition = false;
    bool intangible = false;
    Vector3 lastSafePosition;
    float resetTime = 10f;

    bool isWallSliding = false;
    public float maxWallSlideVel = -5f;
    float maxFallVel = -15f;
    float wallJumpVelocity = 7;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    Vector3 velocitySmoothing;

    characterController controller;
    Animator anim;

    GameObject starRotator;

    bool fastBroom = false;
    bool screenShake = true;
    float wallBlastDelay = 0.2f;

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
        GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/wallBlast"), transform.position + new Vector3(-0.80f * facing, 0.23f, 0), Quaternion.identity); ;
        explosion.transform.localScale = transform.localScale;
        if (screenShake) { Camera.main.GetComponent<cameraController>().StartShake(0.25f, 0.2f); }
        
        SoundManager.Instance.playClip("wallBlast");
        anim.SetBool("isJumping", false);
        anim.SetBool("isFalling", false);
        anim.SetBool("wallSlide", false);
        anim.SetBool("wallBlast", true);

        yield return new WaitForSeconds(wallBlastDelay);

        anim.SetBool("isJumping", true);
        anim.SetBool("wallBlast", false);

        flipHorizontal();

        velocity.y = wallJumpVelocity;

        float startTime = Time.time;
        while (Time.time - startTime < 0.1f)
        {
            velocity.x = wallJumpVelocity * facing;
            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(velocity * Time.fixedDeltaTime);
            if (Input.GetKeyDown(KeyCode.X) && canBroom)
            {
                faceInputDirection();
                state = State.BroomStart;
                canBroom = false;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        anim.SetBool("wallBlast", false);
        returnToMovement();
    }

    void faceInputDirection()
    {
        float dir = Input.GetAxisRaw("Horizontal");
        if (dir != 0)
        {
            facing = (int)dir;
            transform.localScale = new Vector3(facing, 1, 1);
        }
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
                if (isWallSliding) {
                    flipHorizontal();
                } else
                {
                    faceInputDirection();
                }
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

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? groundAccelerationTime : airAccelerationTime);
        velocity.y += gravity * Time.fixedDeltaTime;

        if (isWallSliding && velocity.y < maxWallSlideVel) { velocity.y = maxWallSlideVel; }
        if (velocity.y < maxFallVel) { velocity.y = maxFallVel; }

        controller.Move(velocity * Time.fixedDeltaTime);

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
        state = fastBroom ? State.Broom : State.Wait;
    }

    void startBroom()
    {
        resetAnimator();
        state = State.Broom;
        SoundManager.Instance.playClip("broomLaunch");
    }

    void handleBroom()
    {
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger("broomEnd");
            state = State.Movement;
            return;
        }
        if ((facing == -1) ? controller.collisions.left : controller.collisions.right) {
            startBonk();
            return;
        }
        velocity.x = moveSpeed * 2 * facing;
        velocity.y = 0;
        controller.Move(velocity * Time.fixedDeltaTime);
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
        velocity.y += gravity * Time.fixedDeltaTime;
        if (!isGrounded())
        {
            velocity.x = -moveSpeed/4f * transform.localScale.x;
        }
        
        controller.Move(velocity * Time.fixedDeltaTime);
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
        transform.position = Vector3.SmoothDamp(transform.position, lastSafePosition, ref velocitySmoothing, resetTime * Time.fixedDeltaTime);
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
            transform.localScale = new Vector3(facing, transform.localScale.y, transform.localScale.z);
        }
    }

    void flipHorizontal()
    {
        facing = -(int)transform.localScale.x;
        transform.localScale = new Vector3(facing, 1, 1);
    }

    bool isGrounded()
    {
        if (velocity.y != 0) { return false; }
        return controller.collisions.isGrounded;
    }
}
