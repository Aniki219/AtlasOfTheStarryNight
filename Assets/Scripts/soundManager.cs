using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    public AudioClip collectCelestium;
    public AudioClip onBroom;
    new AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void playCollectCelestium()
    {
        audio.clip = collectCelestium;
        audio.Play();
    }

    public void playOnBroom()
    {
        audio.clip = onBroom;
        audio.Play();
    }
}
