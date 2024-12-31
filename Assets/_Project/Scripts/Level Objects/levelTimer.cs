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
    float startx;
    float starty;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        counterText = GemCounter.GetComponent<Text>();
        startTime = Time.time;
        maxMars = GameObject.Find("Celestium Grid/Celestium").transform.childCount;
        startx = gameManager.Instance.player.transform.position.x;
        starty = gameManager.Instance.player.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startTime = Time.time;
            MarsCounter.GetComponent<collectiblesUIController>().amountCollected = 0;
            gameManager.Instance.playerCtrl.resetAnimator();
            gameManager.Instance.playerCtrl.returnToMovement();
            gameManager.Instance.clearPersistence(SceneManager.GetActiveScene().name);
            gameManager.Instance.switchScene(SceneManager.GetActiveScene().name, startx, starty);
        }

        int currentMars = MarsCounter.GetComponent<collectiblesUIController>().amountCollected;
        int currentStars = GameObject.FindGameObjectsWithTag("Star").Length;
        counterText.text = string.Concat("Gems: ", currentMars.ToString(), "/", maxMars.ToString());

        if (currentMars >= maxMars + currentStars) { return; }
        timerText.text = string.Concat("Time: ", (Time.time - startTime).ToString("F1"));
    }
}
