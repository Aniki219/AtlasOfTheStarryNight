using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomStartFrame : MonoBehaviour
{
    private void Start()
    {
        float num = Random.Range(0f, 1f);
        Animator[] anims = GetComponentsInChildren<Animator>();
        foreach (Animator a in anims)
        {
            AnimatorStateInfo s = a.GetCurrentAnimatorStateInfo(0);
            a.Play(s.fullPathHash, -1, num);
        }
        Animator anim = GetComponent<Animator>();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, -1, num);
    }
}
