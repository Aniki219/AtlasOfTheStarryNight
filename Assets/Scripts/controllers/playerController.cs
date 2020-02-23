using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (characterController))]
public class playerController : MonoBehaviour
{
    float jumpHeight = 2.2f;
    public float doubleJumpHeight = 1f;
    float timeToJumpApex = 0.5f;
    public float timeToDoubleJumpApex = 0.25f;

    int variableJumpIncrements = 6;

    float airAccelerationTime = 0.1f;
    float groundAccelerationTime = 0f;

    public GameObject starParticles;

    float moveSpeed = 4f;
    public int facing = 1;
    bool canBroom = false;
    bool canDoubleJump = false;
    bool resetPosition = false;
    Vector3 lastSafePosition;
    float resetTime = 10f;

    bool isWallSliding = false;
    public float maxWallSlideVel = -5f;
    float maxFallVel = -15f;
    float wallJumpVelocity = 7;

    float gravity;
    float jumpVelocity;
    float doubleJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    Vector3 velocitySmoothing;

    characterController controller;
    Animator anim;

    GameObject starRotator;

    bool fastBroom = false;
    bool screenShake = false;
    float wallBlastDelay = 0.2f;

    //bool fastBroom = true;
    //bool screenShake = false;
    //float wallBlastDelay = 0f;

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

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<characterController>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        float djgravity = -(2 * doubleJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        doubleJumpVelocity = Mathf.Abs(djgravity) * timeToDoubleJumpApex;
        lastSafePosition = transform.position;
    }

    void Update()
    {
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

        Debug.DrawLine(lastSafePosition, lastSafePosition + Vector3.up, Color.blue);
    }

    void wallJumpInit()
    {
        if (resourceManager.Instance.getPlayerMana() < 2) {
            state = State.Movement;
            return;
        }
        resourceManager.Instance.usePlayerMana(2);
        velocity.y = 0;
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
        if (screenShake) { Camera.main.GetComponent<cameraController>().StartShake(0.2f, 0.15f); }
        
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
            velocity.y += gravity * Time.deltaTime;
            //controller.Move(velocity * Time.deltaTime);
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Danger") && controller.collisions.tangible)
        {
            startBonk(1, true);
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
            canDoubleJump = true;
            isWallSliding = false;
            resourceManager.Instance.restoreMana();
        } else
        {
            if (canBroom && Input.GetKeyDown(KeyCode.X))
            {
                state = State.BroomStart;
                canBroom = false;
                if (isWallSliding && velocity.y < 0) {
                    flipHorizontal();
                } else
                {
                    faceInputDirection();
                }
                return;
            }
            isWallSliding = checkWallSliding();

            if (isWallSliding && Input.GetKeyDown(KeyCode.Z) && resourceManager.Instance.getPlayerMana() >= 2)
            {
                state = State.WallJumpInit;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Z) && canDoubleJump && resourceManager.Instance.getPlayerMana() >= 1)
            {
                doubleJump();
            }
        }

        float targetVelocityX = input.x * moveSpeed;

        anim.SetBool("isRunning", isGrounded() && (targetVelocityX != 0));
        anim.SetBool("isJumping", !isGrounded() && (velocity.y > 0) && canDoubleJump);
        anim.SetBool("isFalling", !isGrounded() && (velocity.y < -0.5f) && !controller.collisions.descendingSlope);
        anim.SetBool("isGrounded", isGrounded());
        anim.SetBool("wallSlide", isWallSliding);
        if (velocity.y < 0) { anim.SetBool("doubleJump", false); }

        setFacing(velocity.x);

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? groundAccelerationTime : airAccelerationTime);
        velocity.y += gravity * Time.deltaTime;

        if (isWallSliding && velocity.y < maxWallSlideVel) { velocity.y = maxWallSlideVel; }
        if (velocity.y < maxFallVel) { velocity.y = maxFallVel; }

        if (velocity.y <= 0 && controller.isSafePosition())
        {
            lastSafePosition = transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (state == State.Reset) { return; }
        controller.Move(velocity * Time.deltaTime);
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }

    void doubleJump()
    {
        state = State.Movement;
        resourceManager.Instance.usePlayerMana(1);
        canDoubleJump = false;
        anim.SetBool("doubleJump", true);
        velocity.y = doubleJumpVelocity;
        SoundManager.Instance.playClip("doubleJump");
        StopCoroutine(jumpCoroutine());
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
        velocity = Vector3.zero;
        controller.Move(velocity);
    }

    void startBroom()
    {
        resetAnimator();
        state = State.Broom;
        SoundManager.Instance.playClip("broomLaunch");
    }

    void handleBroom()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("broomEnd");
            state = State.Movement;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Z) && canDoubleJump && resourceManager.Instance.getPlayerMana() >= 1)
        {
            doubleJump();
            return;
        }
        if ((facing == -1) ? controller.collisions.left : controller.collisions.right) {
            startBonk();
            return;
        }
        velocity.x = moveSpeed * 2 * facing;
    }

    public void startBonk(int damage = 0, bool reset = false)
    {
        if (!controller.collisions.tangible) { return; }
        if (reset)
        {
            resetPosition = true;
            controller.collisions.tangible = false;
            SoundManager.Instance.playClip("hurt2");
        } else { 
            SoundManager.Instance.playClip("bonk");
            createStars(transform.position);
        }

        if (damage > 0) { resourceManager.Instance.takeDamage(damage); }

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
        
        //controller.Move(velocity * Time.deltaTime);
    }

    void handleReset(bool isSafe = false)
    {
        controller.collisions.tangible = false;
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
            controller.collisions.tangible = true;
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
        for (int i = 0; i < variableJumpIncrements; i++)
        {
            if (!Input.GetKey(KeyCode.Z)) {
                velocity.y /= 4;
                i = variableJumpIncrements;
            }
            yield return new WaitForSeconds(4/60.0f);
        }
    }

    void setFacing(float vel)
    {
        if (vel == 0) return;
        facing = (int)Mathf.Sign(vel);
        transform.localScale = new Vector3(facing, transform.localScale.y, transform.localScale.z);
    }

    void flipHorizontal()
    {
        facing = -(int)transform.localScale.x;
        transform.localScale = new Vector3(facing, 1, 1);
    }

    bool isGrounded()
    {
        return controller.collisions.below;
    }
}
