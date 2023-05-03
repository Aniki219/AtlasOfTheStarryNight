using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleMaker : MonoBehaviour
{
    public void createDust(bool small = false)
    {
        GameObject left = gameManager.createInstance("Effects/dustCloud", transform.position + new Vector3(-0.20f, -0.3f, 0));
        GameObject right = gameManager.createInstance("Effects/dustCloud", transform.position + new Vector3(0.20f, -0.3f, 0));
        left.GetComponent<translator>().direction = Vector2.left;

        if (small)
        {
            left.transform.localScale = Vector3.one * 0.75f;
            right.transform.localScale = Vector3.one * 0.75f;
        }
    }

    public void createStars(Vector2 location)
    {
        gameManager.createInstance("Effects/StarParticleSpread", location);
    }
}
