using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using _.Scripts.Tools;

public class AudioManager:Singleton<AudioManager>
{
    public Sound[] BGMSounds, sfxSounds,sfxLoop,enemyLoop;
    public AudioSource BGMSource, sfxSource, sfxSource2,enemySfxLoop;

    private void Start()
    {
        PlayBGM("BGM");
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
    //public void PlayUI(string name)
    //{
    //    Sound s = Array.Find(UISounds, x => x.name == name);

    //    if (s == null)
    //    {
    //        Debug.Log("sound not found");
    //    }
    //    else
    //    {
    //        UISourse.PlayOneShot(s.clip);
    //    }
    //}
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
    public void PlaySFX2(string name)
    {
        Sound s = Array.Find(sfxLoop, x => x.name == name);
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
    public void StopPlaySFX2()
    {
            sfxSource2.Stop();
    }
    public void PlayEnemyLoop(string name)
    {
        Sound s = Array.Find(enemyLoop, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            enemySfxLoop.clip = s.clip;
            enemySfxLoop.Play();
        }
    }
    public void StopEnemyLoop()
    {
        enemySfxLoop.Stop();
    }

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
        sfxSource.volume = volume;
    }
}
