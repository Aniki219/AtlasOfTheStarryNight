﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using MyBox;

public class healthController : MonoBehaviour
{
    [HideInInspector] public float hitpoints;
    public float maxHitpoints = 3;

    float smoothTime = 0.1f;
    float smoothVelocity;
    float timeSinceHit = 100f;
    float hideVelocity;

    bool dead = false;

    public bool takeOneDamage = false;
    public bool cantHitThroughWall = false;

    public bool hurtByPlayer = true;
    [ConditionalField("hurtByPlayer")] public HurtEvent hurtCallback;
    [System.Serializable]
    public class HurtEvent : UnityEvent<HitBox> {}

    public bool blockInfront = false;
    [ConditionalField("blockInfront")] public UnityEvent blockCallback;

    Slider slider;
    CanvasGroup fillParent;

    // Start is called before the first frame update
    void Start()
    {
        fillParent = GetComponentInChildren<CanvasGroup>();
        if (fillParent) fillParent.alpha = 0;
        slider = GetComponentInChildren<Slider>();

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

    public void takeDamage(float amount)
    {
        hitpoints -= amount;
        timeSinceHit = 0;
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
            gameManager.Instance.createInstance("Effects/EnemyPop", transform.position);
            GameObject star = gameManager.Instance.createInstance("Effects/StarParticles", transform.position);
            SoundManager.Instance.playClip("hurt");
            Destroy(gameObject, 0.25f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hurtByPlayer && collision.CompareTag("AllyHitbox"))
        {
            HitBox hitbox = collision.GetComponent<AllyHitBoxController>().hitbox;
            Vector3 origin = gameManager.Instance.player.transform.position;
            Vector3 dir = transform.position - origin;

            if (cantHitThroughWall)
            {
                LayerMask wallLayer = LayerMask.NameToLayer("Wall");
                if (Physics2D.RaycastAll(origin, dir, dir.magnitude, 1 << wallLayer).Length > 1)
                {
                    return;
                }
            }
            if (blockInfront && Mathf.Sign(dir.x * transform.localScale.x) != 1)
            {
                if (blockCallback != null) blockCallback.Invoke();
                return;
            }
            float dmg = 1;
            if (!takeOneDamage) {
                dmg = hitbox.damage;
            }
            takeDamage(dmg);

            //Remember to set the hurtCallback as Dynamic and not Static
            //or it will user the Inspector HitBox (which shouldn't exist)
            hurtCallback.Invoke(hitbox);
        }
    }
}
