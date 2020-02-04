using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/SoundManager")]
public class SoundManager : ScriptableObject
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    static AudioSource audioSource;

    //public SoundManager()
    //{
    //    instance = this;
    //}

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<SoundManager>("Managers")[0];
    }

    public void playClip(string clipPath)
    {
        if (!audioSource)
        {
            audioSource = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<AudioSource>();
        }

        if (audioSource)
        {
            AudioClip clip = (AudioClip)Resources.Load<AudioClip>(string.Concat("Sounds/", clipPath));
            audioSource.PlayOneShot(clip);
        }
    }
}
