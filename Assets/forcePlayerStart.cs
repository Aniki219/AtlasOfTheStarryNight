using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forcePlayerStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!gameManager.Instance.canSetPosition)
        {
            gameManager.Instance.player.transform.position = transform.position;
        }
    }
}
