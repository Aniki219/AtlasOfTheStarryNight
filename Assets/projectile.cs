using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public Vector2 direction;
    public float speed;
    public bool hurtPlayer = true;
    public bool hurtEnemy = false;

    public int damage = 1;

    public GameObject impactParticle;

    Bounds bounds;
    Vector2 impactPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bounds = GetComponent<Collider2D>().bounds;

        transform.Translate(direction * speed * Time.deltaTime);

        impactPosition = bounds.center;
        if (speed != 0)
        {
            if (direction.x != 0)
            {
                impactPosition.x = (Mathf.Sign(direction.x * speed) > 0) ? bounds.max.x : bounds.min.x;
            }
            if (direction.y != 0)
            {
                impactPosition.y = (Mathf.Sign(direction.y * speed) > 0) ? bounds.max.y : bounds.min.y;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            hit();
        }
    }

    public void hit()
    {
        Destroy(gameObject);
        gameManager.createInstance(impactParticle, impactPosition);
    }
}
