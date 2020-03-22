using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deformer : MonoBehaviour
{
    public IEnumerator deformAndReform(Vector3 to, float timeTo, Vector3 returnScale, float timeReturn)
    {
        float startTime = Time.time;
        float elapsedTime = 0;

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
            transform.localScale = Vector3.Lerp(transform.localScale, returnScale, elapsedTime / timeReturn);
            elapsedTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        } while (elapsedTime < timeReturn);
    } 
}
