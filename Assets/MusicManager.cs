using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip combatClip, menuClip, gameOverClip, startRoundClip;
    private AudioSource audioSource;
    private AudioSource AudioSource
    {
        get
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            return audioSource;
        }
    }

    public void StopMusic()
    {
        AudioSource.Stop();
    }

    public void PlayMenuMusic()
    {
        if (AudioSource.clip == menuClip && AudioSource.isPlaying)
            return;
        AudioSource.clip = menuClip;
        AudioSource.loop = true;
        AudioSource.Play();
    }

    public void PlayCombatMusic()
    {
        AudioSource.clip = combatClip;
        AudioSource.loop = true;
        AudioSource.Play();
    }

    public void GameOver()
    {
        AudioSource.clip = gameOverClip;
        AudioSource.loop = false;
        AudioSource.Play();
        Invoke("PlayMenuMusic", 2.5f);
    }

    public void StartRound()
    {
        AudioSource.clip = startRoundClip;
        AudioSource.loop = false;
        AudioSource.Play();
        Invoke("PlayCombatMusic", 1.5f);
    }

    private void Update()
    {
        audioSource.volume = Preload.instance.masterVolume * Preload.instance.musicVolume;
    }
}
