using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTestStarController : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            anim.Play("Spin");
    }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && 
            anim.HasState(0, Animator.StringToHash("Rotate"))) {
            anim.Play("Rotate");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && 
            anim.HasState(0, Animator.StringToHash("NonExistentState"))) {
            anim.Play("NonExistentState");
        }
        
    }
}
