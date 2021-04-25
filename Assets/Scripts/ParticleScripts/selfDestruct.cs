using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfDestruct : MonoBehaviour
{
    public float lifetime = Mathf.Infinity;
    public bool destroyOnAnimEnd = false;
    public bool destroyIfNoChildren = false;

    public void destructSelf()
    {
        Destroy(gameObject);
    }

    public void Start()
    {
        if (lifetime != Mathf.Infinity) Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (destroyOnAnimEnd)
        {
            Animator anim = GetComponent<Animator>();
            float normTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (normTime >= 1)
            {
                destructSelf();
            }
        }
        if (destroyIfNoChildren && transform.childCount == 0)
        {
            destructSelf();
        }
    }
}
