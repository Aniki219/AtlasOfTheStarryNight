using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox3DController : MonoBehaviour
{
    playerController pc;
    private void Start()
    {
        pc = transform.parent.GetComponent<playerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ResetDamaging")
        {
            pc.startBonk(1, true);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Danger"))
        {
            pc.startBonk(1, false);
        }
    }
}
