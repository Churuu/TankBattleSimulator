using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySound : MonoBehaviour
{

    AudioSource audioSource;

    public void PlayClip(AudioClip clip)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = Settings.settings.volume;
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(gameObject, clip.length+0.1f);
    }

}
