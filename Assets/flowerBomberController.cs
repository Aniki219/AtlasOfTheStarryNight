using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowerBomberController : MonoBehaviour
{
    characterController cc;
    healthController hc;
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
        cc = GetComponent<characterController>();
        hc = GetComponent<healthController>();
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
        if (hc.inHitStun) return;
        if (cc.collisions.right && velocity.x > 0 ||
            cc.collisions.left && velocity.x < 0)
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

    public void Hurt()
    {
        anim.SetTrigger("Hurt");
        deformer.flashWhite(0.2f);
    }
}
