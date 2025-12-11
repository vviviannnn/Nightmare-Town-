using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource[] audios;

    private void Start()
    {
        audios = GetComponents<AudioSource>();
    }
    public void Play1()
    {
        audios[0].Play(); 
    }

    public void Play2()
    {
        audios[1].Play();
    }

    public void Play3()
    {
        audios[2].Play();
    }

    public void Play4()
    {
        audios[3].Play();
    }
}
