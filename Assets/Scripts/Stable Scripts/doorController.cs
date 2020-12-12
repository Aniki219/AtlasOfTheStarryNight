using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class doorController : MonoBehaviour
{
    public string label = "A";
    public SceneReference targetScene;
    public bool enterable = true;

    public GameObject lockedSprite;
    public int numStars = 0;

    public void setEnterable(bool open = true)
    {
        enterable = open;
        lockedSprite.SetActive(!open);
    }

    public void Start()
    {
        if (lockedSprite != null && gameManager.numberOfStarts >= numStars)
        {
            setEnterable();
        }
    }
}
