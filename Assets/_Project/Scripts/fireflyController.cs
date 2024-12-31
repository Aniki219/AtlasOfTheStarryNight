using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireflyController : MonoBehaviour
{
    public Transform lightTransform;
    Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        col = GetComponent<Collider2D>();
        lightTransform.position = col.bounds.center;
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }
}
