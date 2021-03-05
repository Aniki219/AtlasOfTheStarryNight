using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class persistance : MonoBehaviour
{
    string uid;
    public bool persistant = true;
    public bool markRemovedOnDestroy = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("speedScene1"))
        {
            persistant = false;
        }
        if (!persistant) return;
        uid = string.Concat(SceneManager.GetActiveScene().name, gameObject.name, transform.position.ToString());
        if (!gameManager.Instance.checkObjectKey(uid))
        {
            Destroy(gameObject);
            return;
        }
    }

    public void MarkRemoved()
    {
        if (persistant) gameManager.Instance.setObjectKey(uid, false);
    }

    private void OnDestroy()
    {
        if (markRemovedOnDestroy && gameObject.scene.isLoaded) //Was Deleted
        {
            MarkRemoved();
        }
    }
}
