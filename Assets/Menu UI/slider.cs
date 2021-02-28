using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slider : MonoBehaviour
{
    public void MasterVolumeChange(float f)
    {
        Preload.instance.masterVolume = f;
    }
    public void MusicVolumeChange(float f)
    {
        Preload.instance.musicVolume = f;
    }
    public void SfxVolumeChange(float f)
    {
        Preload.instance.sfxVolume = f;
    }
    public void setFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
