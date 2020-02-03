using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    public AudioClip collectCelestium;
    public AudioClip onBroom;
    public AudioClip broomLaunch;
    public AudioClip bonk;
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

    public void playBroomLaunch()
    {
        audio.clip = broomLaunch;
        audio.Play();
    }

    public void playBonk()
    {
        audio.clip = bonk;
        audio.Play();
    }
}
