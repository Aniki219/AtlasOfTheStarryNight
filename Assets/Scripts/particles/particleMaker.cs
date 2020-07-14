using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleMaker : MonoBehaviour
{
    public void createDust()
    {
        GameObject left = gameManager.Instance.createInstance("Effects/dustCloud", transform.position + new Vector3(-0.20f, -0.23f, 0));
        GameObject right = gameManager.Instance.createInstance("Effects/dustCloud", transform.position + new Vector3(0.20f, -0.23f, 0));
        left.GetComponent<translator>().direction = Vector2.left;
    }
}
