using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombBerryController : MonoBehaviour
{
    Animator anim;
    bool hasBoomed = false;
    bool flying = false;
    Vector3 dir;
    float flySpeed = 5.0f;
    Rigidbody2D rb;

    void Start()
    {
        eventManager.Instance.onBonkEvent += Boom;
        eventManager.Instance.onBroomCancel += Drop;
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
            transform.Translate(dir * flySpeed * Time.deltaTime, Space.World);
        }
    }

    void Boom()
    {
        hasBoomed = true;
        flying = false;
        rb.velocity = Vector2.zero;
        isSimulated(false);
        anim.SetTrigger("Boom");
        SoundManager.Instance.playClip("Boom");
        GetComponent<selfDestruct>().destroyOnAnimEnd = true;
        gameManager.Instance.createInstance("Effects/Explosions/64Explosion", transform.position);
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
        eventManager.Instance.onBonkEvent -= Boom;
        eventManager.Instance.onBroomCancel -= Drop;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Boom();
        }

        if (collision.CompareTag("WooshBerryPlant"))
        {
            if (!collision.GetComponent<BerryPlantController>().canPick) return;
            StartCoroutine(BeginWoosh(collision.transform.position, collision.transform.localScale.x));
            collision.GetComponent<BerryPlantController>().pickCallback.Invoke(ScriptableObject.CreateInstance<HitBox>());
        }

        if (collision.CompareTag("BumpBerryPlant"))
        {
            BerryPlantController bc = collision.GetComponent<BerryPlantController>();
            if (!bc.canPick) return;
            bc.pickCallback.Invoke(ScriptableObject.CreateInstance<HitBox>());
            isSimulated(true);
            anim.SetTrigger("Idle");
            flying = false;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 1;
            rb.AddForce(Vector3.Scale(bc.getDir(), new Vector3(1.0f, 2.0f, 1.0f)) * 150.0f);
        }

        if (collision.CompareTag("AllyHitbox") && rb.velocity.magnitude < 1)
        {
            HitBox hb = collision.GetComponent<AllyHitBoxController>().hitbox;
            if (hb.interactBroom)
            {
                float dx = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);
                rb.AddForce(new Vector2(1.25f * hb.kbDir.x, 1) * 150);
            }
        }
    }

    IEnumerator BeginWoosh(Vector3 startPosition, float dir)
    {
        float elapsedTime = 0;
        float startTime = Time.time;
        anim.SetTrigger("Wings");
        rb.gravityScale = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.angularVelocity = 0;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

        while (elapsedTime < 0.05f)
        {
            elapsedTime = Time.time - startTime;
            transform.position = Vector3.Lerp(transform.position, startPosition, elapsedTime/0.05f);
            yield return new WaitForEndOfFrame();
        }
        flying = true;
        this.dir = Vector3.right * dir;
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
}
