﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (characterController))]
public class playerController : MonoBehaviour
{
    float jumpHeight = 2.2f;
    float doubleJumpHeight = 1.5f;
    float timeToJumpApex = 0.5f;
    float timeToDoubleJumpApex = 0.6f;

    int variableJumpIncrements = 6;

    float airAccelerationTime = 0.1f;
    float groundAccelerationTime = 0f;

    float moveSpeed = 4f;
    int facing = 1;
    bool canBroom = false;
    bool canDoubleJump = false;
    bool canTornado = true;
    [SerializeField] bool resetPosition = false;
    Vector3 lastSafePosition;
    float resetTime = 10f;

    bool isWallSliding = false;
    float maxWallSlideVel = -5f;
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
    Transform currentTornado;

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
        Hurt,
        Reset,
        WallJumpInit,
        WallJump,
        Tornado,
        Eat
    }

    //Any state thatcannot receive damage or interact with triggers
    List<State> intangibleStates = new List<State>
    {
        State.Hurt, State.Bonk, State.Reset
    };

    void Start()
    {
        gameManager.Instance.player = gameObject;
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<characterController>();
        gravity = gameManager.Instance.gravity; //-(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
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
                canPickUp();
                break;
            case State.BroomStart:
                handleBroomStart();
                break;
            case State.Broom:
                handleBroom();
                break;
            //Hurt and Bonk look the same to the player, but have different effects
            case State.Bonk:
            case State.Hurt:
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
            case State.Tornado:
                handleTornado();
                break;
            default:
                break;
        }

        //Debug.DrawLine(lastSafePosition, lastSafePosition + Vector3.up, Color.blue);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "ResetDamaging")
        {
            resetPosition = true;
        }
        if (intangibleStates.Contains(state))
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Danger") && controller.collisions.tangible)
        {
            startBonk(1, resetPosition);
            return;
        }

        if (other.tag == "Tornado" && canTornado)
        {
            resetAnimator();
            SoundManager.Instance.playClip("LevelObjects/EnterTornado");
            canTornado = false;
            state = State.Tornado;
            currentTornado = other.transform;
            Deformer deformer = other.GetComponent<Deformer>();
            if (deformer)
            {
                deformer.startDeform(new Vector3(1.75f, .75f, 1.0f), 0.1f);
            }
        }

        if (other.tag == "Follower")
        {
            followController other_fc = other.GetComponent<followController>();
            if (!other_fc.canCollect) { return; }

            int numFollowers = 0;
            other_fc.following = getFollower(GetComponent<followController>());
            other_fc.following.followedBy = other_fc;
            other_fc.canCollect = false;

            followController getFollower(followController fc)
            {
                if (!fc.followedBy)
                {
                    return fc;
                } else
                {
                    numFollowers++;
                    return getFollower(fc.followedBy);
                }
            }
            SoundManager.Instance.playClip("Collectibles/starShard", numFollowers);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Tornado")
        {
            canTornado = true;
            anim.SetBool("inTornado", false);
        }
    }

    void canPickUp()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded())
        {
            RaycastHit2D pickup = Physics2D.Raycast(transform.position, -Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Pickupable"));
            if (pickup.collider != null)
            {
                pickup.transform.SendMessage("pickUp");
            }
        }
    }

    public void startEat()
    {
        state = State.Eat;
        velocity = new Vector3(0, 0, 0);
        resetAnimator();
        anim.SetBool("eat", true);
    }

    IEnumerator LerpToPosition(Vector3 pos, float time = 0.5f)
    {
        while (Vector3.SqrMagnitude(transform.position - pos) > 0.005f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocitySmoothing, time);
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }

    void handleTornado()
    {
        canTornado = false;
        canBroom = true;
        canDoubleJump = true;
        anim.SetBool("inTornado", true);
        velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, currentTornado.position, ref velocitySmoothing, 0.05f);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            firstJump();
            anim.SetBool("inTornado", false);
            //state = State.Movement;
            returnToMovement();
            Deformer deformer = currentTornado.GetComponent<Deformer>();
            if (deformer)
            {
                deformer.startDeform(new Vector3(0.75f, 2.0f, 1.0f), 0.1f);
                SoundManager.Instance.playClip("LevelObjects/EnterTornado", 1);
            }
            currentTornado = null;
            //SoundManager.Instance.playClip("LevelObjects/WindPuff");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            movingPlatform mp = currentTornado.GetComponent<movingPlatform>();
            if (mp)
            {
                velocity.x = mp.getVelocity().x;
            }
            Deformer deformer = currentTornado.GetComponent<Deformer>();
            if (deformer)
            {
                deformer.startDeform(new Vector3(2.0f, 0.25f, 1.0f), 0.1f);
            }
            anim.SetBool("inTornado", false);
            //state = State.Movement;
            returnToMovement();
            currentTornado = null;
            SoundManager.Instance.playClip("LevelObjects/EnterTornado", -1);
        }
    }

    void wallJumpInit()
    {
        if (resourceManager.Instance.getPlayerMana() < 2) {
            //state = State.Movement;
            returnToMovement();
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
        GameObject explosion = gameManager.Instance.createInstance("Effects/wallBlast", transform.position + new Vector3(-0.80f * facing, 0.23f, 0));
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

    void handleMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (isGrounded())
        {
        //Debug.Log(velocity.y);
            if (Input.GetKeyDown(KeyCode.Z))
            {
                firstJump();
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

    void firstJump()
    {
        GetComponent<Deformer>().startDeform(new Vector3(1.0f, 1.25f, 1.0f), 0.05f, 0.1f);
        velocity.y = jumpVelocity;
        SoundManager.Instance.playClip("jump2");
        StartCoroutine(jumpCoroutine());
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
        //state = State.Movement;
        returnToMovement();
        resourceManager.Instance.usePlayerMana(1);
        canDoubleJump = false;
        anim.SetTrigger("doubleJump");
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

    public void resetAnimator()
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
            //state = State.Movement;
            returnToMovement();
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

    //Take whether to set resetPos or not
    //Set state to Hurt or Bonk
    public void startBonk(int damage = 0, bool reset = false)
    {
        if (!controller.collisions.tangible) { return; }
        controller.collisions.tangible = false;
        if (reset)
        {
            resetPosition = true;
            SoundManager.Instance.playClip("hurt2");
        } else { 
            if (damage > 0)
            {
                SoundManager.Instance.playClip("hurt");
            } else
            {
                SoundManager.Instance.playClip("bonk");
            }
            createStars(transform.position);
        }

        if (damage > 0) { resourceManager.Instance.takeDamage(damage); }

        resetAnimator();
        anim.SetTrigger("bonk");
        velocity.y = 4f;
        state = (damage > 0) ? State.Hurt : State.Bonk;
        
        Camera.main.GetComponent<cameraController>().StartShake();
    }

    void bonk()
    {
        velocity.y += gravity * Time.deltaTime;
        if (!isGrounded())
        {
            velocity.x = -moveSpeed/4f * transform.localScale.x;
        }
    }

    //Activates on State.Reset
    //Sets tangible to true
    //Returnns to movement after reaching lastSafePosition
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

    //Always turns off resetPosition
    //If not in a safe spot and resetPosition is true, sets state to reset
    //Otherwise sets state to movement and sets tangible to true
    public void returnToMovement()
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

    public void cutScenePrep()
    {
        velocity = Vector3.zero;
        resetAnimator();
        state = State.Wait;
    }

    void createStars(Vector3 position)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarParticles"), position + Vector3.up * 0.5f, Quaternion.Euler((transform.localScale.x == 1) ? 180 : 0, 90, 0));
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

    public bool isGrounded()
    {
        return controller.collisions.below;
    }
}
