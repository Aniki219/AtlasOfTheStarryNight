﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bombBerryController : MonoBehaviour
{
    Animator anim;
    bool hasBoomed = false;
    bool flying = false;
    Vector3 dir;
    float flySpeed = 5.0f;
    Rigidbody2D rb;
    public GameObject messageBubble;

    void Start()
    {
        if (transform.parent && transform.parent.name == "Hanger")
        {
            AtlasEventManager.Instance.onBonkEvent += Boom;
            AtlasEventManager.Instance.onBroomCancel += Drop;
        }
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasBoomed && anim.GetCurrentAnimatorStateInfo(0).IsName("BombBerryExplode"))
        {
            Boom();
        }
        if (flying)
        {
            messageBubble.SetActive(false);
            transform.Translate(dir * flySpeed * Time.deltaTime, Space.World);
        }
        if (messageBubble && !hasBoomed && !flying)
        {
            Image timer = messageBubble.GetComponentInChildren<Image>();
            timer.fillAmount = 1.0f - anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
        transform.Find("handle").gameObject.SetActive(!flying);

        gameObject.layer = (rb.velocity.y > 0) ? LayerMask.NameToLayer("IgnoreOneWays") : LayerMask.NameToLayer("IgnorePlayer");
    }

    public void Boom()
    {
        if (hasBoomed) return;
        messageBubble.SetActive(false);
        removeEvents();
        hasBoomed = true;
        flying = false;
        rb.velocity = Vector2.zero;
        isSimulated(false);
        anim.SetTrigger("Boom");
        SoundManager.Instance.playClip("Boom");
        GetComponent<selfDestruct>().destroyOnAnimEnd = true;
        gameManager.createInstance("Effects/Explosions/64Explosion", transform.position);
    }

    void Drop()
    {
        isSimulated();
        transform.parent = null;
        removeEvents();
    }

    private void OnDestroy()
    {
        removeEvents();
    }

    void removeEvents()
    {
        AtlasEventManager.Instance.onBonkEvent -= Boom;
        AtlasEventManager.Instance.onBroomCancel -= Drop;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Boom();
            return;
        }

        if (collision.CompareTag("WooshBerryPlant"))
        {
            BerryPlantController bc = collision.GetComponent<BerryPlantController>();
            if (!bc || !bc.canPick) return;
            BeginWoosh(bc.center, collision.transform.GetComponent<BerryPlantController>().getForward());
            collision.GetComponent<BerryPlantController>().pickCallback.Invoke(ScriptableObject.CreateInstance<HitBox>());
        }

        if (collision.CompareTag("AllyHitbox"))
        {
            AllyHitBoxController hbc = collision.GetComponent<AllyHitBoxController>();
            if (hbc.hasHit) return;
            hbc.hasHit = true;

            HitBox hb = hbc.hitbox;

            if (flying) returnToRigidbody();
            
            if (hb.interactBroom)
            {
                float dx = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.Scale(new Vector2(1.25f, 1), hb.kbDir) * 150);
            }
        }
    }

    public void returnToRigidbody()
    {
        Debug.Log("return to rigidbody");
        isSimulated(true);
        messageBubble.SetActive(true);
        anim.SetBool("Wings", false);
        anim.SetTrigger("Idle");
        flying = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;
    }

    void BeginWoosh(Vector3 startPosition, Vector2 dir)
    {
        anim.SetBool("Wings", true);
        rb.gravityScale = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.angularVelocity = 0;
        rb.rotation = 0;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        transform.position = startPosition;
        Physics2D.SyncTransforms();
        this.dir = dir;
        flying = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (flying)
        {
            Boom();
        }
    }

    public void isSimulated(bool isTrue = true)
    {
        GetComponent<Rigidbody2D>().simulated = isTrue;
    }

    public void BroomPickUp()
    {
        isSimulated(false);
        removeEvents();
        AtlasEventManager.Instance.onBonkEvent += Boom;
        AtlasEventManager.Instance.onBroomCancel += Drop;
    }
}
