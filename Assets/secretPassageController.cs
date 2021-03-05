using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class secretPassageController : MonoBehaviour
{
    Tilemap tilemap;
    Color color;
    Color startColor;
    bool hasPlayedSound;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        color = tilemap.color;
        startColor = tilemap.color;
        hasPlayedSound = false;
    }

    // Update is called once per frame
    void Update()
    {
        tilemap.color = Vector4.Lerp(tilemap.color, color, 3.0f*Time.deltaTime);
        if (!hasPlayedSound && tilemap.color.a < 0.25f)
        {
            hasPlayedSound = true;
            SoundManager.Instance.playClip("LTTP_Secret");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SecretCollider"))
        {
            color = new Color(1.0f, 1.0f, 1.0f, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SecretCollider"))
        {
            color = startColor;
        }
    }
}
