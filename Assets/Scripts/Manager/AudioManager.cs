using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] BGMSounds, sfxSounds1, sfxSounds2;
    public AudioSource BGMSource, sfxSource1, sfxSource2;

    private void Start()
    {
        PlayBGM("BGM");
    }
    public void StopAll()
    {
        BGMSource.Stop();
        sfxSource1.Stop();
        sfxSource2.Stop();
    }
    public void PlayBGM(string name)
    {
        Sound s = Array.Find(BGMSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            BGMSource.clip = s.clip;
            BGMSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds1, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            sfxSource1.PlayOneShot(s.clip);
        }
    }
    public void PlaySFX2(string name)
    {
        Sound s = Array.Find(sfxSounds2, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            sfxSource2.clip = s.clip;
            sfxSource2.Play();
        }
    }
    // public void StopPlaySFX2()
    // {
    //         sfxSource2.Stop();
    // }
    // public void PlayEnemyLoop(string name)
    // {
    //     Sound s = Array.Find(enemyLoop, x => x.name == name);
    //     if (s == null)
    //     {
    //         Debug.Log("sound not found");
    //     }
    //     else
    //     {
    //         enemySfxLoop.clip = s.clip;
    //         enemySfxLoop.Play();
    //     }
    // }
    // public void StopEnemyLoop()
    // {
    //     enemySfxLoop.Stop();
    // }

    //public void Button_In()
    //{
    //    PlayUI("Press1");
    //}
    //public void Bottun_Press()
    //{
    //    PlayUI("Press2");
    //}
    public void BGMVolume(float volume)
    {
        BGMSource.volume = volume;
    }
    public void SFXVolume(float volume)
    {
        sfxSource1.volume = volume;
    }
}
