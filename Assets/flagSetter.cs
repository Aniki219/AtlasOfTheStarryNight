using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flagSetter : MonoBehaviour
{
    public bool whiteoutScreen = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setFlag(string flagname)
    {
        gameFlagsManager.Instance.setFlag(flagname);
    }
}
