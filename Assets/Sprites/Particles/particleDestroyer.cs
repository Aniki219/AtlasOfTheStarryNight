using UnityEngine;
using System.Collections;

public class particleDestroyer : MonoBehaviour
{
    private ParticleSystem ps;


    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
        GetComponent<Renderer>().sortingLayerName = "Foreground";
    }

    public void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}