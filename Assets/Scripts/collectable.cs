using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : MonoBehaviour
{
    public string uiTag = "MarsCelestium";
    public float moveSpeed = 0.75f;

    bool collected = false;
    bool canCollect = true;
    GameObject uiElement;

    private void Start()
    {
        GameObject uiElement = GameObject.FindGameObjectWithTag(uiTag);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collected = true;
            collider.GetComponent<soundManager>().playCollectCelestium();
        }
    }

    private void Update()
    {
        if (collected)
        {
            if (canCollect)
            {
                canCollect = false;
                uiElement = GameObject.FindGameObjectWithTag(uiTag);
                collectiblesController cc = uiElement.transform.parent.GetComponent<collectiblesController>();
                cc.addOne();
            }

            Vector3 screenPoint = uiElement.transform.position;

            //find out where this is in world space
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        
            //move towards the world space position
            transform.position = Vector3.MoveTowards(transform.position, worldPos, moveSpeed);

            if (Vector3.Magnitude(transform.position - worldPos) < 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
