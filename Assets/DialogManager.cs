using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public GameObject startingStory, dialogBox;
    public TMPro.TextMeshProUGUI txt;
    public List<string> starting, win, lose;
    int nrOfWins, nrOfLosses;
    public bool dialogIsRunning;

    public void StartDialog()
    {
        Time.timeScale = 0;
        dialogIsRunning = true;
        if (!Preload.instance.showedIntro)
        {
            Preload.instance.showedIntro = true;
            startingStory.SetActive(true);
        }
        else
        {
            txt.text = starting[Random.Range(0, starting.Count)];
            dialogBox.SetActive(true);
        }
    }

    private bool isAPlayerBroken()
    {
        foreach(PlayerMove player in MecanicsManager.instance.players)
        {
            if (player.me.broken.isPlaying)
                return true;
        }
        return false;
    }

    public void FinishedRound()
    {
        if (isAPlayerBroken())
        {
            nrOfWins = 0;
            nrOfLosses++;
            if(nrOfLosses == 4)
            {
                Time.timeScale = 0;
                txt.text = lose[0];
                dialogBox.SetActive(true);
                dialogIsRunning = true;
            }
        }
        else
        {
            nrOfWins++;
            nrOfLosses = 0;
            if (nrOfWins % 2 == 0 && nrOfWins / 2 - 1 < win.Count)
            {
                Time.timeScale = 0;
                txt.text = win[nrOfWins / 2 - 1];
                dialogBox.SetActive(true);
                dialogIsRunning = true;
            }
        }
    }

    private void Start()
    {
        StartDialog();
    }

    private void Update()
    {
        if(Input.anyKey && (dialogBox.activeSelf || startingStory.activeSelf))
        {
            Time.timeScale = 1;
            dialogIsRunning = false;
            if (startingStory.activeSelf)
                startingStory.SetActive(false);
            dialogBox.SetActive(false);
        }
    }
}
