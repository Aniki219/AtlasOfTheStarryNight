using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWhenNear : MonoBehaviour
{
    SpriteRenderer sprite;
    public float requiredDistance = 1.0f;
    public float lerpSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(gameManager.Instance.player.transform.position, transform.position) < requiredDistance)
        {
            sprite.color = Vector4.MoveTowards(sprite.color, new Color(1, 1, 1, 1), lerpSpeed * Time.deltaTime);
        } else
        {
            sprite.color = Vector4.MoveTowards(sprite.color, new Color(1, 1, 1, 0), lerpSpeed * Time.deltaTime);
        }
    }
}
