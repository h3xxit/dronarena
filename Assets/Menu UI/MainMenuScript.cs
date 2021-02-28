using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public Slider master, music, sfx;

    public void OnOpen()
    {
        master.value = Preload.instance.masterVolume;
        music.value = Preload.instance.musicVolume;
        sfx.value = Preload.instance.sfxVolume;
    }

    public void PlaySolo()
    {
        Preload.instance.nrOfPlayers = 1;
        SceneManager.LoadScene(2);
    }

    public void PlayDuo()
    {
        Preload.instance.nrOfPlayers = 2;
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
