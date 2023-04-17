using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using MyBox;

[RequireComponent (typeof (characterController))]
public class playerController : MonoBehaviour
{
    #region Vars
    //float jumpHeight = 2.2f;
    float doubleJumpHeight = 1.5f;
    float timeToJumpApex = 0.5f;
    float timeToDoubleJumpApex = 0.6f;

    int variableJumpIncrements = 6;

    [Foldout("Acceleration Constants", true)]
    public float airAccelerationTime = 0.1f;
    public float airDeccelerationTime = 0.2f;
    public float groundAccelerationTime = 0.1f;
    [Foldout("Acceleration Constants")]
    public float groundDeccelerationTime = 0f;

    float resetTime = 0.25f;
    [HideInInspector] public Vector3 lastSafePosition;

    doorController currentDoor = null;
    string currentDoorLabel = "none";

    float maxWallSlideVel = -3.5f;
    float maxFallVel = -15f;
    float fastFallVel = -25f;
    float wallJumpVelocity = 6;
    int maxCoyoteTime = 3;
    int coyoteTime = 0;
    int graceFrames = 0;
    int maxGraceFrames = 0;
    int sortingOrder;

    [Foldout("Movement Stats", true)]
    public float moveSpeed = 5f;
    public float jumpVelocity = 8.8f;
    public float doubleJumpVelocity;
    public float wallBlastDelay = 0.2f;

    [Foldout("State Info", true)]
    public bool hasDoubleJump = false;
    public bool hasWallJump = false;
    public bool canDoubleJump = false;
    public bool canTornado = true;
    [SerializeField] bool arialAttacking = false;
    [SerializeField] bool fastFalling = false;
    [SerializeField] bool wallRiding = false;
    public int facing = 1;
    public bool canBroom = false;
    public bool invulnerable = false;
    public bool dropThroughPlatforms = false;
    public bool resetPosition = false;

    float velocityXSmoothing;
    Vector3 velocitySmoothing;

    [HideInInspector] public characterController controller;
    particleMaker particleMaker;
    Animator anim;
    Transform sprite;
    atlasSpriteController spriteController;
    BoxCollider2D boxCollider;
    Deformer deformer;
    stepSounds steps;

    [Foldout("Object References")]
    public Collider2D secretCollider;

    Vector3 colliderStartSize;
    Vector3 colliderStartOffset;

    GameObject starRotator;
    Transform hanger;
    Transform currentTornado;
    Transform heldObject;

    bool fastBroom = false;
    bool screenShake = false;

    Coroutine jumpCRVar;
    Coroutine invulnerableCRVar;

    [HideInInspector] public static bool created = false;
    #endregion

    #region State
    public State state = State.Movement;
    State prevState = State.Movement;

    public enum State
    {
        Wait,
        WaitMoveable,
        Menu,
        Movement,
        BroomStart,
        Broom,
        Bonk,
        Hurt,
        Reset,
        WallJumpInit,
        WallJump,
        Tornado,
        Eat,
        Attack,
        ChargeAttack,
        Slide,
        Sliding,
        Slip
    }

    //Any state thatcannot receive damage or interact with triggers
    List<State> intangibleStates = new List<State>
    {
        State.Hurt, State.Bonk, State.Reset
    };

    //Moving Platform States
    List<State> moveableStates = new List<State>
    {
        State.Movement, State.Attack, State.Bonk, State.Hurt, State.Eat, State.WaitMoveable, State.Slide, State.Slip
    };

    //States that can be returned to in resetPosition
    List<State> returnableStates = new List<State>
    {
        State.Broom
    };
    #endregion

