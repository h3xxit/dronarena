using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    private AudioSource audioSource;
    public List<TMPro.TextMeshProUGUI> upgradeName, upgradeDescription;
    public TMPro.TextMeshProUGUI exp;
    public List<Button> upgradeBtn;
    public Button ready;
    public List<Image> upgradeImg;
    public int playerNr;
    private List<Upgrade> upgrades;
    public List<Sprite> upgradeSprites;
    private List<Upgrade> selectedUpgrades = new List<Upgrade>();

    private void initUpgrades()
    {
        upgrades = new List<Upgrade>();
        audioSource = GetComponent<AudioSource>();
        upgrades.Add(new Upgrade((p) => { p.Me.maxHeight++; },
            (p) => p.Me.maxHeight < 3,
            2,
            "Increase Height",
            "Fly over towers. Press Q/RB to descend and E/LB to ascend",
            upgradeSprites[0]));
        upgrades.Add(new Upgrade((p) => { p.Me.bulletSpeed += 1; },
            (p) => p.Me.bulletSpeed < 12,
            3,
            "Projectile Speed",
            "Projectiles fly faster",
            upgradeSprites[1]));
        upgrades.Add(new Upgrade((p) => { p.Me.bulletDamage++; },
            (p) => true,
            3,
            "Projectile Power",
            "Projectiles deal more damage",
            upgradeSprites[2]));
        upgrades.Add(new Upgrade((p) => { p.Me.nrOfBounces++; },
            (p) => true,
            1,
            "Projectile Ricochet",
            "Projectiles bounce more times",
            upgradeSprites[3]));
        upgrades.Add(new Upgrade((p) => { p.Me.maxHp+=5; },
            (p) => true,
            1,
            "Stronger Armor",
            "Increase your drone's resistance to projectiles",
            upgradeSprites[4]));
        upgrades.Add(new Upgrade((p) => { p.Me.speed += 50; },
            (p) => p.Me.speed < 700,
            2,
            "Improved propellers",
            "Increase your drone's speed",
            upgradeSprites[5]));
    }

    public void init()
    {
        if (upgrades == null)
            initUpgrades();
        ready.interactable = true;
        selectedUpgrades.Clear();
        exp.text = "Points " + MecanicsManager.instance.players[playerNr].Me.exp;
        List<Upgrade> possibleUpgrades = upgrades.FindAll((up) => up.CanAppear(MecanicsManager.instance.players[playerNr]));
        for (int i = 0; i< possibleUpgrades.Count; i++)
        {
            if(Random.Range(0, possibleUpgrades.Count - i) < 3 - selectedUpgrades.Count)
            {
                selectedUpgrades.Add(possibleUpgrades[i]);
            }
        }
        for(int i = 0; i<3; i++)
        {
            if(selectedUpgrades[i].cost <= MecanicsManager.instance.players[playerNr].Me.exp)
                upgradeBtn[i].interactable = true;
            else
                upgradeBtn[i].interactable = false;
            upgradeImg[i].sprite = selectedUpgrades[i].img;
            upgradeName[i].text = selectedUpgrades[i].name;
            upgradeDescription[i].text = selectedUpgrades[i].description + "\nCost: " + selectedUpgrades[i].cost;
        }
        gameObject.SetActive(true);
    }

    public void OnClickUpgrade(int nr)
    {
        audioSource.volume = Preload.instance.masterVolume * Preload.instance.sfxVolume;
        audioSource.Play();
        if (nr != -1)
        {
            selectedUpgrades[nr].OnUpgrade(MecanicsManager.instance.players[playerNr]);
            MecanicsManager.instance.players[playerNr].Me.exp -= selectedUpgrades[nr].cost;
        }
        upgradeBtn[0].interactable = false;
        upgradeBtn[1].interactable = false;
        upgradeBtn[2].interactable = false;
        ready.interactable = false;
    }

    private void Update()
    {
        if (playerNr == 1)
        {
            if (Input.GetAxis("option1") > 0.3f && upgradeBtn[0].interactable)
            {
                OnClickUpgrade(0);
            }
            else if (Input.GetAxis("option2") > 0.3f && upgradeBtn[1].interactable)
            {
                OnClickUpgrade(1);
            }
            else if (Input.GetAxis("option3") > 0.3f && upgradeBtn[2].interactable)
            {
                OnClickUpgrade(2);
            }
            else if (Input.GetAxis("option4") > 0.3f && ready.interactable)
            {
                OnClickUpgrade(-1);
            }
        }
    }
}


public class Upgrade
{
    public System.Action<PlayerMove> OnUpgrade;
    public System.Predicate<PlayerMove> CanAppear;
    public int cost;
    public string name, description;
    public Sprite img;

    public Upgrade(System.Action<PlayerMove> OnUpgrade, System.Predicate<PlayerMove> CanAppear, int cost, string name, string description, Sprite img)
    {
        this.OnUpgrade = OnUpgrade;
        this.CanAppear = CanAppear;
        this.cost = cost;
        this.name = name;
        this.description = description;
        this.img = img;
    }
}