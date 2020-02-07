using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class levelTimer : MonoBehaviour
{
    public GameObject MarsCounter;
    public GameObject GemCounter;
    public int maxMars = 25;
    Text timerText;
    Text counterText;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        counterText = GemCounter.GetComponent<Text>();
        startTime = Time.time;
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

        int currentMars = MarsCounter.GetComponent<collectiblesController>().amountCollected;
        counterText.text = string.Concat("Gems: ", currentMars.ToString(), "/", maxMars.ToString());

        if (currentMars >= maxMars) { return; }
        timerText.text = string.Concat("Time: ", (Time.time - startTime).ToString("F2"));
    }
}