    #region Unity functions
    void Start()
    {
        graceFrames = maxGraceFrames;
        sprite = transform.Find("AtlasSprite");
        spriteController = sprite.GetComponent<atlasSpriteController>();

        anim = GetComponentInChildren<Animator>();
        deformer = GetComponentInChildren<Deformer>();
        controller = GetComponent<characterController>();
        particleMaker = GetComponent<particleMaker>();
        boxCollider = GetComponent<BoxCollider2D>();
        steps = GetComponentInChildren<stepSounds>();
        hanger = transform.Find("AtlasSprite/Hanger");

        colliderStartSize = boxCollider.size;
        colliderStartOffset = boxCollider.offset;

        sortingOrder = sprite.GetComponent<SpriteRenderer>().sortingOrder;

        //jumpVelocity = Mathf.Abs(gameManager.Instance.gravity) * timeToJumpApex;
        float djgravity = -(2 * doubleJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        doubleJumpVelocity = Mathf.Abs(djgravity) * timeToDoubleJumpApex;
        setLastSafePosition();
        warpToCurrentDoor();

        if (created) Destroy(gameObject);
        created = true;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (AtlasInputManager.getKeyPressed("Cheat"))
        {
            hasDoubleJump = true;
            hasWallJump = true;
            SoundManager.Instance.playClip("jump2");
        }
        playerShouldWait();
        handleParticles();
        if (state != State.Wait && state != State.WaitMoveable)
        {
            handleUncrouch();
            handleHolding();
            handleScout();
        }

        switch (state)
        {
            case State.Movement:
                handleMovement();
                handleAttackInput();
                canPickUp();
                allowDoor();
                allowCrouch();
                break;
            case State.Slip:
                handleSlip();
                handleAttackInput();
                allowCrouch();
                break;
            case State.Slide:
                handleSlide();
                break;
            case State.BroomStart:
                handleBroomStart();
                break;
            case State.Broom:
                handleBroom();
                handleAttackInput();
                break;
            case State.Attack:
                handleAttack();
                handleMovement(isGrounded() ? 0f : 1.0f, false, false);
                break;
            //Hurt and Bonk look the same to the player, but have different effects
            case State.Bonk:
            case State.Hurt:
                bonk();
                break;
            case State.Reset:
                handleReset();
                break;
            case State.WaitMoveable:
                handleMovement(1, false, false);
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

    private void LateUpdate()
    {
        checkRoomBoundaries();

        if (controller.debug) {
            if (controller.highlightGrounded) {
                if (isGrounded()) {
                    FlashColor flashColor = FlashColor.builder
                                                        .withColor(Color.green)
                                                        .withTimeUnits(TimeUnits.CONTINUOUS)
                                                        .build();
                    deformer.flashColor(flashColor);
                } else {
                    FlashColor flashColor = FlashColor.builder
                                                        .withColor(Color.red)
                                                        .withTimeUnits(TimeUnits.CONTINUOUS)
                                                        .build();
                    deformer.flashColor(flashColor);
                }
            } else {
            //TODO: NO
            deformer.endFlashColor();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!dropThroughPlatforms && AtlasInputManager.getKey("Jump") && isCrouching())
        {
            dropThroughPlatforms = true;
            controller.collisions.Reset();
            controller.checkGrounded();
        }
        anim.SetBool("isGrounded", isGrounded());
        if (isGrounded())
        {
            coyoteTime = maxCoyoteTime;
        } else if (coyoteTime > 0)
        {
            coyoteTime--;
        }

        if (state == State.Reset || state == State.Wait || state == State.Menu) {
            controller.canMove = false;
            controller.canGravity = false;
        } else
        {
            controller.canMove = true;
            controller.canGravity = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        List<Collider2D> hitters = new List<Collider2D>();
        other.GetContacts(hitters);

        //Because the hitboxes appear as children, we have to filter AllyHitboxes out
        //Otherwise you could get hurt by whacking brambles or something
        //We can probably use this to implement hitlag when hurting things..
        foreach( Collider2D h in hitters)
        {
            if (h.tag == "AllyHitbox") continue;

            //if (other.transform.tag == "ResetDamaging" && !invulnerable)
            //{
            //    if (graceFrames > 0)
            //    {
            //        graceFrames--;
            //    }
            //    else
            //    {
            //        resetPosition = true;
            //    }
            //}
            if (intangibleStates.Contains(state))
            {
                return;
            }

            //if (other.gameObject.layer == LayerMask.NameToLayer("Danger") && !invulnerable)
            //{
            //    if (other.CompareTag("ResetDamaging") && graceFrames > 0) return;
            //    startBonk(1, resetPosition);
            //    return;
            //}

            if (other.tag == "Tornado" && canTornado)
            {
                resetAnimator();
                SoundManager.Instance.playClip("LevelObjects/EnterTornado");
                canTornado = false;
                state = State.Tornado;
                currentTornado = other.transform;
                Deformer nadoDeformer = other.GetComponent<Deformer>();
                if (nadoDeformer)
                {
                    nadoDeformer.startDeform(new Vector3(1.75f, .75f, 1.0f), 0.1f);
                }
            }

            if (other.CompareTag("Door"))
            {
                if (other.GetComponent<doorController>().enterable)
                currentDoor = other.GetComponent<doorController>();
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
                    }
                    else
                    {
                        numFollowers++;
                        return getFollower(fc.followedBy);
                    }
                }
                SoundManager.Instance.playClip("Collectibles/starShard", numFollowers);
            }

            if (other.CompareTag("BroomCollectible") && state == State.Broom && hanger.childCount == 0)
            {
                other.transform.parent = hanger;
                other.transform.position = hanger.position - Vector3.up * 0.15f;
                other.SendMessage("BroomPickUp");
            }

            if (other.CompareTag("Water"))
            {
                steps.isSubmerged = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("BroomTrigger") && state == State.Broom)
        {
            other.SendMessage("OnBroomCollide");
        }

        if (other.CompareTag("Liftable") && AtlasInputManager.getKeyPressed("Up", true)) {
            liftObject(other.gameObject);
        }

        if (other.CompareTag("ResetDamaging"))
        {
            if (graceFrames > 0)
            {
                graceFrames--;
            } else
            {
                resetPosition = true;
            }
        };

        if (other.CompareTag("ResetDrown"))
        {
            drown();
        }

        if (intangibleStates.Contains(state)) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Danger") && (!invulnerable || other.CompareTag("ResetDamaging")))
        {
            if (other.CompareTag("ResetDamaging") && graceFrames > 0) return;

            if (other.CompareTag("Projectile"))
            {
                projectile p = other.GetComponent<projectile>();
                if (p.hurtPlayer) p.hit();
            }

            startBonk(1, resetPosition);

            return;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Tornado")
        {
            canTornado = true;
            anim.SetBool("inTornado", false);
        }

        if (other.CompareTag("Door"))
        {
            currentDoor = null;
        }

        if (other.CompareTag("ResetDamaging"))
        {
            graceFrames = maxGraceFrames;
        }

        if (other.CompareTag("Water"))
        {
            steps.isSubmerged = false;
        }
    }
    #endregion

    void handleMovement(float msMod = 1.0f, bool canJump = true, bool canTurnAround = true)
    {
        //TODO: this should not be here
        if (heldObject != null)
        {
            msMod *= 0.5f;
            canJump = false;
        }

        Vector2 input = new Vector2(0, 0);

        //TODO we need a separate input method
        if (!(state == State.WaitMoveable && isGrounded()))
        input = new Vector2(AtlasInputManager.getAxisState("Dpad").x, AtlasInputManager.getAxisState("Dpad").y);

        if (isGrounded() || coyoteTime > 0)
        {
            if (isGrounded() && controller.collisions.getGroundSlope().y < 0.5f && controller.velocity.y < -0.5f) {
                resetAnimator();
                anim.SetTrigger("slip");
                state = State.Slip;
                return;
            }
            if (AtlasInputManager.getKeyPressed("Jump") && canJump)
            {
                firstJump();
            }
            if (AtlasInputManager.getKeyPressed("Broom"))
            {
                triggerBroomStart();
            }
            if (isGrounded())
            {
                if (!canBroom && state == State.Movement)
                {
                    canBroom = true;
                }
                canDoubleJump = true;
            }
            wallRiding = false;
        }
        else
        {
            if (canBroom && AtlasInputManager.getKeyPressed("Broom"))
            {
                if (wallRiding)
                {
                    flipHorizontal();
                    //TODO: we need to investigate facing i think
                    triggerBroomStart(fastBroom, facing);
                } else
                {
                    triggerBroomStart(fastBroom);
                }
                return;
            }
            isWallSliding();

            if (hasWallJump && wallRiding && AtlasInputManager.getKeyPressed("Jump") && resourceManager.Instance.getPlayerMana() >= 2)
            {
                state = State.WallJumpInit;
                return;
            }

            if (hasDoubleJump && AtlasInputManager.getKeyPressed("Jump") && canJump && canDoubleJump && resourceManager.Instance.getPlayerMana() >= 1)
            {
                doubleJump();
            }
        }

        float currentMoveSpeed = moveSpeed;
        if (isCrouching()   )
        {
            currentMoveSpeed = 1.5f;
        }
        float targetVelocityX = input.x * currentMoveSpeed * msMod;

        anim.SetBool("isRunning", isGrounded() && (targetVelocityX != 0));
        anim.SetBool("isJumping", !isGrounded() && (controller.velocity.y > 0) && canDoubleJump);
        anim.SetBool("isFalling", !isGrounded() && (controller.velocity.y < -0.5f) && !heldObject);
        anim.SetBool("wallSlide", wallRiding);
        if (wallRiding)
        {
            deformer.RemoveDeform("jump");
            deformer.RemoveDeform("fastfall");
        }

        if (canTurnAround) setFacing(controller.velocity.x);

        float smoothTime = 0;
        if (controller.collisions.getBelow()) {
            if (AtlasHelpers.Sign(controller.velocity.x) == AtlasHelpers.Sign(targetVelocityX) && 
            Mathf.Abs(targetVelocityX) >= Mathf.Abs(controller.velocity.x)) {
                smoothTime = groundAccelerationTime;
            } else {
                smoothTime = groundDeccelerationTime;
            }
        }  else {
            if (AtlasHelpers.Sign(controller.velocity.x) == AtlasHelpers.Sign(targetVelocityX) && 
            Mathf.Abs(targetVelocityX) >= Mathf.Abs(controller.velocity.x)) {
                smoothTime = airAccelerationTime;
            } else {
                smoothTime = airDeccelerationTime;
            }
        }
        controller.velocity.x = Mathf.SmoothDamp(controller.velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
        //controller.velocity.x = targetVelocityX;
        controller.gravityMod = ((wallRiding && controller.velocity.y <= 0) ? 0.5f : 1.0f);

        if (wallRiding && controller.velocity.y < maxWallSlideVel) { controller.velocity.y = maxWallSlideVel; }


        
        if (fastFalling)
        {
            if (AtlasInputManager.getAxisState("Dpad").y >=0 || isGrounded() || controller.velocity.y > 0)
            {
                fastFalling = false;
            }
            //controller.termVel = fastFallVel;
            controller.gravityMod = 2.0f;
        }
        else
        {
            //controller.termVel = maxFallVel;
            deformer.RemoveDeform("fastfall");
            if (AtlasInputManager.getKeyPressed("Down") && !isGrounded() && (controller.velocity.y <= 0.5f || !AtlasInputManager.getKey("Jump")))
            {
                fastFalling = true;
                deformer.startDeform(new Vector3(0.85f, 1.4f, 1.0f), 0.2f, -1.0f, -1.0f, "fastfall", true);
            }
        }


        if (controller.velocity.y <= 0 && controller.isSafePosition())
        {
            lastSafePosition = transform.position;
        }
    }

    async void handleSlide() {
        state = State.Sliding;
        await Task.Delay(300);
        resetVelocity();
        resetAnimator();
        returnToMovement();
    }

    void handleSlip() {
        controller.velocity.x = 0;
        setFacing(AtlasHelpers.Sign(controller.collisions.getGroundSlope().x));
        if (AtlasInputManager.getKeyPressed("Jump"))
        {
            novahop();
        }
        if (controller.collisions.getGroundSlope().y > 0.5f || !isGrounded()) {
            resetAnimator();
            returnToMovement();
        }
    }

    async void novahop() {
        controller.velocity = new Vector2(facing * 8, 5);
        returnToMovement();
    }

    #region Attacking
    void handleAttackInput()
    {
        if (AtlasInputManager.getKeyPressed("Attack"))
        {
            if (state == State.Broom) { endBroom(); }
            state = State.Attack;
            anim.SetTrigger("SelectAttack");
        }
    }

    void handleAttack()
    {
        anim.SetFloat("animTime", anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        deformer.RemoveDeform("fastfall");
        
        if (AtlasInputManager.getKeyPressed("Attack"))
        {
            state = State.Attack;
            anim.SetTrigger("Attack");
        }
        if (!isGrounded())
        {
            arialAttacking = true;
        } else
        {
            if (arialAttacking)
            {
                if (fastFalling) {
                    anim.SetTrigger("fastFall");
                }
                anim.SetTrigger("attackLand");
                anim.SetBool("Attacking", false);
                particleMaker.createDust();
            }
            arialAttacking = false;
        }
    }

    void handleParticles()
    {
        spriteController.dustTrail.SetActive(wallRiding && controller.velocity.y < 0);
    }

    public void createHitbox(HitBox hitBox)
    {
        HitBox derefHitBox = hitBox.clone();
        GameObject hb = gameManager.createInstance("AllyHitbox", transform.position + Vector3.Scale(derefHitBox.position, sprite.localScale), transform);
        derefHitBox.kbDir.x *= (sprite.localScale.x > 0) ? 1.0f : -1.0f;
        hb.transform.localScale = derefHitBox.size;
        hb.GetComponent<AllyHitBoxController>().hitbox = derefHitBox;
        Destroy(hb, derefHitBox.duration);
    }
    #endregion

    #region Interact
    void canPickUp()
    {
        if (AtlasInputManager.getKeyPressed("Down") && isGrounded())
        {
            RaycastHit2D pickup = Physics2D.Raycast(transform.position, -Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Pickupable"));
            if (pickup.collider != null)
            {
                pickup.transform.SendMessage("pickUp");
            }
        }
    }
    #endregion

    #region Enumerators
    //TODO: Look up UniTask to find cancellable async methods that we can replace coroutines with
    IEnumerator LerpToPosition(Vector3 pos, float time = 0.5f)
    {
        while (Vector3.SqrMagnitude(transform.position - pos) > 0.005f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocitySmoothing, time);
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }
    IEnumerator flashEffect(float duration = 1.0f)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = !GetComponentInChildren<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(0.1f);
        }
        GetComponentInChildren<SpriteRenderer>().enabled = true;
    }
    IEnumerator jumpCoroutine(bool dj = false)
    {
        controller.velocity.y = dj ? doubleJumpVelocity : jumpVelocity;
        controller.resetGravity();

        for (int i = 0; i < variableJumpIncrements; i++)
        {
            if (!AtlasInputManager.getKey("Jump"))
            {
                if (dj)
                {
                    controller.velocity.y = Mathf.Min(controller.velocity.y, doubleJumpVelocity/2.0f);
                }
                else
                {
                    controller.velocity.y /= 4;
                }
                i = variableJumpIncrements;
                yield return 0;
            }
            yield return new WaitForSeconds(4 / 60.0f);
        }
    }
    IEnumerator WallJumpCoroutine()
    {
        state = State.WallJump;
        GameObject explosion = gameManager.createInstance("Effects/Explosions/wallBlast", transform.position + new Vector3(-0.80f * facing, 0.23f, 0));
        explosion.transform.localScale = sprite.localScale;
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

        controller.velocity.y = wallJumpVelocity;

        float startTime = Time.time;
        while (Time.time - startTime < 0.1f)
        {
            controller.velocity.x = wallJumpVelocity * facing;

            if (AtlasInputManager.getKeyPressed("Broom") && canBroom)
            {
                triggerBroomStart();
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        anim.SetBool("wallBlast", false);
        returnToMovement();
    }
    IEnumerator invulnerableCoroutine(float invulnTime)
    {
        StartCoroutine(flashEffect(invulnTime));
        invulnerable = true;
        yield return new WaitForSeconds(invulnTime);
        invulnerable = false;
        invulnerableCRVar = null;
    }
    #endregion

    #region One-offs

    public void setInvulnerable(float time)
    {
        if (invulnerableCRVar != null)
        {
            StopCoroutine(invulnerableCRVar);
        }
        invulnerableCRVar = StartCoroutine(invulnerableCoroutine(time));
    }

    void allowDoor()
    {
        if (currentDoor != null && AtlasInputManager.getKeyPressed("Interact"))
        {
            cutScenePrep();
            StartCoroutine(doDoor());
        }
    }
    void allowCrouch()
    {
        if (AtlasInputManager.getAxisState("Dpad").y < 0 && isGrounded())
        {
            anim.SetBool("isCrouching", true);
        } else if (controller.checkVertDist(0.3f))
        {
            anim.SetBool("isCrouching", false);
        }
    }
    void handleUncrouch()
    {
        checkCrouchHitbox();
        if (dropThroughPlatforms && AtlasInputManager.getAxisState("Dpad").y >= 0 || isGrounded())
        {
            dropThroughPlatforms = false;
        }
    }
    void checkCrouchHitbox()
    {
        if (isCrouching())
        {
            boxCollider.size = Vector2.Scale(colliderStartSize, new Vector3(1.0f, 0.5f));
            boxCollider.offset = Vector2.up * (colliderStartOffset.y - colliderStartSize.y * 0.25f);
        }
        else if (controller.checkVertDist(0.3f))
        {
            boxCollider.size = colliderStartSize;
            boxCollider.offset = colliderStartOffset;
        }
    }
    public bool canLift()
    {
        if (!isGrounded()) return false;
        if (state != State.Movement) return false;
        if (isCrouching()) return false;
        return true;
    }
    public void liftObject(GameObject other)
    {
        if (!canLift()) return;
        liftController lc = other.GetComponent<liftController>();
        Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
        if (lc && rb)
        {
            float liftTime = 0.2f;
            setFacing(Mathf.Sign(other.transform.position.x - transform.position.x));
            rb.velocity = Vector2.zero;
            rb.simulated = false;
            lc.startLift(new Vector3(0.4f, 0.4f, 0), new Vector3(0, 0.4f, 0), transform, liftTime);
            resetAnimator();
            resetVelocity();
            anim.SetTrigger("Lift");
            anim.SetBool("isHolding", true);
            freezeForSeconds(liftTime + 0.15f, true);
            heldObject = other.transform.parent;
        }
    }
    void handleHolding()
    {
        if (!anim.GetBool("isHolding")) return;
        if (!heldObject)
        {
            anim.SetBool("isHolding", false);
            return;
        }

        heldObject.transform.localScale = new Vector3(facing, 1, 1);
        if ((state != State.Movement && state != State.WaitMoveable) || AtlasInputManager.getKeyPressed("Down"))
        {
            anim.SetBool("isHolding", false);
            dropHolding();
        } else if (AtlasInputManager.getKeyPressed("Up") || AtlasInputManager.getKeyPressed("Jump")) {
            anim.SetBool("isHolding", false);
            anim.SetTrigger("Throw");
            freezeForSeconds(0.4f, true);
            Invoke("throwHolding", 0.15f);
        }
    }
    void throwHolding()
    {
        dropHolding(true);
    }
    void dropHolding(bool throwing = false)
    {
        if (!heldObject) return;
        Rigidbody2D rb = heldObject.GetComponent<Rigidbody2D>();
        rb.simulated = true;

        if (throwing)
        {
            rb.AddForce(new Vector2(200.0f * facing, 75.0f));
        } else
        {
            rb.AddForce(new Vector2(0, 50.0f));
        }

        heldObject.parent = null;
        SceneManager.MoveGameObjectToScene(heldObject.gameObject, SceneManager.GetActiveScene());
        heldObject = null;
    }

    public void handleScout()
    {
        if (AtlasInputManager.getKeyPressed("Scout"))
        {
            //pauseUntilCondition();
            gameManager.Instance.pause_manager.addPause(pauseManager.PauseType.Scouter);
            GameObject scouter = gameManager.createInstance("UI/ScouterTarget", transform.position);
            Camera.main.GetComponent<cameraController>().target = scouter.transform;
        }
    }

    public void cutScenePrep()
    {
        state = State.Wait;
        controller.velocity = Vector2.zero;
        resetAnimator();
    }

    public bool canMovingPlatform()
    {
        return moveableStates.Contains(state);
    }
    IEnumerator doDoor()
    {
        currentDoorLabel = currentDoor.label;
        GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<canvasController>().doBlackout();
        yield return new WaitForSeconds(0.5f);
        gameManager.Instance.currentDoorLabel = currentDoor.label;
        gameManager.Instance.switchScene(currentDoor.targetScene.ScenePath, 0, 0);
    }
    void warpToCurrentDoor()
    {
        currentDoorLabel = gameManager.Instance.currentDoorLabel;
        if (currentDoorLabel != "none")
        {
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            if (doors.Length == 0)
            {
                currentDoorLabel = "none";
                return;
            }
            foreach (GameObject door in doors)
            {
                if (door.GetComponent<doorController>().label == currentDoorLabel)
                {
                    transform.position = door.transform.position;
                    return;
                }
            }
        }
    }
    void handleTornado()
    {
        canTornado = false;
        canBroom = true;
        canDoubleJump = true;
        anim.SetBool("inTornado", true);
        controller.velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, currentTornado.position, ref velocitySmoothing, 0.05f);

        if (AtlasInputManager.getKeyPressed("Jump"))
        {
            firstJump();
            anim.SetBool("inTornado", false);
            returnToMovement();
            Deformer nadoDeformer = currentTornado.GetComponent<Deformer>();
            if (nadoDeformer)
            {
                nadoDeformer.startDeform(new Vector3(0.75f, 2.0f, 1.0f), 0.1f);
                SoundManager.Instance.playClip("LevelObjects/EnterTornado", 1);
            }
            currentTornado = null;
        }
        if (AtlasInputManager.getKeyPressed("Down"))
        {
            movingPlatform mp = currentTornado.GetComponent<movingPlatform>();
            if (mp)
            {
                controller.velocity.x = mp.getVelocity().x;
            }
            Deformer nadoDeformer = currentTornado.GetComponent<Deformer>();
            if (nadoDeformer)
            {
                nadoDeformer.startDeform(new Vector3(2.0f, 0.25f, 1.0f), 0.1f);
            }
            anim.SetBool("inTornado", false);
            returnToMovement();
            currentTornado = null;
            SoundManager.Instance.playClip("LevelObjects/EnterTornado", -1);
        }
    }
    public void startEat()
    {
        state = State.Eat;
        controller.velocity = new Vector3(0, 0, 0);
        resetAnimator();
        anim.SetBool("eat", true);
    }
    public void setFacing(float vel)
    {
        //During Movement we can keep track of the direction the player is facing each frame
        if (Mathf.Abs(vel) < 0.01f) return;
        if (facing != (int)Mathf.Sign(vel))
        {
            if (isCrouching())
            {
                anim.SetFloat("animDir", -1.0f);
            } else {
                if (anim.GetBool("isRunning") && !anim.GetBool("cantTurnAround")) anim.SetTrigger("turnAround");
                facing = (int)Mathf.Sign(vel);
            }
        } else
        {
            anim.SetFloat("animDir", 1.0f);
        }
        sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * facing, sprite.localScale.y, sprite.localScale.z);
    }
    public void OnBonkCeiling()
    {
        if (controller.velocity.y <= 0) return;
        if (state == State.Movement) {
            deformer.startDeform(new Vector3(1.25f, 0.75f, 1.0f), 0.05f, 0.35f, 1.0f);
            gameManager.createInstance("Effects/StarParticleSpread", transform.position + 0.2f * Vector3.up);
            controller.velocity.y = Mathf.Min(0, controller.velocity.y);
            controller.resetGravity();
        }
    }
    public void OnLanding()
    {
        AtlasEventManager.Instance.PlayerLandEvent(resetPosition);
        // if (controller.velocity.y < -10) {
        //     deformer.startDeform(new Vector3(1.25f, 0.90f, 1.0f), 0.05f, 0.15f, -1.0f, "Landing", true);
        //     particleMaker.createDust(true);
        // }
    }
    public void hitLag(float duration = 0.1f)
    {
        StartCoroutine(doHitLag(duration));
    }
    IEnumerator doHitLag(float duration)
    {
        Vector3 oldVelocity = controller.velocity;
        controller.velocity = Vector3.zero;
        State oldState = state;
        state = State.Wait;
        deformer.flashColor();
        yield return new WaitForSeconds(duration);
        state = oldState;
        controller.velocity = oldVelocity;
    }
    #endregion

    #region Jumps & WallJumps
    void wallJumpInit()
    {
        // if (resourceManager.Instance.getPlayerMana() < 2)
        // {
        //     //state = State.Movement;
        //     returnToMovement();
        //     return;
        // }
        // resourceManager.Instance.usePlayerMana(2);
        controller.velocity.y = 0;
        StartCoroutine(WallJumpCoroutine());
    }

    void handleWallJump()
    {
    }

    void firstJump()
    {
        //Cancel jump if stuck under something while crawling
        if (isCrouching()) {
            state = State.Slide;
            anim.SetTrigger("slide");
            controller.velocity.x = facing * 6;
            return;
        }

        SoundManager.Instance.playClip("jump2");
        deformer.startDeform(new Vector3(0.8f, 1.3f, 1.0f), 0.1f, 0.3f, 1.0f, "jump", true);
        particleMaker.createDust(true);

        jumpCRVar = StartCoroutine(jumpCoroutine());
    }

    void doubleJump()
    {
        returnToMovement();

        canDoubleJump = false;
        anim.SetTrigger("doubleJump");

        SoundManager.Instance.playClip("doubleJump");
        spriteController.doubleJumpParticle.Play();

        if (jumpCRVar != null) StopCoroutine(jumpCRVar);
        jumpCRVar = StartCoroutine(jumpCoroutine(true));
    }

    public void bounce(float bounceVelocity, string sound = "jump2")
    {
        deformer.startDeform(new Vector3(0.8f, 1.4f, 1.0f), 0.1f, 0.3f, 1.0f);
        if (state == State.Broom) endBroom();
        controller.velocity.y = bounceVelocity;
        SoundManager.Instance.playClip(sound);
        if (jumpCRVar != null)  StopCoroutine(jumpCRVar);
    }
    #endregion

    #region Broom Mechanics
    //Popping a WooshBerry calls this.
    public void triggerBroomStart(bool fast = false, float dir = 0)
    {
        if (resetPosition || !controller.collisions.isTangible()) return;
        if (intangibleStates.Contains(state)) return;
        //Cancel is ceiling above while crouching
        if (isCrouching() && !controller.checkVertDist(0.3f)) return;
        if (dir == 0 && !wallRiding)
        {
            dir = AtlasInputManager.getAxisState("Dpad").x;
        }
        setFacing(dir);
        state = State.BroomStart;
        fastBroom = fast;
        canBroom = false;
    }
    
    //State BroomStart waits for Atlas to get on Broom. Animator calls startBroom
    void handleBroomStart()
    {
        anim.SetTrigger("broomStart");
        SoundManager.Instance.playClip("onBroom");
        deformer.RemoveDeform("fastfall");
        deformer.RemoveDeform("jump");
        if (state == State.Attack)
        {
            setFacing(AtlasInputManager.getAxisState("Dpad").x);
        }
        //if (AtlasInputManager.Instance.aimAtMouse())
        //{
        //    setFacing(AtlasInputManager.Instance.getPlayerAim(true).x);
        //}

        state = fastBroom ? State.Broom : State.Wait;
        fastBroom = false;
        controller.velocity = Vector3.zero;
    }

    //Called by animator after hopping on broom
    public void startBroom()
    {
        resetAnimator();
        state = State.Broom;
        SoundManager.Instance.playClip("broomLaunch");
        controller.canGravity = false;
    }

    void handleBroom()
    {
        if (AtlasInputManager.getKeyPressed("Broom") || 
            AtlasInputManager.getKeyPressed("Down") ||
            (!AtlasInputManager.getKey("Broom") && 
            AtlasInputManager.Instance.holdBroom))
        {
            endBroom();
            return;
        }
        if (AtlasInputManager.getKeyPressed("Jump") && canDoubleJump && resourceManager.Instance.getPlayerMana() >= 1)
        {
            endBroom();
            if (hasDoubleJump) doubleJump();
            return;
        }
        if ((facing == -1) ? controller.collisions.getLeft() : controller.collisions.getRight()) {
            if (Mathf.Abs(Vector2.Dot(controller.collisions.getNorm(), Vector2.right)) > 0.8f) {
                startBonk();
            }
            return;
        }
        float vdir = AtlasInputManager.getAxisState("Dpad").y;
        controller.velocity.y = moveSpeed / 2.0f * vdir;
        controller.velocity.x = moveSpeed * 2 * facing;
    }

    void endBroom()
    {
        controller.canGravity = true;
        anim.SetTrigger("broomEnd");
        AtlasEventManager.Instance.BroomCancelEvent();
        returnToMovement();
    }
    #endregion

    #region Damage, Bonking, and Reseting
    //Take whether to set resetPos or not
    //Set state to Hurt or Bonk
    public async void startBonk(int damage = 0, bool reset = false)
    {
        anim.SetBool("resetSpin", reset);
        if (!controller.collisions.isTangible()) { return; }
        controller.collisions.setTangible(false);


   
        if (reset)
        {
            resetPosition = true;
            SoundManager.Instance.playClip("hurt2");
        } else { 
            if (damage > 0)
            {
                SoundManager.Instance.playClip("hurt");
                setInvulnerable(2.0f);
            } else
            {
                SoundManager.Instance.playClip("bonk");
            }
            createStars(transform.position);
        }

        state = (damage > 0) ? State.Hurt : State.Bonk;
        resetAnimator();
        anim.SetTrigger("bonk");
        controller.lockPosition = true;
        deformer.startDeform(new Vector3(.25f, 1.1f, 1), 0.1f);
        await Task.Delay(100);
        controller.lockPosition = false;

        if (damage > 0) { resourceManager.Instance.takeDamage(damage); }

        
        controller.velocity.y = 4f;
        

        
        AtlasEventManager.Instance.BonkEvent();
        
        Camera.main.GetComponent<cameraController>().StartShake();
    }

    void bonk()
    {
        if (!isGrounded())
        {
            controller.velocity.x = -moveSpeed/4f * sprite.localScale.x;
        }
    }

    public void drown()
    {
        if (!controller.collisions.isTangible()) { return; }
        controller.collisions.setTangible(false);
        anim.SetBool("resetSpin", true);
        resetPosition = true;
        state = State.Wait;
        AtlasEventManager.Instance.BonkEvent();

        resetAnimator();
        deformer.RemoveDeform("fastfall");
        anim.SetTrigger("Drown");
    }

    //Activates on State.Reset
    //Sets tangible to true
    //Returnns to movement after reaching lastSafePosition
    void handleReset(bool isSafe = false)
    {
        secretCollider.enabled = false;
        sprite.GetComponent<SpriteRenderer>().sortingOrder = 25;
        controller.collisions.setTangible(false);
        if (!starRotator)
        {
            starRotator = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarRotator"), transform);
        }
        if (Vector3.SqrMagnitude(lastSafePosition - transform.position) < 0.01f)
        {
            transform.position = lastSafePosition;
            controller.velocity = Vector3.zero;
            returnToMovement();
            anim.SetBool("resetSpin", false);
            StartCoroutine(flashEffect());
            Destroy(starRotator);
        }
        transform.position = Vector3.SmoothDamp(transform.position, lastSafePosition, ref velocitySmoothing, resetTime);
    }

    //Always turns off resetPosition
    //If not in a safe spot and resetPosition is true, sets state to reset
    //Otherwise sets state to movement and sets tangible to true
    public void returnToMovement(State returnState = State.Movement)
    {
        if (state == State.ChargeAttack || state == State.Attack)
        {
            if (!isCrouching()) resetAnimator();
        }
        if (resetPosition && !controller.isSafePosition())
        {
            state = State.Reset;
        }
        else
        {
            controller.collisions.setTangible();
            if (returnableStates.Contains(returnState))
            {
                state = returnState;
            } else
            {
                state = State.Movement;
            }

            if (!isCrouching()) anim.SetTrigger("Idle");
        }
        resetPosition = false;
        arialAttacking = false;
        anim.speed = 1;
        secretCollider.enabled = true;
        sprite.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
    }

    public void resetAnimator(bool returnToIdle = false)
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.name == "resetSpin") continue;
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                anim.SetBool(parameter.name, false);
            }
            if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                anim.ResetTrigger(parameter.name);
            }
        }
    }

    public void pauseAnimator()
    {
        anim.speed = 0;
    }

    public void resetVelocity()
    {
        controller.velocity = Vector2.zero;
    }
    #endregion

    #region Helpers
    public void setLastSafePosition()
    {
        lastSafePosition = transform.position;
    }
    public void playerShouldWait()
    {
        if (gameManager.Instance.pauseMenus.Count > 0)
        {
            anim.speed = 0;
            if (state != State.Menu) {
                prevState = state;
                state = State.Menu;
            }
        } else
        {
            if (state == State.Menu)
            {
                anim.speed = 1;
                state = prevState;
            }
        }
    }

    public void pauseUntilCondition()
    {
        StartCoroutine(pauseCoroutine());
    }

    //Ideally we can find a way to pause until  a certain condition is met
    //But what do we do about multiple pauseCoroutines?
    IEnumerator pauseCoroutine() { 
        Vector2 prevVelocity = controller.velocity;
        State prevState = state;
        float prevAnimSpeed = anim.speed;

        controller.velocity = Vector3.zero;
        state = State.Wait;
        anim.speed = 0;

        while (AtlasInputManager.getKey("Scout"))
        {
            yield return new WaitForEndOfFrame();
        }

        state = prevState;
        controller.velocity = prevVelocity;
        anim.speed = prevAnimSpeed;
    }

    public void freezeForSeconds(float time, bool moveable = false)
    {
        StartCoroutine(freezeCoroutine(time, moveable));
    }

    IEnumerator freezeCoroutine(float time, bool moveable)
    {
        State prevState = state;
        Vector3 prevVel = controller.velocity;
        anim.SetBool("isRunning", false);

        if (!moveable) controller.velocity = Vector3.zero;
        state = moveable ? State.WaitMoveable : State.Wait;

        yield return new WaitForSeconds(time);
        state = prevState;
        if (!moveable) controller.velocity = prevVel;
    }

    void createStars(Vector3? position = null)
    {
        if (position == null) position = transform.position;
        Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarParticles"), (Vector3)position + Vector3.up * 0.5f, Quaternion.Euler((facing == 1) ? 180 : 0, 90, 0));
    }

    //Flip horizontal without turn animation
    void flipHorizontal()
    {
        facing = -(int)Mathf.Sign(sprite.localScale.x);
        sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * facing, 1, 1);
    }

