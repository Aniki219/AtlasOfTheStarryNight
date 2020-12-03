using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(oscillator), typeof(Collider2D))]
public class shakeWhen : MonoBehaviour
{
    oscillator oscillator;
    public bool onTouch = true;
    public bool onHit = false;
    public bool onBonk = false;
    public float shakeDuration = 1.0f;

    private void Start()
    {
        oscillator = GetComponent<oscillator>();
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
        oscillator.SetOscillatorActive("default", false);
        oscillator.SetOscillatorActive("shake", true);
        yield return new WaitForSeconds(shakeDuration);
        oscillator.SetOscillatorActive("shake", false);
        oscillator.SetOscillatorActive("default", true);
    }
}
