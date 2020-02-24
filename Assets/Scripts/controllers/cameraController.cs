using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class cameraController : MonoBehaviour
{
    public Transform target;
    public Collider2D roomBounds;
    Camera cam;
    Vector3 shakeValue;
    Vector3 shakeVelocity;

    float w;
    float h;

    Vector3 velocity;
    Vector3 focusVelocity;
    public float smoothingTime = 0.5f;
    public bool cameraTracking = true;
    Vector3 focusPoint;

    void Start()
    {
        cam = GetComponent<Camera>();
        h = cam.orthographicSize;
        w = h * Screen.width / Screen.height;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetVelocity = target.GetComponent<characterController>().cameraTarget;
        Vector3 targetPoint = target.transform.position;
        Vector3 to;

        if (!cameraTracking)
        {
            to = targetPoint;
            to.x = Mathf.Clamp(to.x, roomBounds.bounds.min.x + w, roomBounds.bounds.max.x - w);
            to.y = Mathf.Clamp(to.y, roomBounds.bounds.min.y + h, roomBounds.bounds.max.y - h);
            transform.position = new Vector3(to.x, to.y, transform.position.z);
            return;
        }
        Vector3 focusTarget = targetPoint + Vector3.up * Mathf.Min(0,targetVelocity.y / 10.0f);
        focusPoint = Vector3.SmoothDamp(focusPoint, focusTarget, ref focusVelocity, smoothingTime/200.0f);
        to = new Vector3(focusPoint.x, focusPoint.y, transform.position.z);

        to.x = Mathf.Clamp(to.x, roomBounds.bounds.min.x + w, roomBounds.bounds.max.x - w);
        to.y = Mathf.Clamp(to.y, roomBounds.bounds.min.y + h, roomBounds.bounds.max.y - h);

        float smoothX = Mathf.Lerp(transform.position.x, to.x, smoothingTime/10.0f);
        float smoothY = Mathf.Lerp(transform.position.y, to.y, smoothingTime/4.0f);
        transform.position = new Vector3(smoothX, smoothY, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, to, ref velocity, smoothingTime);
    }

    public void StartShake(float shakeMagnitude = 0.1f, float shakeDuration = 0.5f)
    {
        StartCoroutine(shakeCoroutine(shakeMagnitude, shakeDuration));
    }

    IEnumerator shakeCoroutine(float shakeMagnitude, float shakeDuration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < shakeDuration)
        {
            shakeValue += (Vector3)Random.insideUnitCircle * shakeMagnitude;
            yield return new WaitForEndOfFrame();
        }
        shakeValue = Vector3.zero;
    }
}
