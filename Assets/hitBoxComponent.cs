using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitBoxComponent : MonoBehaviour
{
    public HitBox hitBox;

    // Start is called before the first frame update
    void Start()
    {
        if (hitBox == null) { return; }
        GameObject hb = gameManager.Instance.createInstance("AllyHitbox", transform.position + Vector3.Scale(hitBox.position, transform.localScale));
        hb.transform.localScale = hitBox.size;
        hb.GetComponent<AllyHitBoxController>().hitbox = hitBox;
        Destroy(hb, hitBox.duration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
