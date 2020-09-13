using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombBerryController : MonoBehaviour
{
    Animator anim;
    bool hasBoomed = false;

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

    public void isSimulated(bool isTrue = true)
    {
        GetComponent<Rigidbody2D>().simulated = isTrue;
    }
}
