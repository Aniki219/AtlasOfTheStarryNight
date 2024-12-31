using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shakeWhen : MonoBehaviour
{
    Deformer deformer;
    public bool onTouch = true;
    public bool onHit = false;
    public bool onBonk = false;
    public float shakeDuration = 1.0f;

    private void Start()
    {
        deformer = GetComponentInChildren<Deformer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        deformer.SetOscillatorActive("default", false);
        deformer.SetOscillatorActive("shake", true);
        yield return new WaitForSeconds(shakeDuration);
        deformer.SetOscillatorActive("shake", false);
        deformer.SetOscillatorActive("default", true);
    }
}
