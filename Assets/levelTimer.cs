using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class levelTimer : MonoBehaviour
{
    public GameObject MarsCounter;
    int maxMars = 20;
    Text timerText;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        int currentMars = MarsCounter.GetComponent<collectiblesController>().amountCollected;
        if (currentMars >= maxMars) { return; }
        timerText.text = string.Concat("Time: ", (Time.time - startTime).ToString("F2"));
    }
}
