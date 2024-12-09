using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using MyBox;

public class HealthController : MonoBehaviour
{
    [HideInInspector] public float hitpoints;
    public float maxHitpoints = 3;

    float smoothTime = 0.1f;
    float smoothVelocity;
    float timeSinceHit = 100f;
    float hideVelocity;

    bool dead = false;

    public bool takeNoDamage = false;
    public bool takeOneDamage = false;
    public bool cantHitThroughWall = false;
    public bool preventMultihit = false;
    public bool flashWhiteOnHit = true;
    public bool receivesHitStun = true;
    public float hitStunDuration = 0.1f;
    public bool inHitStun = false;
    public float deathPause = 0;

    public bool hurtByPlayer = true;
    [ConditionalField("hurtByPlayer")] public HurtEvent hurtCallback;
    [System.Serializable]
    public class HurtEvent : UnityEvent<HitBox> { }

    public bool deadCallback = false;
    [ConditionalField("deadCallback")] public UnityEvent onDeath;

    [HideInInspector] public bool blocking = false;
    [HideInInspector] public HitBox lastHitBy = null;
    public bool blockInfront = false;
    [ConditionalField("blockInfront")] public UnityEvent blockCallback;

    Slider slider;
    CanvasGroup fillParent;
    Deformer deformer;

    public AudioClip hurtSound;

    // Start is called before the first frame update
    void Start()
    {
        fillParent = GetComponentInChildren<CanvasGroup>();
        if (fillParent) fillParent.alpha = 0;
        slider = GetComponentInChildren<Slider>();

        deformer = GetComponent<Deformer>();
        if (!deformer) deformer = GetComponentInChildren<Deformer>();

        hitpoints = maxHitpoints;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceHit += Time.deltaTime;
        if (hitpoints >= maxHitpoints)
        {
            hitpoints = maxHitpoints;
        }

        if (slider && fillParent)
        {
            if (timeSinceHit >= 2f)
            {
                fillParent.alpha = Mathf.SmoothDamp(fillParent.alpha, 0, ref hideVelocity, 0.25f);
            }
            else
            {
                fillParent.alpha = 1;
            }

            slider.value = Mathf.SmoothDamp(slider.value, hitpoints / maxHitpoints, ref smoothVelocity, smoothTime);
        }
    }

    public void takeDamage(float amount, HitBox hitbox = null, bool byPlayer = false)
    {
        if (!takeNoDamage) hitpoints -= amount;
        timeSinceHit = 0;

        if (flashWhiteOnHit)
        {
            if (deformer != null) deformer.flashColor();
            //if (byPlayer) gameManager.Instance.playerCtrl.hitLag(0);
        }

        //Remember to set the hurtCallback as Dynamic and not Static
        //or it will user the Inspector HitBox (which shouldn't exist)
        if (receivesHitStun) hitStun(hitStunDuration);
        if (hurtCallback != null) hurtCallback.Invoke(hitbox);
        if (hitbox && hitbox.incendiary)
        {
            Vector3 rpos = new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0);
            gameManager.createInstance("Effects/Fire/IncendiaryParticle", rpos + transform.position, transform);
        }
        if (hurtSound) SoundManager.Instance.playClip(hurtSound.name);

        if (!dead)
        {
            checkDead();
        }
    }

    void checkDead()
    {
        if (hitpoints <= 0)
        {
            dead = true;
            Destroy(gameObject, deathPause);
        }
    }

    public void starPop()
    {
        gameManager.createInstance("Effects/EnemyPop", transform.position);
        GameObject star = gameManager.createInstance("Effects/StarParticles", transform.position);
        SoundManager.Instance.playClip("hurt");
        foreach (Collider2D c in GetComponents<Collider2D>())
        {
            c.enabled = false;
        }
    }

    public void hitStun(float duration = 0.1f)
    {
        StartCoroutine(startHitStun(duration));
    }

    private IEnumerator startHitStun(float duration)
    {
        inHitStun = true;
        yield return new WaitForSeconds(duration);
        inHitStun = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hurtByPlayer && collision.CompareTag("AllyHitbox"))
        {
            AllyHitBoxController ac = collision.GetComponent<AllyHitBoxController>();
            //if (ac.hasHit) return;
            HitBox hitbox = ac.hitbox;
            if (preventMultihit) ac.hasHit = true;
            Vector3 origin = gameManager.Instance.player.transform.position;
            Vector3 dir = transform.position - origin;
            lastHitBy = hitbox;

            if (cantHitThroughWall)
            {
                LayerMask wallLayer = LayerMask.NameToLayer("Wall");
                if (Physics2D.RaycastAll(origin, dir, dir.magnitude, 1 << wallLayer).Length > 1)
                {
                    return;
                }
            }
            if (blocking || (blockInfront && Mathf.Sign(dir.x * transform.localScale.x) != 1))
            {
                if (blockCallback != null) blockCallback.Invoke();
                return;
            }
            float dmg = 1;
            if (!takeOneDamage)
            {
                dmg = hitbox.damage;
            }
            takeDamage(dmg, hitbox, true);
        }
    }

    private void OnDestroy()
    {
        if (dead && deadCallback) onDeath.Invoke();
    }
}
