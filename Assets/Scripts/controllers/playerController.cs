using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent (typeof (characterController))]
public class playerController : MonoBehaviour
{
    #region Vars
    //float jumpHeight = 2.2f;
    float doubleJumpHeight = 1.5f;
    float timeToJumpApex = 0.5f;
    float timeToDoubleJumpApex = 0.6f;

    int variableJumpIncrements = 6;

    float airAccelerationTime = 0.1f;
    float groundAccelerationTime = 0f;

    float moveSpeed = 4f;
    public int facing = 1;
    bool canBroom = false;
    bool canDoubleJump = false;
    bool canTornado = true;
    bool arialAttacking = false;
    [SerializeField] bool resetPosition = false;
    Vector3 lastSafePosition;
    float resetTime = 0.25f;

    doorController currentDoor = null;
    string currentDoorLabel = "none";

    float maxWallSlideVel = -5f;
    float maxFallVel = -15f;
    float wallJumpVelocity = 7;

    float gravity;
    float jumpVelocity;
    float doubleJumpVelocity;

    public Vector3 velocity;
    float velocityXSmoothing;
    Vector3 velocitySmoothing;

    [HideInInspector] public characterController controller;
    particleMaker particleMaker;
    Animator anim;
    Transform sprite;
    BoxCollider2D boxCollider;
    Deformer deformer;

    GameObject starRotator;
    Transform currentTornado;

    bool fastBroom = false;
    bool screenShake = false;
    float wallBlastDelay = 0.2f;

    Coroutine jumpCRVar;
    Coroutine turnAroundCRVar;

    private static bool created = false;

    //bool fastBroom = true;
    //bool screenShake = false;
    //float wallBlastDelay = 0f;
    #endregion

