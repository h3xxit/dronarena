using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject[,] map = new GameObject[22, 12];
    public GameObject wall1, wall2, wall3, station1Prefab, station2Prefab, enemy1, enemy2, enemy3;
    public GameObject spawnTarget;
    public Station station1, station2;
    public string[,] reprez = new string[22, 12];
    private float distancePerUnit = 0.5f;
    private bool allClear = true;

    [HideInInspector]
    public int difficulty = 1;
    private const float MIN_PERETI = 0.3f, MAX_PERETI = 0.6f;
    [HideInInspector]
    public int nrOfEnemies;

    public void killAllEnemies()
    {
        for (int i = 0; i < 20; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (map[i, j] != null)
                {
                    if (reprez[i, j]?[0] == 'e')
                    {
                        map[i, j].GetComponent<Stats>().die();
                    }
                }
            }
        }
    }

    public void reset()
    {
        allClear = true;
        for (int i = 0; i < 20; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (map[i, j] != null)
                {
                    if (reprez[i, j]?[0] == 'w')
                    {
                        map[i, j].GetComponent<wallLogic>().Anim.Play("Disappear");
                    }
                    else if (reprez[i, j]?[0] == 'e')
                    {
                        map[i, j].GetComponent<Stats>().die();
                    }
                    else if (map[i, j] != null && reprez[i, j] != "station1" && reprez[i, j] != "station2")
                        Destroy(map[i, j]);
                }
            }
        }
        System.Array.Clear(reprez, 0, reprez.Length);
        reprez[8, 0] = "station1";
        if (MecanicsManager.instance.players.Count > 1)
            reprez[11, 0] = "station2";
    }

    public float nrPereti, nrInamici;

    public void randomizeLevel()
    {
        MecanicsManager.instance.musicManager.StartRound();
        allClear = false;
        float offset = Random.Range(0f, 100f);
        for(int i = 0; i<20; ++i)
        {
            for(int j = 0; j<10; ++j)
            {
                if(reprez[i, j] == null && Mathf.PerlinNoise(i * distancePerUnit + offset, j * distancePerUnit + offset) < nrPereti)
                {
                    reprez[i, j] = "wall" + Random.Range(1, 4);
                }
            }
        }
        nrOfEnemies = 0;
        do
        {
            offset = Random.Range(0f, 100f);
            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    if (reprez[i, j] == null && Mathf.PerlinNoise(i * distancePerUnit + offset, j * distancePerUnit + offset) < nrInamici + 0.0025 * difficulty)
                    {
                        float chance = Random.Range(0f, 1f);
                        if (chance < 0.6f - difficulty * 0.03f)
                            reprez[i, j] = "enemy1";
                        else if (chance < 0.7f)
                            reprez[i, j] = "enemy3";
                        else
                            reprez[i, j] = "enemy2";
                        nrOfEnemies++;
                    }
                }
            }
        } while (nrOfEnemies == 0);
        generateLevel();
    }

    private void generateLevel()
    {
        for (int i = 0; i < 20; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (reprez[i, j] == "wall1")
                    map[i, j] = Instantiate(wall1, spawnTarget.transform);
                else if (reprez[i, j] == "wall2")
                    map[i, j] = Instantiate(wall2, spawnTarget.transform);
                else if (reprez[i, j] == "wall3")
                    map[i, j] = Instantiate(wall3, spawnTarget.transform);
                else if (reprez[i, j] == "station1" && map[i, j] == null)
                {
                    map[i, j] = Instantiate(station1Prefab, spawnTarget.transform);
                    station1 = map[i, j].GetComponent<Station>();
                    station1.stationNumber = 0;
                }
                else if (reprez[i, j] == "station2" && map[i, j] == null)
                {
                    map[i, j] = Instantiate(station2Prefab, spawnTarget.transform);
                    station2 = map[i, j].GetComponent<Station>();
                    station2.stationNumber = 1;
                }
                else if (reprez[i, j] == "enemy1")
                    map[i, j] = Instantiate(enemy1, spawnTarget.transform);
                else if (reprez[i, j] == "enemy3")
                    map[i, j] = Instantiate(enemy2, spawnTarget.transform);
                else if (reprez[i, j] == "enemy2")
                    map[i, j] = Instantiate(enemy3, spawnTarget.transform);

                if (map[i, j] != null)
                    map[i, j].transform.position = new Vector3(0.25f + 0.5f * i - 5, 0.5f * j - 2f + (reprez[i, j]?[0] != 'w' ? -0.25f : 0));
            }
        }
    }

    public void UpdateEnemyStatus()
    {
        foreach (PlayerMove player in MecanicsManager.instance.players)
            player.Me.exp += 1;
        nrOfEnemies--;
        if(nrOfEnemies <= 0)
        {
            MecanicsManager.instance.musicManager.PlayMenuMusic();
            station1?.StartStation();
            station2?.StartStation();
        }
    }

    private void Start()
    {
        reprez[8, 0] = "station1";
        if (MecanicsManager.instance.players.Count > 1)
            reprez[11, 0] = "station2";
        randomizeLevel();
    }

    private void Update()
    {
        if (station1 != null && !allClear)
        {
            int cnt = 0;
            if (MecanicsManager.instance.players[0].stunned == true && MecanicsManager.instance.players[0].transform.position == station1.transform.position)
            {
                cnt++;
            }
            if (MecanicsManager.instance.players.Count > 1 && MecanicsManager.instance.players[1].stunned == true && MecanicsManager.instance.players[1].transform.position == station2.transform.position)
            {
                cnt++;
            }
            if (cnt == MecanicsManager.instance.players.Count)
            {
                reset();
                MecanicsManager.instance.dialogManager.FinishedRound();
                StartCoroutine(displayUpgrades());
            }
        }
    }

    IEnumerator displayUpgrades()
    {
        yield return new WaitForSeconds(0.2f);
        MecanicsManager.instance.upgradePanel.SetActive(true);
        for (int i = 0; i < MecanicsManager.instance.players.Count; i++)
        {
            MecanicsManager.instance.playerUpgrades[i].init();
        }
    }
}
