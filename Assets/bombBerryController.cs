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

    void Start()
    {
        eventManager.Instance.onBonkEvent += Boom;
        eventManager.Instance.onBroomCancel += Drop;
        anim = GetComponent<Animator>();
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
        isSimulated(false);
        anim.SetTrigger("Boom");
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

        if (collision.CompareTag("AllyHitbox") && GetComponent<Rigidbody2D>().velocity.magnitude < 1)
        {
            HitBox hb = collision.GetComponent<AllyHitBoxController>().hitbox;
            if (hb.interactBroom)
            {
                float dx = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);
                float scale = -350 * Mathf.Pow((dx - 0.5f), 2) + 200;
                GetComponent<Rigidbody2D>().AddForce(new Vector2(1.25f * hb.facing, 1) * scale);
            }
        }
    }

    IEnumerator BeginWoosh(Vector3 startPosition, float dir)
    {
        float elapsedTime = 0;
        float startTime = Time.time;
        anim.SetTrigger("Wings");
        GetComponent<Rigidbody2D>().gravityScale = 0;
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = new Vector3(0f, 0f, 0f);
        rigidbody.angularVelocity = 0;
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