    void checkRoomBoundaries()
    {
        GameObject roomBounds = GameObject.FindGameObjectWithTag("RoomBounds");
        if (roomBounds == null) throw new System.Exception("No RoomBounds in Scene: " + SceneManager.GetActiveScene().name);

        Bounds bounds = roomBounds.GetComponent<BoxCollider2D>().bounds;

        //AtlasSceneManager.getPlayerCoords();
        //UP
        if (transform.position.y + boxCollider.size.y / 2.0f > bounds.max.y)
        {
            AtlasSceneManager.switchScene(-Vector2.up, true);
        }
        //LEFT
        if (transform.position.x - boxCollider.size.x / 2.0f < bounds.min.x)
        {
            AtlasSceneManager.switchScene(-Vector2.right, true);
        }
        //RIGHT
        if (transform.position.x + boxCollider.size.x / 2.0f > bounds.max.x)
        {
            AtlasSceneManager.switchScene(Vector2.right, true);
        }
        //DOWN
        if (transform.position.y - boxCollider.size.y / 2.0f < bounds.min.y)
        {
            AtlasSceneManager.switchScene(Vector2.up, true);
        }
        
    }
    #endregion

    #region isBools
    public bool isGrounded()
    {
        return controller.collisions.isGrounded();
    }

    void isWallSliding()
    {
        float hdir = AtlasInputManager.getAxisState("Dpad").x;
        wallRiding = controller.checkWallSlide(hdir);
        // if (controller.collisions.getBelow() || controller.collisions.getAbove() || state == State.Attack) {
        //     wallRiding = false;
        //     return;
        // }
        // float hdir = AtlasInputManager.getAxisState("Dpad").x;

        // if (!wallRiding)
        // {
        //     if (controller.collisions.getLeft() && hdir == -1 || controller.collisions.getRight() && hdir == 1)
        //     {
        //         wallRiding = true;
        //         return;
        //     }
        // } else
        // {
        //     wallRiding = controller.collisions.getLeft() || controller.collisions.getRight();
        //     return;
        // }
        // wallRiding = false;
    }

    public bool isCrouching()
    {
        if (!anim.GetBool("isCrouching")) return false;
        return isGrounded();
    }
    #endregion
}