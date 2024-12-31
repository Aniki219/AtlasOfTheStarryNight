using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class collectiblesController : MonoBehaviour
{
    public string uiTag = "MarsCelestium";
    public float moveSpeed = 0.75f;

    bool collected = false;
    bool canCollect = true;
    GameObject uiElement;

    string uid;

    public string collectionClipPath = "collectCelestium2";

    private void Start()
    {
        uiElement = GameObject.FindGameObjectWithTag(uiTag);
    }

    public void collect()
    {
        collected = true;
        persistance p = GetComponent<persistance>();
        if (p != null)
        {
            p.MarkRemoved();
        }
        SoundManager.Instance.playClip(collectionClipPath);
    }

    private void Update()
    {
        if (collected)
        {
            if (canCollect)
            {
                canCollect = false;

                collectiblesUIController controller = uiElement.transform.parent.GetComponent<collectiblesUIController>();
                controller.addOne();
            }

            cameraController cc = Camera.main.GetComponent<cameraController>();
            Vector3 screenPoint = new Vector3(0, 1, 0);
            Vector3 worldPos = Camera.main.ViewportToWorldPoint(screenPoint) + uiElement.transform.position/128.0f;
            transform.position = Vector3.MoveTowards(transform.position, worldPos, moveSpeed);

            if (Vector3.Magnitude(transform.position - worldPos) < 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}

