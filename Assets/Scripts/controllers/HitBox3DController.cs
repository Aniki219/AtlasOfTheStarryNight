using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox3DController : MonoBehaviour
{
    PlayerController pc;
    private void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ResetDamaging")
        {
            pc.StartBonk(1, true);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Danger"))
        {
            pc.StartBonk(1, false);
        }
    }
}
