using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowerBomberController : MonoBehaviour
{
    CharacterController cc;
    HealthController hc;
    Animator anim;
    Deformer deformer;

    Vector3 velocity;

    public float startDir = 1;
    public float moveSpeed = 1.0f;

    public float dropTimer = 1.5f;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        hc = GetComponent<HealthController>();
        anim = GetComponentInChildren<Animator>();
        deformer = GetComponentInChildren<Deformer>();

        

        while (startDir == 0)
        {
            startDir = Random.Range(-1, 1);
        }
        velocity = Vector3.right * startDir;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Movement and turning
        //if (hc.inHitStun) return;
        if (cc.collisions.getRight() && velocity.x > 0 ||
            cc.collisions.getLeft() && velocity.x < 0)
        {
            velocity *= -1;
        }

        cc.Move(velocity * moveSpeed * Time.deltaTime);

        timer += Time.fixedDeltaTime;
        if (timer > dropTimer)
        {
            timer = 0;
            gameManager.createInstance("LevelPrefabs/Enemies/Projectiles/flowerBomberSeed", transform.position);
        }
    }

    public void Hurt(HitBox hitbox)
    {
        if (!hitbox) return;
        float kbStrength = (hitbox.knockback ? 2.5f : 1.5f);
        float dx = hitbox.kbDir.x;
        float dy = hitbox.kbDir.y;
        velocity.x = kbStrength * dx;
        velocity.y = kbStrength * 1.5f * dy;

        anim.SetTrigger("Hurt");
        deformer.flashColor();
    }
}
