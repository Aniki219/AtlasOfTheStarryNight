using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/SoundManager")]
public class SoundManager : ScriptableObject
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    static AudioSource audioSource;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<SoundManager>("Managers")[0];
    }

    public void playClip(string clipPath, int pitch = 0, Vector3? origin = null)
    {
        if (!audioSource)
        {
            audioSource = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<AudioSource>();
        }

        if (audioSource)
        {
            AudioClip clip = (AudioClip)Resources.Load<AudioClip>(string.Concat("Sounds/", clipPath));
            audioSource.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1.0f + pitch / 20.0f);
            if (origin == null || Vector3.Distance(Camera.main.transform.position + Vector3.forward * 10.0f, (Vector3)origin) < 9.5f)
            audioSource.PlayOneShot(clip);
        }
    }
}
