using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using MyBox;

public class PlayerController : EntityStateMachine
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

    [Foldout("Movement Stats")] public float maxWallSlideVel = -3.5f;
    float maxFallVel = -15f;
    float fastFallVel = -25f;
    public float wallJumpVelocity = 6;
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
    public bool hasBroom = false;
    public bool hasDoubleJump = false;
    public bool hasWallJump = false;
    public bool canDoubleJump = false;
    public bool canTornado = true;
    [SerializeField] bool arialAttacking = false;
    [SerializeField] bool fastFalling = false;
    [SerializeField] bool wallRiding = false;
    public bool canBroom = false;
    public bool invulnerable = false;
    public bool dropThroughPlatforms = false;
    public bool resetPosition = false;

    float velocityXSmoothing;
    Vector3 velocitySmoothing;

    [HideInInspector] public BroomEffectsController broomEffects {get; private set;}
    [HideInInspector] public NovaManager novaManager {get; private set;}
    AtlasSpriteController spriteController;

    stepSounds steps;

    [Foldout("Object References")]
    [SerializeReference] playerCanvasController playerCanvas;

    GameObject starRotator;
    Transform hanger;
    Transform currentTornado;
    public Transform liftableObject;
    public Transform heldObject;

    bool fastBroom = false;
    public bool screenShake = false;

    Coroutine jumpCRVar;
    Coroutine invulnerableCRVar;

    [HideInInspector] public static bool created = false;
    #endregion

    #region State
    public State depState = State.Movement;
    State depPrevState = State.Movement;

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
    public override void Start()
    {
        startState = new States.Idle();
        base.Start();
        graceFrames = maxGraceFrames;
        sprite = transform.Find("AtlasSprite");
        broomEffects = GetComponentInChildren<BroomEffectsController>();
        novaManager = GetComponent<NovaManager>();
        spriteController = sprite.GetComponent<AtlasSpriteController>();

        steps = GetComponentInChildren<stepSounds>();
        hanger = transform.Find("AtlasSprite/Hanger");

        sortingOrder = sprite.GetComponent<SpriteRenderer>().sortingOrder;

        //jumpVelocity = Mathf.Abs(gameManager.Instance.gravity) * timeToJumpApex;
        float djgravity = -(2 * doubleJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        doubleJumpVelocity = Mathf.Abs(djgravity) * timeToDoubleJumpApex;
        setLastSafePosition();
        warpToCurrentDoor();
    }

    public void Awake(){
        if (created) Destroy(gameObject);
        created = true;

        DontDestroyOnLoad(gameObject);
    }

    public override void Update()
    {
        base.Update();

        if (isGrounded()) {
            canBroom = true && hasBroom;
            canDoubleJump = true && hasDoubleJump;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            gameManager.setPause(gameManager.PauseType.TOGGLE);
        }

        // if (AtlasInputManager.getKeyPressed("Cheat"))
        // {
        //     hasDoubleJump = true;
        //     hasWallJump = true;
        //     SoundManager.Instance.playClip("jump2");
        // }
        // playerShouldWait();
        // handleParticles();
        // if (depState != State.Wait && depState != State.WaitMoveable)
        // {
        //     handleUncrouch();
        //     handleHolding();
        //     handleScout();
        // }

        // switch (depState)
        // {
        //     case State.Movement:
        //         handleMovement();
        //         handleAttackInput();
        //         canPickUp();
        //         allowDoor();
        //         allowCrouch();
        //         break;
        //     case State.Slip:
        //         handleSlip();
        //         handleAttackInput();
        //         allowCrouch();
        //         break;
        //     case State.Slide:
        //         handleSlide();
        //         break;
        //     case State.BroomStart:
        //         handleBroomStart();
        //         break;
        //     case State.Broom:
        //         handleBroom();
        //         handleAttackInput();
        //         break;
        //     case State.Attack:
        //         handleAttack();
        //         handleMovement(isGrounded() ? 0f : 1.0f, false, false);
        //         break;
        //     //Hurt and Bonk look the same to the player, but have different effects
        //     case State.Bonk:
        //     case State.Hurt:
        //         bonk();
        //         break;
        //     case State.Reset:
        //         handleReset();
        //         break;
        //     case State.WaitMoveable:
        //         handleMovement(1, false, false);
        //         break;
        //     case State.WallJumpInit:
        //         wallJumpInit();
        //         break;
        //     case State.WallJump:
        //         handleWallJump();
        //         break;
        //     case State.Tornado:
        //         handleTornado();
        //         break;
        //     default:
        //         break;
        // }
        // //Debug.DrawLine(lastSafePosition, lastSafePosition + Vector3.up, Color.blue);
    }

    private void LateUpdate()
    {
        checkRoomBoundaries();
        handleControllerDebug();
    }

    private void handleControllerDebug() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            controller.debug = !controller.debug;
            stateDisplay.gameObject.SetActive(controller.debug);
            playerCanvas.Shoutout("Debug " + (controller.debug ? "On" : "Off"));
        }
            
        if (Input.GetKeyDown(KeyCode.F2)) {
            controller.showNormal = !controller.showNormal;
            playerCanvas.Shoutout("Collision Normals " + (controller.showNormal ? "On" : "Off"));
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            controller.showVelocityNormal = !controller.showVelocityNormal;
            playerCanvas.Shoutout("Velocity Normal " + (controller.showVelocityNormal ? "On" : "Off"));
        }
        if (Input.GetKeyDown(KeyCode.F4)) {
            controller.showCollisionResolution = !controller.showCollisionResolution;
            playerCanvas.Shoutout("Collision Highlights " + (controller.showCollisionResolution ? "On" : "Off"));
        }
        if (Input.GetKeyDown(KeyCode.F5))  {
            controller.highlightGrounded =!controller.highlightGrounded;
            playerCanvas.Shoutout("Highlight Grounded " + (controller.highlightGrounded ? "On" : "Off"));
        }

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
        } else {
        //TODO: NO
        deformer.endFlashColor();
        }
    }

    private void FixedUpdate()
    {
        // if (!dropThroughPlatforms && AtlasInputManager.getKey("Jump") && isCrouching())
        // {
        //     dropThroughPlatforms = true;
        //     controller.collisions.Reset();
        //     controller.checkGrounded();
        // }
        // anim.SetBool("isGrounded", isGrounded());
        // if (isGrounded())
        // {
        //     coyoteTime = maxCoyoteTime;
        // } else if (coyoteTime > 0)
        // {
        //     coyoteTime--;
        // }

        // if (depState == State.Reset || depState == State.Wait || depState == State.Menu) {
        //     controller.canMove = false;
        //     controller.canGravity = false;
        // } else
        // {
        //     controller.canMove = true;
        //     controller.canGravity = true;
        // }

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
            if (intangibleStates.Contains(depState))
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
                depState = State.Tornado;
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

            if (other.CompareTag("BroomCollectible") && depState == State.Broom && hanger.childCount == 0)
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
        if (other.gameObject.layer == LayerMask.NameToLayer("BroomTrigger") && depState == State.Broom)
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

        if (intangibleStates.Contains(depState)) return;
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
        // //TODO: this should not be here
        // if (heldObject != null)
        // {
        //     msMod *= 0.5f;
        //     canJump = false;
        // }

        // Vector2 input = new Vector2(0, 0);

        // //TODO we need a separate input method
        // if (!(depState == State.WaitMoveable && isGrounded()))
        // input = new Vector2(AtlasInputManager.getAxis("Dpad").getValue().x, AtlasInputManager.getAxis("Dpad").getValue();.y);

        // if (isGrounded() || coyoteTime > 0)
        // {
        //     if (isGrounded() && controller.collisions.getGroundSlope().y < 0.5f && controller.velocity.y < -0.5f) {
        //         resetAnimator();
        //         anim.SetTrigger("slip");
        //         depState = State.Slip;
        //         return;
        //     }
        //     if (AtlasInputManager.getKeyPressed("Jump") && canJump)
        //     {
        //         firstJump();
        //     }
        //     if (AtlasInputManager.getKeyPressed("Broom"))
        //     {
        //         triggerBroomStart();
        //     }
        //     if (isGrounded())
        //     {
        //         if (!canBroom && depState == State.Movement)
        //         {
        //             canBroom = true;
        //         }
        //         canDoubleJump = true;
        //     }
        //     wallRiding = false;
        // }
        // else
        // {
        //     if (canBroom && AtlasInputManager.getKeyPressed("Broom"))
        //     {
        //         if (wallRiding)
        //         {
        //             flipHorizontal();
        //             //TODO: we need to investigate facing i think
        //             triggerBroomStart(fastBroom, facing);
        //         } else
        //         {
        //             triggerBroomStart(fastBroom);
        //         }
        //         return;
        //     }

        //     if (hasWallJump && wallRiding && AtlasInputManager.getKeyPressed("Jump") && resourceManager.Instance.getPlayerMana() >= 2)
        //     {
        //         depState = State.WallJumpInit;
        //         return;
        //     }

        //     if (hasDoubleJump && AtlasInputManager.getKeyPressed("Jump") && canJump && canDoubleJump && resourceManager.Instance.getPlayerMana() >= 1)
        //     {
        //         doubleJump();
        //     }
        // }

        // float currentMoveSpeed = moveSpeed;
        // if (isCrouching()   )
        // {
        //     currentMoveSpeed = 1.5f;
        // }
        // float targetVelocityX = input.x * currentMoveSpeed * msMod;

        // anim.SetBool("isRunning", isGrounded() && (targetVelocityX != 0));
        // anim.SetBool("isJumping", !isGrounded() && (controller.velocity.y > 0) && canDoubleJump);
        // anim.SetBool("isFalling", !isGrounded() && (controller.velocity.y < -0.5f) && !heldObject);
        // anim.SetBool("wallSlide", wallRiding);
        // if (wallRiding)
        // {
        //     deformer.RemoveDeform("jump");
        //     deformer.RemoveDeform("fastfall");
        // }

        // if (canTurnAround) setFacing(controller.velocity.x);

        // float smoothTime = 0;
        // if (controller.collisions.getBelow()) {
        //     if (AtlasHelpers.Sign(controller.velocity.x) == AtlasHelpers.Sign(targetVelocityX) && 
        //     Mathf.Abs(targetVelocityX) >= Mathf.Abs(controller.velocity.x)) {
        //         smoothTime = groundAccelerationTime;
        //     } else {
        //         smoothTime = groundDeccelerationTime;
        //     }
        // }  else {
        //     if (AtlasHelpers.Sign(controller.velocity.x) == AtlasHelpers.Sign(targetVelocityX) && 
        //     Mathf.Abs(targetVelocityX) >= Mathf.Abs(controller.velocity.x)) {
        //         smoothTime = airAccelerationTime;
        //     } else {
        //         smoothTime = airDeccelerationTime;
        //     }
        // }
        // controller.velocity.x = Mathf.SmoothDamp(controller.velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
        // //controller.velocity.x = targetVelocityX;
        // controller.gravityMod = ((wallRiding && controller.velocity.y <= 0) ? 0.5f : 1.0f);

        // if (wallRiding && controller.velocity.y < maxWallSlideVel) { controller.velocity.y = maxWallSlideVel; }


        
        // //TODO: fast fall state
        // // if (fastFalling)
        // // {
        // //     if (AtlasInputManager.getAxis("Dpad").getValue();.y >=0 || isGrounded() || cc.velocity.y > 0)
        // //     {
        // //         fastFalling = false;
        // //     }
        // //     cc.termVel = fastFallVel;
        // //     cc.gravityMod = 2.0f;
        // // }
        // // else
        // // {
        // //     //cc.termVel = maxFallVel;
        // //     state.deformer.RemoveDeform("fastfall");
        // //     if (AtlasInputManager.getKeyPressed("Down") && !isGrounded() && (cc.velocity.y <= 0.5f || !AtlasInputManager.getKey("Jump")))
        // //     {
        // //         fastFalling = true;
        // //         state.deformer.startDeform(new Vector3(0.85f, 1.4f, 1.0f), 0.2f, -1.0f, Vector2.down, "fastfall", true);
        // //     }
        // // }


        // if (controller.velocity.y <= 0 && controller.isSafePosition())
        // {
        //     lastSafePosition = transform.position;
        // }
    }

    #region Attacking
    void handleAttackInput()
    {
        // if (AtlasInputManager.getKeyPressed("Attack"))
        // {
        //     if (depState == State.Broom) { endBroom(); }
        //     depState = State.Attack;
        //     anim.SetTrigger("SelectAttack");
        // }
    }

    void handleAttack()
    {
        // anim.SetFloat("animTime", anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        // deformer.RemoveDeform("fastfall");
        
        // if (AtlasInputManager.getKeyPressed("Attack"))
        // {
        //     state = State.Attack;
        //     anim.SetTrigger("Attack");
        // }
        // if (!isGrounded())
        // {
        //     arialAttacking = true;
        // } else
        // {
        //     if (arialAttacking)
        //     {
        //         if (fastFalling) {
        //             anim.SetTrigger("fastFall");
        //         }
        //         anim.SetTrigger("attackLand");
        //         anim.SetBool("Attacking", false);
        //         particleMaker.createDust();
        //     }
        //     arialAttacking = false;
        // }
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
        depState = State.WallJump;
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



    // public bool canLift()
    // {
    //     // if (!isGrounded()) return false;
    //     // if (depState != State.Movement) return false;
    //     // if (isCrouching()) return false;
    //     // return true;
    // }
    public void liftObject(GameObject other)
    {
        // if (!canLift()) return;
        // liftController lc = other.GetComponent<liftController>();
        // Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
        // if (lc && rb)
        // {
        //     float liftTime = 0.2f;
        //     setFacing(Mathf.Sign(other.transform.position.x - transform.position.x));
        //     rb.velocity = Vector2.zero;
        //     rb.simulated = false;
        //     lc.startLift(new Vector3(0.4f, 0.4f, 0), new Vector3(0, 0.4f, 0), transform, liftTime);
        //     resetAnimator();
        //     resetVelocity();
        //     anim.SetTrigger("Lift");
        //     anim.SetBool("isHolding", true);
        //     freezeForSeconds(liftTime + 0.15f, true);
        //     heldObject = other.transform.parent;
        // }
    }
    void handleHolding()
    {
        // if (!anim.GetBool("isHolding")) return;
        // if (!heldObject)
        // {
        //     anim.SetBool("isHolding", false);
        //     return;
        // }

        // heldObject.transform.localScale = new Vector3(facing, 1, 1);
        // if ((depState != State.Movement && depState != State.WaitMoveable) || AtlasInputManager.getKeyPressed("Down"))
        // {
        //     anim.SetBool("isHolding", false);
        //     dropHolding();
        // } else if (AtlasInputManager.getKeyPressed("Up") || AtlasInputManager.getKeyPressed("Jump")) {
        //     anim.SetBool("isHolding", false);
        //     anim.SetTrigger("Throw");
        //     freezeForSeconds(0.4f, true);
        //     Invoke("throwHolding", 0.15f);
        // }
    }
    void throwHolding()
    {
        // dropHolding(true);
    }
    void dropHolding(bool throwing = false)
    {
        // if (!heldObject) return;
        // Rigidbody2D rb = heldObject.GetComponent<Rigidbody2D>();
        // rb.simulated = true;

        // if (throwing)
        // {
        //     rb.AddForce(new Vector2(200.0f * facing, 75.0f));
        // } else
        // {
        //     rb.AddForce(new Vector2(0, 50.0f));
        // }

        // heldObject.parent = null;
        // SceneManager.MoveGameObjectToScene(heldObject.gameObject, SceneManager.GetActiveScene());
        // heldObject = null;
    }

    public void handleScout()
    {
        // if (AtlasInputManager.getKeyPressed("Scout"))
        // {
        //     //pauseUntilCondition();
        //     gameManager.Instance.pause_manager.addPause(pauseManager.PauseType.Scouter);
        //     GameObject scouter = gameManager.createInstance("UI/ScouterTarget", transform.position);
        //     Camera.main.GetComponent<cameraController>().target = scouter.transform;
        // }
    }

    public void cutScenePrep()
    {
        depState = State.Wait;
        controller.velocity = Vector2.zero;
        resetAnimator();
    }

    public bool canMovingPlatform()
    {
        return moveableStates.Contains(depState);
    }
    IEnumerator doDoor()
    {
        currentDoorLabel = currentDoor.label;
        GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasController>().doBlackout();
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
        depState = State.Eat;
        controller.velocity = new Vector3(0, 0, 0);
        resetAnimator();
        anim.SetBool("eat", true);
    }
    // public void setFacing(float vel)
    // {
    //     //During Movement we can keep track of the direction the player is facing each frame
    //     if (Mathf.Abs(vel) < 0.01f) return;
    //     if (facing != (int)Mathf.Sign(vel))
    //     {
    //         if (isCrouching())
    //         {
    //             anim.SetFloat("animDir", -1.0f);
    //         } else {
    //             if (anim.GetBool("isRunning") && !anim.GetBool("cantTurnAround")) anim.SetTrigger("turnAround");
    //             facing = (int)Mathf.Sign(vel);
    //         }
    //     } else
    //     {
    //         anim.SetFloat("animDir", 1.0f);
    //     }
    //     sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * facing, sprite.localScale.y, sprite.localScale.z);
    // }

    public void OnBonkCeiling()
    {
        // if (controller.velocity.y <= 0) return;
        // if (depState == State.Movement) {
        //     deformer.startDeform(new Vector3(1.25f, 0.75f, 1.0f), 0.05f, 0.35f, 1.0f);
        //     gameManager.createInstance("Effects/StarParticleSpread", transform.position + 0.2f * Vector3.up);
        //     controller.velocity.y = Mathf.Min(0, controller.velocity.y);
        //     controller.resetGravity();
        // }
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
        State oldState = depState;
        depState = State.Wait;
        deformer.flashColor();
        yield return new WaitForSeconds(duration);
        depState = oldState;
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
        // //Cancel jump if stuck under something while crawling
        // if (isCrouching()) {
        //     depState = State.Slide;
        //     anim.SetTrigger("slide");
        //     controller.velocity.x = facing * 6;
        //     return;
        // }

        // SoundManager.Instance.playClip("jump2");
        // deformer.startDeform(new Vector3(0.8f, 1.3f, 1.0f), 0.1f, 0.3f, Vector2.up, "jump", true);
        // particleMaker.createDust(true);

        // jumpCRVar = StartCoroutine(jumpCoroutine());
    }

    void doubleJump()
    {
        // returnToMovement();

        // canDoubleJump = false;
        // anim.SetTrigger("doubleJump");

        // SoundManager.Instance.playClip("doubleJump");
        // spriteController.doubleJumpParticle.Play();

        // if (jumpCRVar != null) StopCoroutine(jumpCRVar);
        // jumpCRVar = StartCoroutine(jumpCoroutine(true));
    }

    public void bounce(float bounceVelocity, string sound = "jump2")
    {
        deformer.startDeform(new Vector3(0.8f, 1.4f, 1.0f), 0.1f, 0.3f, Vector2.up);
        if (depState == State.Broom) endBroom();
        controller.velocity.y = bounceVelocity;
        SoundManager.Instance.playClip(sound);
        if (jumpCRVar != null)  StopCoroutine(jumpCRVar);
    }
    #endregion

    #region Broom Mechanics
    //Popping a WooshBerry calls this.
    public void triggerBroomStart(bool fast = false, float dir = 0)
    {
    //     if (resetPosition || !controller.collisions.isTangible()) return;
    //     if (intangibleStates.Contains(depState)) return;
    //     //Cancel is ceiling above while crouching
    //     if (isCrouching() && !controller.checkVertDist(0.3f)) return;
    //     if (dir == 0 && !wallRiding)
    //     {
    //         dir = AtlasInputManager.getAxis("Dpad").getValue().x;
    //     }
    //     setFacing(dir);
    //     depState = State.BroomStart;
    //     fastBroom = fast;
    //     canBroom = false;
    }
    
    //State BroomStart waits for Atlas to get on Broom. Animator calls startBroom
    // void handleBroomStart()
    // {
    //     anim.SetTrigger("broomStart");
    //     SoundManager.Instance.playClip("onBroom");
    //     deformer.RemoveDeform("fastfall");
    //     deformer.RemoveDeform("jump");
    //     if (depState == State.Attack)
    //     {
    //         setFacing(AtlasInputManager.getAxis("Dpad").getValue().x);
    //     }
    //     //if (AtlasInputManager.Instance.aimAtMouse())
    //     //{
    //     //    setFacing(AtlasInputManager.Instance.getPlayerAim(true).x);
    //     //}

    //     depState = fastBroom ? State.Broom : State.Wait;
    //     fastBroom = false;
    //     controller.velocity = Vector3.zero;
    // }

    //Called by animator after hopping on broom
    // public void startBroom()
    // {
    //     resetAnimator();
    //     depState = State.Broom;
    //     SoundManager.Instance.playClip("broomLaunch");
    //     controller.canGravity = false;
    // }

    // void handleBroom()
    // {
    //     if (AtlasInputManager.getKeyPressed("Broom") || 
    //         AtlasInputManager.getKeyPressed("Down") ||
    //         (!AtlasInputManager.getKey("Broom") && 
    //         AtlasInputManager.Instance.holdBroom))
    //     {
    //         endBroom();
    //         return;
    //     }
    //     if (AtlasInputManager.getKeyPressed("Jump") && canDoubleJump && resourceManager.Instance.getPlayerMana() >= 1)
    //     {
    //         endBroom();
    //         if (hasDoubleJump) doubleJump();
    //         return;
    //     }
    //     if ((facing == -1) ? controller.collisions.getLeft() : controller.collisions.getRight()) {
    //         if (controller.collisions.getNorms().Find(normal => 
    //             Mathf.Abs(Vector2.Dot(normal, Vector2.right)) > 0.8f) != null)
    //         {
    //             startBonk();
    //         }
    //         return;
    //     }
    //     float vdir = AtlasInputManager.getAxis("Dpad").getValue();.y;
    //     controller.velocity.y = moveSpeed / 2.0f * vdir;
    //     controller.velocity.x = moveSpeed * 2 * facing;
    // }

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
    //     anim.SetBool("resetSpin", reset);
    //     if (!controller.collisions.isTangible()) { return; }
    //     controller.collisions.setTangible(false);


   
    //     if (reset)
    //     {
    //         resetPosition = true;
    //         SoundManager.Instance.playClip("hurt2");
    //     } else { 
    //         if (damage > 0)
    //         {
    //             SoundManager.Instance.playClip("hurt");
    //             setInvulnerable(2.0f);
    //         } else
    //         {
    //             SoundManager.Instance.playClip("bonk");
    //         }
    //         createStars(transform.position);
    //     }

    //     depState = (damage > 0) ? State.Hurt : State.Bonk;
    //     resetAnimator();
    //     anim.SetTrigger("bonk");
    //     controller.lockPosition = true;
    //     deformer.startDeform(new Vector3(.25f, 1.1f, 1), 0.1f);
    //     await Task.Delay(100);
    //     controller.lockPosition = false;

    //     if (damage > 0) { resourceManager.Instance.takeDamage(damage); }
        
    //     controller.velocity.y = 4f;
        
    //     AtlasEventManager.Instance.BonkEvent();
        
    //     Camera.main.GetComponent<cameraController>().StartShake();
    }

    void bonk()
    {
        // if (!isGrounded())
        // {
        //     controller.velocity.x = -moveSpeed/4f * sprite.localScale.x;
        // }
    }

    public void drown()
    {
        if (!controller.collisions.isTangible()) { return; }
        controller.collisions.setTangible(false);
        anim.SetBool("resetSpin", true);
        resetPosition = true;
        depState = State.Wait;
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
        colliderManager.disableCollider("SecretCollider");
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
        Debug.Log("return to movement");
        // if (depState == State.ChargeAttack || depState == State.Attack)
        // {
        //     if (!isCrouching()) resetAnimator();
        // }
        if (resetPosition && !controller.isSafePosition())
        {
            depState = State.Reset;
        }
        else
        {
            controller.collisions.setTangible();
            if (returnableStates.Contains(returnState))
            {
                depState = returnState;
            } else
            {
                depState = State.Movement;
            }

            // if (!isCrouching()) anim.SetTrigger("Idle");
        }
        resetPosition = false;
        arialAttacking = false;
        anim.speed = 1;
        colliderManager.enableCollider("Secret");
        sprite.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        changeState(new States.Idle());
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
        if (returnToIdle) {
            // anim.SetTrigger("Idle");
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
            if (depState != State.Menu) {
                depPrevState = depState;
                depState = State.Menu;
            }
        } else
        {
            if (depState == State.Menu)
            {
                anim.speed = 1;
                depState = depPrevState;
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
        State prevState = depState;
        float prevAnimSpeed = anim.speed;

        controller.velocity = Vector3.zero;
        depState = State.Wait;
        anim.speed = 0;

        while (AtlasInputManager.getKey("Scout"))
        {
            yield return new WaitForEndOfFrame();
        }

        depState = prevState;
        controller.velocity = prevVelocity;
        anim.speed = prevAnimSpeed;
    }

    public void freezeForSeconds(float time, bool moveable = false)
    {
        StartCoroutine(freezeCoroutine(time, moveable));
    }

    IEnumerator freezeCoroutine(float time, bool moveable)
    {
        State prevState = depState;
        Vector3 prevVel = controller.velocity;
        anim.SetBool("isRunning", false);

        if (!moveable) controller.velocity = Vector3.zero;
        depState = moveable ? State.WaitMoveable : State.Wait;

        yield return new WaitForSeconds(time);
        depState = prevState;
        if (!moveable) controller.velocity = prevVel;
    }

//TODO: this should be on particlecreator
    public void createStars(Vector3? position = null)
    {
        if (position == null) position = transform.position;
        Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarParticles"), (Vector3)position + Vector3.up * 0.5f, Quaternion.Euler((facing == 1) ? 180 : 0, 90, 0));
    }

    //Flip horizontal without turn animation
    public void flipHorizontal()
    {
        facing = -(int)Mathf.Sign(sprite.localScale.x);
        sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * facing, 1, 1);
    }

    void checkRoomBoundaries()
    {
        BoxCollider2D boxCollider = colliderManager.getCollider();
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
    #endregion
}