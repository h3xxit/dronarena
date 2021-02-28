using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MoveScript : MonoBehaviour
{
    protected const int BULLET1_LAYER = 11;
    protected const int CHARACTER1_LAYER = 8;

    public Rigidbody2D rigid;
    protected Collider2D collider;
    protected Animator anim;
    protected float angle;
    [HideInInspector]
    public Stats me;
    public Stats Me
    {
        get
        {
            if (me == null)
                me = GetComponent<Stats>();
            return me;
        }
    }
    public List<GameObject> rotors;
    public GameObject gun1, gun2, body;
    public List<Color> levelColor;
    public SpriteRenderer heightIndicator;
    public List<SpriteRenderer> materials;
    public Light2D light2d;
    private float fadeVal = 0;

    [HideInInspector]
    public float attackCD = 0;

    public float distFromCenter = 0.15f, sinDiff = 0.3f;

    protected void elipticRotation()
    {
        gun1.transform.position = transform.position + new Vector3(-Mathf.Sin(Mathf.Deg2Rad * angle) * distFromCenter, Mathf.Cos(Mathf.Deg2Rad * angle) * distFromCenter * sinDiff);
        gun2.transform.position = transform.position + new Vector3(-Mathf.Sin(Mathf.Deg2Rad * angle) * distFromCenter * 1.2f, Mathf.Cos(Mathf.Deg2Rad * angle) * distFromCenter * sinDiff * 1.2f);

        float a2 = angle + 360 / rotors.Count / 2;
        foreach (GameObject g in rotors)
        {
            g.transform.position = transform.position + new Vector3(-Mathf.Sin(Mathf.Deg2Rad * a2) * distFromCenter * 1.8f, Mathf.Cos(Mathf.Deg2Rad * a2) * distFromCenter * sinDiff * 1.8f);
            a2 += 360 / rotors.Count;
        }
    }

    private void Update()
    {
        if (Me.dead)
        {
            fadeVal -= Time.deltaTime;
            if (fadeVal < 0)
            {
                Me.JustDie();
            }
            foreach (SpriteRenderer mat in materials)
            {
                mat.material.SetFloat("_Fade", fadeVal);
            }
            if (light2d != null)
                light2d.intensity = fadeVal;
            return;
        }
        if (fadeVal < 1)
        {
            fadeVal += Time.deltaTime;
            if (fadeVal > 1)
            {
                fadeVal = 1;
            }
            foreach (SpriteRenderer mat in materials)
            {
                mat.material.SetFloat("_Fade", fadeVal);
            }
            if(light2d != null)
                light2d.intensity = fadeVal;
        }
        OnUpdate();
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        attackCD = 2;
        OnStart();
    }

    public IEnumerator Shoot(int times)
    {
        for (int i = 0; i < times; i++)
        {
            if (Me.playerNr == 1)
            {
                if (Input.GetAxis("Slow") > 0.3f)
                    attackCD = 0.2f;
                else
                    attackCD = Me.maxAttackCD;
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    attackCD = 0.2f;
                else
                    attackCD = Me.maxAttackCD;
            }
            BulletLogic bullet = MecanicsManager.instance.bulletPool.GetNewBullet();
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);
            bullet.transform.position = transform.position + (bulletRotation * Vector2.up) * 0.35f;
            bullet.init(Me.nrOfBounces, Me.bulletSpeed, Me.bulletDamage, BULLET1_LAYER - 1 + Me.height, bulletRotation, body.transform.localPosition, levelColor[Me.height]);
            yield return new WaitForSeconds(0.17f);
        }
    }

    protected virtual void OnStart()
    {

    }

    protected virtual void OnUpdate()
    {

    }
}
