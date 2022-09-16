using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager Instance { get;private set; }
    public AudioSource audioSource;


    private void Awake()
    {
        Instance = this;
    }

    public void PlayMusic(string musicName,bool loop=true)
    {
        audioSource.loop = loop;
        audioSource.clip = Resources.Load<AudioClip>("AudioClips/"+musicName);
        audioSource.Play();
    }

    public void PlaySound(string soundName)
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("AudioClips/" +soundName));
    }
}
