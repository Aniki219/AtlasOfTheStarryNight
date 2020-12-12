using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showStars : MonoBehaviour
{
    public int num = 1;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.numberOfStarts >= num)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
