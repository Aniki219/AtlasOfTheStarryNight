using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ribbonLockController : MonoBehaviour
{
    Animator anim;
    bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        healthController hc = GetComponentInParent<healthController>();
        if (hc) hc.takeNoDamage = true;
        AtlasEventManager.Instance.onPlayerLand += OnLanding;
    }

    void OnLanding(bool hurt)
    {
        if (dead || hurt) return;

        GameObject[] ribbons = GameObject.FindGameObjectsWithTag("Ribbon");
        foreach (GameObject r in ribbons)
        {
            ribbonController rc = r.GetComponent<ribbonController>();
            if (!rc.collected) return;
        }

        dead = true;
        anim.SetTrigger("RemoveLock");
    }

    public void pulse()
    {
        anim.SetTrigger("Pulse");
    }

    public void selfDestruct()
    {
        healthController hc = GetComponentInParent<healthController>();
        if (hc) hc.takeNoDamage = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        
        AtlasEventManager.Instance.onPlayerLand -= OnLanding;
    }
}
