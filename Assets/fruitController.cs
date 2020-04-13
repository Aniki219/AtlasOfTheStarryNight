using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fruitController : MonoBehaviour
{
    bool canWobble = true;
    bool canPickUp = false;
    Rigidbody2D rb;
    new Renderer renderer;
    Shader defaultShader;
    Shader outlineShader;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wobble());
        rb = GetComponent<Rigidbody2D>();

        renderer = GetComponentInChildren<Renderer>();
        defaultShader = renderer.material.shader;
        outlineShader = Shader.Find("Unlit/SpriteOutline");
    }

    public void fall()
    {
        rb.simulated = true;
        rb.AddForce(Vector2.up * 150f + Vector2.right * 75f * Mathf.Sign(transform.position.x - transform.parent.position.x));
        //transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        canWobble = false;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        StartCoroutine(turnIntoPickup());
    }

    public void pickUp()
    {
        rb.simulated = false;
        StartCoroutine(getEaten());
    }

    IEnumerator getEaten()
    {
        GetComponentInChildren<Animator>().SetTrigger("eat");
        GameObject atlas = GameObject.Find("Atlas");
        playerController pc = atlas.GetComponent<playerController>();
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(atlas.transform.localScale.x, 1.0f, 1.0f);
        transform.position = atlas.transform.position + atlas.transform.localScale.x * Vector3.right * 0.75f + Vector3.up * 0.75f;
        pc.startEat();
        yield return new WaitForSeconds(2.0f);
        pc.returnToMovement();
        pc.resetAnimator();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            renderer.material.shader = outlineShader;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            renderer.material.shader = defaultShader;
        }
    }

    IEnumerator turnIntoPickup()
    {
        float startTime = Time.time;

        while (Time.time - startTime < 2.0f)
        {
            rb.angularDrag += 5.0f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canPickUp = true;
        gameObject.layer = LayerMask.NameToLayer("Pickupable");
    }

    IEnumerator wobble()
    {
        float startTime = Time.time;
        float elapsedTime = 0;
        int dir = -1;
        float fullTiming = 1.5f;
        float timing = fullTiming / 2.0f;

        while (canWobble)
        {
            elapsedTime = Time.time - startTime;
            if (elapsedTime > timing)
            {
                dir *= -1;
                startTime = Time.time;
                timing = fullTiming;
            }
            transform.Rotate(new Vector3(0, 0, 1) * dir * 30 * (1.0f - elapsedTime / 2.0f / fullTiming) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