    #region State
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
        Eat,
        Attack,
        ChargeAttack
    }

    //Any state thatcannot receive damage or interact with triggers
    List<State> intangibleStates = new List<State>
    {
        State.Hurt, State.Bonk, State.Reset
    };

    List<State> moveableStates = new List<State>
    {
        State.Movement, State.Attack, State.Bonk, State.Hurt, State.Eat
    };
    #endregion
    
    #region Unity functions
    void Start()
    {
        sprite = transform.Find("AtlasSprite");

        anim = GetComponentInChildren<Animator>();
        deformer = GetComponentInChildren<Deformer>();
        controller = GetComponent<characterController>();
        particleMaker = GetComponent<particleMaker>();
        boxCollider = GetComponent<BoxCollider2D>();

        gravity = gameManager.Instance.gravity; //-(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        float djgravity = -(2 * doubleJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        doubleJumpVelocity = Mathf.Abs(djgravity) * timeToDoubleJumpApex;
        lastSafePosition = transform.position;
        warpToCurrentDoor();

        if (created) Destroy(gameObject);
        created = true;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        anim.speed = 1;
        switch (state)
        {
            case State.Movement:
                handleMovement();
                handleAttackInput();
                canPickUp();
                allowDoor();
                allowCrouch();
                break;
            case State.BroomStart:
                handleBroomStart();
                break;
            case State.Broom:
                handleBroom();
                handleAttackInput();
                break;
            case State.Attack:
                handleMovement(!isGrounded(), false, false);
                handleAttack();
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

    private void LateUpdate()
    {
        checkRoomBoundaries();
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
                Deformer nadoDeformer = other.GetComponent<Deformer>();
                if (nadoDeformer)
                {
                    nadoDeformer.startDeform(new Vector3(1.75f, .75f, 1.0f), 0.1f);
                }
            }

            if (other.CompareTag("Door"))
            {
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
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("BroomTrigger") && state == State.Broom)
        {
            other.SendMessage("OnBroomCollide");
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
    }
    #endregion

    void handleMovement(bool acceptInput = true, bool canJump = true, bool canTurnAround = true)
    {
        Vector2 input = new Vector2(0, 0);
        if (acceptInput)
        {
            input = new Vector2(AtlasInputManager.getAxisState("Dpad").x, AtlasInputManager.getAxisState("Dpad").y);
        }

        if (isGrounded())
        {
            if (AtlasInputManager.getKeyPressed("Jump") && canJump)
            {
                firstJump();
            }
            canBroom = true;
            canDoubleJump = true;
            resourceManager.Instance.restoreMana();
            if (AtlasInputManager.getKeyPressed("Broom"))
            {
                triggerBroomStart();
            }
        }
        else
        {
            if (canBroom && AtlasInputManager.getKeyPressed("Broom"))
            {
                if (isWallSliding())
                {
                    flipHorizontal();
                    triggerBroomStart(fastBroom, facing);
                } else
                {
                    triggerBroomStart(fastBroom, facing);
                }
                return;
            }

            if (isWallSliding() && AtlasInputManager.getKeyPressed("Jump") && resourceManager.Instance.getPlayerMana() >= 2)
            {
                state = State.WallJumpInit;
                return;
            }

            if (AtlasInputManager.getKeyPressed("Jump") && canJump && canDoubleJump && resourceManager.Instance.getPlayerMana() >= 1)
            {
                doubleJump();
            }
        }

        float currentMoveSpeed = moveSpeed;
        if (anim.GetBool("isCrouching"))
        {
            currentMoveSpeed = 1.5f;
        }
        float targetVelocityX = input.x * currentMoveSpeed;

        anim.SetBool("isRunning", isGrounded() && (targetVelocityX != 0));
        anim.SetBool("isJumping", !isGrounded() && (velocity.y > 0) && canDoubleJump);
        anim.SetBool("isFalling", !isGrounded() && (velocity.y < -0.5f) && !controller.collisions.descendingSlope);
        anim.SetBool("isGrounded", isGrounded());
        anim.SetBool("wallSlide", isWallSliding());

        if (canTurnAround) setFacing(velocity.x);

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? groundAccelerationTime : airAccelerationTime);
        velocity.y += gravity * Time.deltaTime;

        if (isWallSliding() && velocity.y < maxWallSlideVel) { velocity.y = maxWallSlideVel; }
        if (velocity.y < maxFallVel) { velocity.y = maxFallVel; }

        if (velocity.y <= 0 && controller.isSafePosition())
        {
            lastSafePosition = transform.position;
        }
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
                anim.SetTrigger("attackLand");
                anim.SetBool("Attacking", false);
                particleMaker.createDust();
                
            }
            arialAttacking = false;
        }
    }

    public void createHitbox(HitBox hitBox)
    {
        GameObject hb = gameManager.Instance.createInstance("AllyHitbox", transform.position + Vector3.Scale(hitBox.position, sprite.localScale), transform);
        hitBox.kbDir.x = (sprite.localScale.x > 0) ? 1 : -1;
        hb.transform.localScale = hitBox.size;
        hb.GetComponent<AllyHitBoxController>().hitbox = hitBox;
        Destroy(hb, hitBox.duration);
    }
    #endregion

    #region Interact
    void canPickUp()
    {
        if (AtlasInputManager.getKeyPressed("Crouch") && isGrounded())
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
    IEnumerator jumpCoroutine()
    {
        for (int i = 0; i < variableJumpIncrements; i++)
        {
            if (!AtlasInputManager.getKey("Jump"))
            {
                velocity.y /= 4;
                i = variableJumpIncrements;
                yield return 0;
            }
            yield return new WaitForSeconds(4 / 60.0f);
        }
    }
    IEnumerator WallJumpCoroutine()
    {
        state = State.WallJump;
        GameObject explosion = gameManager.Instance.createInstance("Effects/Explosions/wallBlast", transform.position + new Vector3(-0.80f * facing, 0.23f, 0));
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

        velocity.y = wallJumpVelocity;

        float startTime = Time.time;
        while (Time.time - startTime < 0.1f)
        {
            velocity.x = wallJumpVelocity * facing;
            velocity.y += gravity * Time.deltaTime;

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
    #endregion

    #region One-offs
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
        anim.SetBool("isCrouching", AtlasInputManager.getAxisState("Dpad").y < 0 && isGrounded());
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
        gameManager.Instance.switchScene(currentDoor.targetScene.name, 0, 0);
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
        velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, currentTornado.position, ref velocitySmoothing, 0.05f);

        if (AtlasInputManager.getKeyPressed("Jump"))
        {
            firstJump();
            anim.SetBool("inTornado", false);
            //state = State.Movement;
            returnToMovement();
            Deformer nadoDeformer = currentTornado.GetComponent<Deformer>();
            if (nadoDeformer)
            {
                nadoDeformer.startDeform(new Vector3(0.75f, 2.0f, 1.0f), 0.1f);
                SoundManager.Instance.playClip("LevelObjects/EnterTornado", 1);
            }
            currentTornado = null;
            //SoundManager.Instance.playClip("LevelObjects/WindPuff");
        }
        if (AtlasInputManager.getKeyPressed("Crouch"))
        {
            movingPlatform mp = currentTornado.GetComponent<movingPlatform>();
            if (mp)
            {
                velocity.x = mp.getVelocity().x;
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
        velocity = new Vector3(0, 0, 0);
        resetAnimator();
        anim.SetBool("eat", true);
    }
    void setFacing(float vel)
    {
        //During Movement we can keep track of the direction the player is facing each frame
        if (vel == 0) return;
        if (facing != (int)Mathf.Sign(vel))
        {
            if (anim.GetBool("isCrouching"))
            {
            } else {
                
                anim.SetTrigger("turnAround");
                facing = (int)Mathf.Sign(vel);
            }
        }
        sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * facing, sprite.localScale.y, sprite.localScale.z);
    }
    public void OnBonkCeiling(float vy)
    {
        if (state == State.Movement)
        deformer.startDeform(new Vector3(1.2f, 0.8f, 1.0f), 0.2f, 0.25f, 1.0f);
        gameManager.Instance.createInstance("Effects/StarParticleSpread", transform.position + 0.2f * Vector3.up);
    }
    public void OnLanding()
    {
        deformer.startDeform(new Vector3(1.15f, 0.85f, 1.0f), 0.125f, 0.125f, -1.0f);
        particleMaker.createDust(true);
    }
    public void hitLag(float duration = 0.1f)
    {
        StartCoroutine(doHitLag(duration));
    }
    IEnumerator doHitLag(float duration)
    {
        Vector3 oldVelocity = velocity;
        velocity = Vector3.zero;
        State oldState = state;
        state = State.Wait;
        deformer.flashWhite();
        yield return new WaitForSeconds(duration);
        state = oldState;
        //velocity = oldVelocity;
    }
    #endregion

    #region Jumps & WallJumps
    void wallJumpInit()
    {
        if (resourceManager.Instance.getPlayerMana() < 2)
        {
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

    void firstJump()
    {
        deformer.startDeform(new Vector3(0.8f, 1.4f, 1.0f), 0.1f, 0.3f, 1.0f);
        velocity.y = jumpVelocity;
        SoundManager.Instance.playClip("jump2");
        jumpCRVar = StartCoroutine(jumpCoroutine());
    }

    void doubleJump()
    {
        returnToMovement();
        resourceManager.Instance.usePlayerMana(1);
        canDoubleJump = false;
        anim.SetTrigger("doubleJump");
        velocity.y = doubleJumpVelocity;
        SoundManager.Instance.playClip("doubleJump");
        if (jumpCRVar != null) StopCoroutine(jumpCRVar);
    }

    public void bounce(float bounceVelocity, string sound = "jump2")
    {
        deformer.startDeform(new Vector3(0.8f, 1.4f, 1.0f), 0.1f, 0.3f, 1.0f);
        if (state == State.Broom) endBroom();
        velocity.y = bounceVelocity;
        SoundManager.Instance.playClip(sound);
        if (jumpCRVar != null)  StopCoroutine(jumpCRVar);
    }
    #endregion

    #region Broom Mechanics
    //Popping a WooshBerry calls this.
    public void triggerBroomStart(bool fast = false, float dir = 0)
    {
        if (resetPosition || !controller.collisions.tangible) return;
        if (intangibleStates.Contains(state)) return;
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
        state = fastBroom ? State.Broom : State.Wait;
        fastBroom = false;
        velocity = Vector3.zero;
    }

    //Called by animator after hopping on broom
    public void startBroom()
    {
        resetAnimator();
        state = State.Broom;
        SoundManager.Instance.playClip("broomLaunch");
    }

    void handleBroom()
    {
        if (AtlasInputManager.getKeyPressed("Broom"))
        {
            endBroom();
            return;
        }
        if (AtlasInputManager.getKeyPressed("Jump") && canDoubleJump && resourceManager.Instance.getPlayerMana() >= 1)
        {
            endBroom();
            doubleJump();
            return;
        }
        if ((facing == -1) ? controller.collisions.left : controller.collisions.right) {
            startBonk();
            return;
        }
        float vdir = 0 * AtlasInputManager.getAxisState("Dpad").y;
        velocity.y = moveSpeed / 2.0f * vdir;
        velocity.x = moveSpeed * 2 * facing;
    }

    void endBroom()
    {
        anim.SetTrigger("broomEnd");
        eventManager.Instance.BroomCancelEvent();
        returnToMovement();
    }
    #endregion

    #region Damage, Bonking, and Reseting
    //Take whether to set resetPos or not
    //Set state to Hurt or Bonk
    public void startBonk(int damage = 0, bool reset = false)
    {
        anim.SetBool("resetSpin", reset);
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
        eventManager.Instance.BonkEvent();
        
        Camera.main.GetComponent<cameraController>().StartShake();
    }

    void bonk()
    {
        velocity.y += gravity * Time.deltaTime;
        if (!isGrounded())
        {
            velocity.x = -moveSpeed/4f * sprite.localScale.x;
        }
    }

    //Activates on State.Reset
    //Sets tangible to true
    //Returnns to movement after reaching lastSafePosition
    void handleReset(bool isSafe = false)
    {
        controller.collisions.tangible = false;
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
        transform.position = Vector3.SmoothDamp(transform.position, lastSafePosition, ref velocitySmoothing, resetTime);
    }

    //Always turns off resetPosition
    //If not in a safe spot and resetPosition is true, sets state to reset
    //Otherwise sets state to movement and sets tangible to true
    public void returnToMovement()
    {
        if (state == State.ChargeAttack || state == State.Attack)
        {
            if (!anim.GetBool("isCrouching")) resetAnimator();
        }
        if (resetPosition && !controller.isSafePosition())
        {
            state = State.Reset;
        } else
        {
            controller.collisions.tangible = true;
            state = State.Movement;

            if (!anim.GetBool("isCrouching")) anim.SetTrigger("Idle");
        }
        resetPosition = false;
        arialAttacking = false;
    }

    public void resetAnimator()
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
    #endregion

    #region Helpers
    public void cutScenePrep()
    {
        velocity = Vector3.zero;
        resetAnimator();
        state = State.Wait;
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

        AtlasSceneManager.getPlayerCoords();
        //UP
        if (transform.position.y + boxCollider.size.y / 2.0f > bounds.max.y)
        {
            AtlasSceneManager.switchScene(-Vector2.up);
        }
        //LEFT
        if (transform.position.x - boxCollider.size.x / 2.0f < bounds.min.x)
        {
            AtlasSceneManager.switchScene(-Vector2.right);
        }
        //RIGHT
        if (transform.position.x + boxCollider.size.x / 2.0f > bounds.max.x)
        {
            AtlasSceneManager.switchScene(Vector2.right);
        }
        //DOWN
        if (transform.position.y - boxCollider.size.y / 2.0f < bounds.min.y)
        {
            AtlasSceneManager.switchScene(Vector2.up);
        }
        
    }
    #endregion

    #region isBools
    public bool isGrounded()
    {
        return controller.collisions.below;
    }

    bool isWallSliding()
    {
        if (isGrounded()) { return false; }
        float hdir = AtlasInputManager.getAxisState("Dpad").x;

        if (controller.collisions.wallRideLeft && hdir == -1 || controller.collisions.wallRideRight && hdir == 1)
        {
            return true;
        }
        return false;
    }
    #endregion
}