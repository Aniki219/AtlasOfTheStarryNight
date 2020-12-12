using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class doorLabel : MonoBehaviour
{
    public Text labelText;
    public doorController door;
    // Start is called before the first frame update
    void Start()
    {
        labelText.text = door.targetScene.ScenePath;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
