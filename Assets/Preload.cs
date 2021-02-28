using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Preload : MonoBehaviour
{
    public static Preload instance;
    [HideInInspector]
    public float masterVolume, musicVolume, sfxVolume;
    public int nrOfPlayers;
    public bool showedIntro;
    public int highScore;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
        try
        {
            if (File.Exists(Application.persistentDataPath + "highscore.txt"))
                highScore = int.Parse(File.ReadAllLines(Application.persistentDataPath + "highscore.txt")[0]);
        }catch(System.Exception)
        {
            highScore = 0;
        }
        masterVolume = 1;
        musicVolume = 1;
        sfxVolume = 1;
    }

    public void updateHighScore()
    {
        File.WriteAllText(Application.persistentDataPath + "highscore.txt", highScore + "");
    }
}
