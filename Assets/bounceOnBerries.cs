using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class bounceOnBerries : MonoBehaviour
{
    Rigidbody2D rb;
    public float startDelay = 0;
    public UnityEvent bounceCallback;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (startDelay <= 0 && collision.CompareTag("BumpBerryPlant"))
        {
            BerryPlantController bc = collision.GetComponent<BerryPlantController>();
            if (!bc || !bc.canPick) return;
            bc.pickCallback.Invoke(ScriptableObject.CreateInstance<HitBox>());

            bounceCallback?.Invoke();

            Vector2 bumpDir = new Vector2(1.0f, 2.0f);
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector3.Scale(bc.getDir(), bumpDir * 150.0f));
        }
    }
}
