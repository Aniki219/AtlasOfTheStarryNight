﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class threadtest : MonoBehaviour
{
    public bool isPaused = false;
    public Vector3 velocity;

    public float remainingTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        doTasks();
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = isPaused ? 0 : 1;

        if (Input.GetKeyDown(KeyCode.Space)) {
            isPaused = !isPaused;
        }

        transform.position += velocity * Time.deltaTime;
    }

    async void doTasks() {
        await move();
        velocity = Vector3.zero;
    }

    async Task move() {
        velocity = Vector3.right;

        await AtlasHelpers.WaitSeconds(2);
        velocity *= -1;
        await AtlasHelpers.WaitSeconds(2);
    }


}
