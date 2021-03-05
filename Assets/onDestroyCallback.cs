using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class onDestroyCallback : MonoBehaviour
{
    public UnityEvent pickCallback;

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            pickCallback.Invoke();
        }
    }
}
