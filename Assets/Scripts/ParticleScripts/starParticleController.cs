using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starParticleController : MonoBehaviour
{
    public float lifeTime = 0.5f;
    public Vector3 velocity = new Vector3(0, 0, 0);

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
        transform.position += velocity * Time.deltaTime;
    }
}
