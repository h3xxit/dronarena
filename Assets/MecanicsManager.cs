using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MecanicsManager : MonoBehaviour
{
    public static MecanicsManager instance;

    public BulletPool bulletPool;
    public List<PlayerMove> players;
    public MapManager mapManager;
    public MusicManager musicManager;
    public DialogManager dialogManager;
    public Camera mainCamera;
    public GameObject upgradePanel;
    public List<UpgradeUI> playerUpgrades;
    bool ok;
    private const float maxBulletCameraShake = 0.4f;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private void Awake()
    {
        instance = this;
        mainCamera = Camera.main;
        mapManager = GetComponent<MapManager>();
        musicManager = GetComponent<MusicManager>();
        dialogManager = GetComponent<DialogManager>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        bulletPool = GameObject.FindGameObjectWithTag("BulletPool").GetComponent<BulletPool>();
        if(Preload.instance.showedIntro && Preload.instance.nrOfPlayers == 1)
        {
            players[0].indications1.SetActive(false);
            players[1].indications1.SetActive(false);
        }
        if (Preload.instance.nrOfPlayers == 1)
        {
            players[1].indications1.SetActive(false);
            players[1].gameObject.SetActive(false);
            players.RemoveAt(1);
        }
        for(int i = 0; i<players.Count; ++i)
        {
            players[i].Me.playerNr = i;
        }
    }

    private void Update()
    {
        ok = true;
        for (int i = 0; i < players.Count; i++)
            if (playerUpgrades[i].ready.interactable == true)
                ok = false;
        if (upgradePanel.activeSelf == true && ok && !inNextWave)
        {
            inNextWave = true;
            Invoke("NextWave", 0.5f);
        }
    }

    private bool inNextWave;

    public IEnumerator bulletShake()
    {
        float ampGain = 0;
        if (cinemachineBasicMultiChannelPerlin.m_AmplitudeGain < maxBulletCameraShake)
        {
            ampGain = 0.2f;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain += 0.2f;
        }
        yield return new WaitForSeconds(0.1f);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain -= ampGain;
    }

    public IEnumerator dieSlow(float duration)
    {
        if(Time.timeScale == 1)
        {
            Time.timeScale = 0.1f;
        }
        yield return new WaitForSecondsRealtime(duration);
        if (Time.timeScale == 0.1f)
        {
            Time.timeScale = 1f;
        }
    }

    void NextWave()
    {
        foreach (UpgradeUI upgradeUI in playerUpgrades)
        {
            upgradeUI.gameObject.SetActive(false);
        }
        upgradePanel.SetActive(false);
        foreach (PlayerMove player in players)
        {
            player.stunned = false;
            player.Me.init();
        }
        mapManager.station1?.StopStation();
        mapManager.station2?.StopStation();
        mapManager.difficulty++;
        if(mapManager.difficulty > Preload.instance.highScore)
        {
            Preload.instance.highScore = mapManager.difficulty;
            Preload.instance.updateHighScore();
        }
        mapManager.randomizeLevel();
        inNextWave = false;
    }
}
