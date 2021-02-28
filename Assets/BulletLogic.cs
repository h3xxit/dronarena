using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    private const float MIN_PITCH = 0.9f, MAX_PITCH = 1;

    public AudioClip laserClip, bounceClip;
    private AudioSource audioSource;
    public ParticleSystem sparks;
    [HideInInspector]
    public int nrOfBounces;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float speed;

    private int currentCollisions;
    private SpriteRenderer sprite;
    private Collider2D col;
    private SpriteRenderer spriteR;
    private Rigidbody2D rigid;
    public Rigidbody2D Rigid
    {
        get
        {
            if (rigid == null)
                rigid = GetComponent<Rigidbody2D>();
            return rigid;
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioSource.volume = Preload.instance.masterVolume * Preload.instance.sfxVolume;
    }

    public void init(int nrOfBounces, float speed, float damage, int layer, Quaternion rotation, Vector3 height, Color color)
    {
        this.nrOfBounces = nrOfBounces;
        this.speed = speed;
        this.damage = damage;

        if(col == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
            col = GetComponentInChildren<Collider2D>();
        }

        col.gameObject.layer = layer;
        col.transform.rotation = rotation;
        sprite.transform.rotation = rotation;
        sprite.transform.localPosition = height;
        sprite.color = color;
        gameObject.SetActive(true);
        currentCollisions = 0;
        Rigid.velocity = rotation * transform.up * speed;

        PlayClip(laserClip);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        sparks.Play();
        PlayClip(bounceClip);
        MecanicsManager.instance.StartCoroutine(MecanicsManager.instance.bulletShake());
        if (other.gameObject.CompareTag("Wall"))
        {
            currentCollisions++;
            if(currentCollisions > nrOfBounces)
            {
                Despawn();
                return;
            }
            Vector2 wallNormal = other.contacts[0].normal;
            Vector2 reflection = Vector2.Reflect(Rigid.velocity, wallNormal);
            col.transform.rotation = Quaternion.FromToRotation(Rigid.velocity, reflection) * col.transform.rotation;
            sprite.transform.rotation = Quaternion.FromToRotation(Rigid.velocity, reflection) * sprite.transform.rotation;
            Rigid.velocity = reflection;
        }
        else if(other.gameObject.CompareTag("Character"))
        {
            other.gameObject.GetComponent<Stats>().TakeDamage(damage);
            other.rigidbody.velocity = Vector2.zero;
            Despawn();
            return;
        }
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;
        audioSource.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void Despawn()
    {
        sprite.enabled = false;
        col.enabled = false;
        Invoke("Disappear", 0.2f);
    }

    private void Disappear()
    {
        col.enabled = true;
        gameObject.SetActive(false);
        sprite.enabled = true;
        MecanicsManager.instance.bulletPool.pool.Enqueue(this);
    }
}
