using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateHighScore : MonoBehaviour
{
    TMPro.TextMeshProUGUI txt;

    private void Start()
    {
        txt = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Update()
    {
        if (txt.text != "Highscore " + Preload.instance.highScore)
            txt.text = "Highscore " + Preload.instance.highScore;
    }
}
