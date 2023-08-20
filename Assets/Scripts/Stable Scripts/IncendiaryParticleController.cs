using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncendiaryParticleController : MonoBehaviour
{
    HealthController hc;
    public float duration = 2f;
    public float dps = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        hc = GetComponentInParent<HealthController>();
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        hc.takeDamage(dps * Time.deltaTime);
    }
}
