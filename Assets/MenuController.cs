using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public SceneReference mainmenu;
    static bool created = false;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (created)
        {
            Destroy(gameObject);
        }
        created = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                Application.Quit();
            } else
            {
                GameObject atlas = GameObject.Find("Atlas");
                if (atlas != null) { Destroy(atlas); }
                playerController.created = false;
                //gameManager.Instance.player = null;
                gameManager.Instance.canSetPosition = false;
                SceneManager.LoadScene(mainmenu.ScenePath);
            }
        }
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
