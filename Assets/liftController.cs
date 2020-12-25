using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class liftController : MonoBehaviour
{
    public Transform isHandleOf;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (transform.parent == null)
        {
            transform.position = transform.position + isHandleOf.localPosition;
            isHandleOf.localPosition = Vector3.zero;
        }
    }

    public void startLift(Vector3 m_CurvePosition, Vector3 m_TargetPosition, Transform parent, float duration = 0.25f)
    {
        transform.parent = parent;
        StartCoroutine(liftCoroutine(transform.localPosition, m_CurvePosition, m_TargetPosition, duration));
    }

    IEnumerator liftCoroutine(Vector3 m_StartPosition, Vector3 m_CurvePosition, Vector3 m_TargetPosition, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = Time.time - startTime;

        while (elapsedTime <= 0.1f)
        {
            elapsedTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        }

        startTime = Time.time;
        elapsedTime = Time.time - startTime;

        while (elapsedTime <= duration)
        {
            float timer = elapsedTime/duration;
            transform.localPosition = (((1 - timer) * (1 - timer)) * m_StartPosition) + (((1 - timer) * 2.0f) * timer * m_CurvePosition) + ((timer * timer) * m_TargetPosition);
            elapsedTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
