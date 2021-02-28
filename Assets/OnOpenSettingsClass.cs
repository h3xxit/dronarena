using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOpenSettingsClass : MonoBehaviour
{
    public Slider master, music, sfx;

    public void OnOpen()
    {
        master.value = Preload.instance.masterVolume;
        music.value = Preload.instance.musicVolume;
        sfx.value = Preload.instance.sfxVolume;
    }
}
