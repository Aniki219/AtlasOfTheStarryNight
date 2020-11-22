using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class levelTimer : MonoBehaviour
{
    public GameObject MarsCounter;
    public GameObject GemCounter;
    int maxMars;
    Text timerText;
    Text counterText;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        counterText = GemCounter.GetComponent<Text>();
        startTime = Time.time;
        maxMars = GameObject.Find("Celestium Grid/Celestium").transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        int currentMars = MarsCounter.GetComponent<collectiblesUIController>().amountCollected;
        int currentStars = GameObject.FindGameObjectsWithTag("Star").Length;
        counterText.text = string.Concat("Gems: ", currentMars.ToString(), "/", maxMars.ToString());

        if (currentMars >= maxMars + currentStars) { return; }
        timerText.text = string.Concat("Time: ", (Time.time - startTime).ToString("F1"));
    }
}
