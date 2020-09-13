using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfDestruct : MonoBehaviour
{
    public bool destroyOnAnimEnd = false;
    public void destructSelf()
    {
        Destroy(gameObject);
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
    }
}
