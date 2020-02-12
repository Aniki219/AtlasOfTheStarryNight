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
    public bool persistant = true;

    private void Start()
    {
        uiElement = GameObject.FindGameObjectWithTag(uiTag);
        if (SceneManager.GetActiveScene().name.Equals("speedScene1")) {
            persistant = false;
        }
        if (!persistant) return;
        uid = string.Concat(SceneManager.GetActiveScene().name, transform.position.ToString());
        if (!gameManager.Instance.checkObjectKey(uid))
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collected = true;
            SoundManager.Instance.playClip("collectCelestium2");
        }
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
                if (persistant) gameManager.Instance.setObjectKey(uid, false);
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
