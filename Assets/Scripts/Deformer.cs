using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Deformer : MonoBehaviour
{
    Material defaultMaterial;
    public Material flashMaterial;

    Collider2D col;
    Coroutine deformerCR = null;
    Vector3 startTransform;

    public void Start()
    {
        col = GetComponentInParent<Collider2D>();
        startTransform = transform.localPosition;
    }

    public void startDeform(Vector3 to, float timeTo, float timeReturn = 0.5f, float offsetDir = 0)
    {
        if (deformerCR != null)
        {
            StopCoroutine(deformerCR);
            deformerCR = null;
        }
        transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1.0f, 1.0f);
        transform.localPosition = startTransform;
        deformerCR = StartCoroutine(deformAndReform(to, timeTo, timeReturn, offsetDir));
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
        sprite.material = playerStatsManager.Instance.currentSkin;
    }

    public IEnumerator deformAndReform(Vector3 to, float timeTo, float timeReturn, float offsetDir)
    {
        float startTime = Time.time;
        float elapsedTime = 0;

        do
        {
            to.x = Mathf.Abs(to.x) * Mathf.Sign(transform.localScale.x);
            transform.localScale = Vector3.Lerp(transform.localScale, to, elapsedTime / timeTo);
            transform.localPosition = startTransform + Vector3.up * (col.bounds.extents.y - col.offset.y) * (1.0f-transform.localScale.y) * offsetDir;
            elapsedTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        } while (elapsedTime < timeTo);

        startTime = Time.time;
        elapsedTime = 0;

        do
        {
            Vector3 returnScale = new Vector3(Mathf.Sign(transform.localScale.x), 1.0f, 1.0f);
            transform.localScale = Vector3.Lerp(transform.localScale, returnScale, elapsedTime / timeReturn);
            transform.localPosition = startTransform + Vector3.up * (col.bounds.extents.y - col.offset.y) * (1.0f - transform.localScale.y) * offsetDir;
            elapsedTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        } while (elapsedTime < timeReturn);
        transform.localPosition = startTransform;
    } 
}
