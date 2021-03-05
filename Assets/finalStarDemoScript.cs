using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finalStarDemoScript : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    public void activate()
    {
        panel.SetActive(true);
        Destroy(gameObject, 5.0f);
    }
}
