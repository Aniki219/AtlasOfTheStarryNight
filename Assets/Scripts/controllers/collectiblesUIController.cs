﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class collectiblesUIController : MonoBehaviour
{
    public int amountCollected = 0;
    Animator anim;

    float lastCollected = 0;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void addOne()
    {
        anim.SetBool("isIn", true);
        amountCollected++;
        GetComponentInChildren<Text>().text = amountCollected.ToString();
        lastCollected = Time.time;
    }

    void Update()
    {
        if (Time.time - lastCollected > 2f)
        {
            anim.SetBool("isIn", false);
        }
    }
}
