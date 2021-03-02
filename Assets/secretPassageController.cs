using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class secretPassageController : MonoBehaviour
{
    Tilemap tilemap;
    Color color;
    Color startColor;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        color = tilemap.color;
        startColor = tilemap.color;
    }

    // Update is called once per frame
    void Update()
    {
        tilemap.color = Vector4.Lerp(tilemap.color, color, 3.0f*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            color = new Color(1.0f, 1.0f, 1.0f, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            color = startColor;
        }
    }
}
