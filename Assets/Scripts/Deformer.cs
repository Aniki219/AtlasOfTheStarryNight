using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deformer : MonoBehaviour
{
    public Material defaultMaterial;
    public Material flashMaterial;
    public void startDeform(Vector3 to, float timeTo, float timeReturn = 0.5f)
    {
        StopCoroutine("deformAndReform");
        transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1.0f, 1.0f);
        
        StartCoroutine(deformAndReform(to, timeTo, timeReturn));
    }

    public void flashWhite(float time = 0.1f)
    {
        StartCoroutine(flashCoroutine(time));
    }

    private IEnumerator flashCoroutine(float time)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
        sprite.material = flashMaterial;
        yield return new WaitForSeconds(time);
        sprite.material = defaultMaterial;
    }

    public IEnumerator deformAndReform(Vector3 to, float timeTo, float timeReturn)
    {

        float startTime = Time.time;
        float elapsedTime = 0;
        to.x *= Mathf.Sign(transform.localScale.x);

        do
        {
            transform.localScale = Vector3.Lerp(transform.localScale, to, elapsedTime / timeTo);
            elapsedTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        } while (elapsedTime < timeTo);

        startTime = Time.time;
        elapsedTime = 0;

        do
        {
            Vector3 returnScale = new Vector3(Mathf.Sign(transform.localScale.x), 1.0f, 1.0f);
            transform.localScale = Vector3.Lerp(transform.localScale, returnScale, elapsedTime / timeReturn);
            elapsedTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        } while (elapsedTime < timeReturn);
    } 
}
