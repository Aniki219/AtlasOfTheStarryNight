﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthController : MonoBehaviour
{
    [HideInInspector] public float hitpoints;
    public float maxHitpoints = 3;

    float smoothTime = 0.1f;
    float smoothVelocity;
    float timeSinceHit = 100f;
    float hideVelocity;

    bool dead = false;

    Slider slider;
    CanvasGroup fillParent;

    // Start is called before the first frame update
    void Start()
    {
        fillParent = GetComponentInChildren<CanvasGroup>();
        fillParent.alpha = 0;
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

        if (timeSinceHit >= 2f) { 
            fillParent.alpha = Mathf.SmoothDamp(fillParent.alpha, 0, ref hideVelocity, 0.25f);
        } else
        {
            fillParent.alpha = 1;
        }
        slider.value = Mathf.SmoothDamp(slider.value, hitpoints / maxHitpoints, ref smoothVelocity, smoothTime);
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
}
