using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuAtlas : MonoBehaviour
{
    bool up = true;
    bool fwd = true;

    bool canAct = true;

    float startTime;
    float timeBetweenActions = 8f;

    IEnumerator changeHeight()
    {
        up = !up;

        RectTransform t = GetComponent<RectTransform>();
        Vector3 targetPos = (up) ? new Vector3(0, 350, t.anchoredPosition3D.z) : new Vector3(0, 210, t.anchoredPosition3D.z);
        Vector3 vel = new Vector3(0, 0, 0);

        while ((targetPos - t.anchoredPosition3D).sqrMagnitude > 1f)
        {
            t.anchoredPosition3D = Vector3.SmoothDamp(t.anchoredPosition3D, targetPos, ref vel, 1.5f);
            yield return new WaitForEndOfFrame();
        }

        resetCanAct();
    }

    IEnumerator changeDepth()
    {
        fwd = !fwd;

        RectTransform t = GetComponent<RectTransform>();
        Vector3 targetSize = (fwd) ? new Vector3(0.4f, 0.4f, 0.4f) : new Vector3(0.25f, 0.25f, 0.25f);

        while (Mathf.Abs(t.localScale.x - targetSize.x) > 0.01)
        {
            t.localScale = Vector3.Lerp(t.localScale, targetSize, 0.025f);
            yield return new WaitForEndOfFrame();
        }

        resetCanAct();
    }

    void resetCanAct()
    {
        canAct = true;
        startTime = Time.time;
    }

    private void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (canAct && Time.time - startTime > timeBetweenActions)
        {
            canAct = false;
            int choice = Random.Range(0, 3);

            switch (choice)
            {
                default:
                    resetCanAct();
                    break;
                case 1:
                    StartCoroutine(changeDepth());
                    break;
                case 2:
                    StartCoroutine(changeHeight());
                    break;
            }
        }
    }
}
