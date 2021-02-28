using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public AudioClip dieClip, upClip, downClip, hurtClip, shieldUpClip, shieldDownClip, brokenClip;
    private AudioSource audioSource;
    public ParticleSystem broken;
    public SpriteRenderer shieldSprite;
    public float maxHp, maxShield;
    public float rotSpd, speed;
    public float bulletSpeed, bulletDamage;
    public float fullShieldRechargeTime, shieldRechargeSpeed;
    public int nrOfBounces;
    public float maxAttackCD = 0.2f;
    public int maxBulletChain = 1;
    [HideInInspector]
    public int exp, maxHeight, playerNr;
    public SpriteRenderer hpHolder, hpVal;
    private float maxWidth = 0.3125f, lastHitTime;

    private bool isPlayer;

    [HideInInspector]
    public int height = 1;
    [HideInInspector]
    public float hp, shield;
    private float currShieldTimer = 0;
    [HideInInspector]
    public bool dead = false;
    
    public void init()
    {
        if (isPlayer)
        {
            audioSource.loop = false;
            audioSource.Stop();
            broken.Stop();
        }
        else
        {
            maxHp += MecanicsManager.instance.mapManager.difficulty;
            bulletDamage += MecanicsManager.instance.mapManager.difficulty / 2;
            speed += MecanicsManager.instance.mapManager.difficulty * 6;
        }
        hp = maxHp;
        shield = maxShield;
        currShieldTimer = 0;
        if (shield == 0)
            shieldSprite.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (dead)
            return;
        if(shield > 0 && shield < maxShield)
        {
            shield = Mathf.Min(shield + Time.deltaTime * shieldRechargeSpeed, maxShield);
        }
        else if(shield == 0)
        {
            currShieldTimer += Time.deltaTime;
            if(currShieldTimer >= fullShieldRechargeTime)
            {
                currShieldTimer = 0;
                shield = maxShield;
                PlayClip(shieldUpClip);
                shieldSprite.gameObject.SetActive(true);
            }
        }
        if(isPlayer || Time.time - lastHitTime < 5)
            hpHolder.gameObject.SetActive(true);
        else
            hpHolder.gameObject.SetActive(false);
        if (hpHolder.gameObject.activeSelf)
            hpVal.size = new Vector2(hp / maxHp * maxWidth, hpVal.size.y);
        audioSource.volume = Preload.instance.masterVolume * Preload.instance.sfxVolume;
    }

    public void TakeDamage(float damage)
    {
        if (dead)
            return;
        lastHitTime = Time.time;
        if (shield > 0)
        {
            shield -= damage;
            if (shield <= 0)
            {
                PlayClip(shieldDownClip);
                shieldSprite.gameObject.SetActive(false);
            }
            if (shield < 0)
            {
                PlayClip(shieldDownClip);
                PlayClip(hurtClip);
                hp += shield;
                shield = 0;
            }
        }
        else
        {
            PlayClip(hurtClip);
            hp -= damage;
        }
        if (hp <= 0)
        {
            hp = 0;
            die();
        }
    }

    public void die()
    {
        if (!isPlayer)
        {
            hpVal.size = new Vector2(0, hpVal.size.y);
            dead = true;
            GetComponent<Animator>().Play("Die");
            PlayClip(dieClip);
            if (MecanicsManager.instance.mapManager.nrOfEnemies == 1)
            {
                MecanicsManager.instance.StartCoroutine(MecanicsManager.instance.dieSlow(1f));
            }
            else
                MecanicsManager.instance.StartCoroutine(MecanicsManager.instance.dieSlow(0.4f));
        }
        else
        {
            audioSource.loop = true;
            PlayClip(brokenClip);
            broken.Play();
            MecanicsManager.instance.musicManager.GameOver();
            MecanicsManager.instance.mapManager.killAllEnemies();
        }
    }

    public void onDieFinish()
    {
        
    }

    public void JustDie()
    {
        MecanicsManager.instance.mapManager.UpdateEnemyStatus();
        Destroy(gameObject);
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null || (isPlayer && broken.isPlaying))
            return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void Start()
    {
        maxHeight = 1;
        init();
        audioSource = GetComponent<AudioSource>();
        isPlayer = GetComponent<PlayerMove>() != null;
    }
}