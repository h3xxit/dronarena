using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MoveScript
{
    public bool stunned = false;
    public GameObject indications1, indications2, indications3;
    private bool attacked, moved, pressedSlow, changedHeight;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!stunned)
        {
            Movement();
            Attack();
        }
        if(indications1.activeSelf && attacked && moved)
        {
            indications1.SetActive(false);
            Invoke("ActivateIndic2", 5);
        }
        if(indications2.activeSelf && pressedSlow)
        {
            indications2.SetActive(false);
        }
        if(!changedHeight && !indications3.activeSelf && me.maxHeight > 1)
        {
            indications3.SetActive(true);
        }
        if(indications3.activeSelf && changedHeight)
        {
            indications3.SetActive(false);
        }
    }

    void ActivateIndic2()
    {
        indications2.SetActive(true);
    }

    private void Attack()
    {
        attackCD = Mathf.Max(0, attackCD - Time.deltaTime);
        if (Input.GetAxis("FireGood" + (Me.playerNr + 1)) > 0.3f && attackCD == 0)
        {
            if (!attacked)
                attacked = true;
            StartCoroutine(Shoot(Me.maxBulletChain));
        }
    }

    private void Movement()
    {
        float horizontal;
        float vertical;

        if(Me.playerNr == 1)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            angle = Vector2.SignedAngle(Vector2.up, new Vector2(Input.GetAxis("HorizontalTarget"), Input.GetAxis("VerticalTarget")));
            rigid.AddForce(new Vector2(horizontal, vertical) * Me.speed * Time.deltaTime * (Input.GetAxis("Slow") > 0.3f ? 0.25f : 1) * (me.broken.isPlaying ? 0.5f : 1));
            if (!pressedSlow && Input.GetAxis("Slow") > 0.3f)
                pressedSlow = true;
        }
        else
        {
            horizontal = Input.GetAxis("Horizontal2");
            vertical = Input.GetAxis("Vertical2");
            angle = Vector2.SignedAngle(Vector2.up, MecanicsManager.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition) - body.transform.position);
            rigid.AddForce(new Vector2(horizontal, vertical) * Me.speed * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 0.25f : 1) * (me.broken.isPlaying ? 0.5f : 1));
            if (!pressedSlow && Input.GetKey(KeyCode.LeftShift))
                pressedSlow = true;
        }
        if (!moved && horizontal + vertical > 0.1f)
            moved = true;

        if (Me.playerNr == 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (Me.height > 1)
                {
                    SetHeight(Me.height - 1);
                    attackCD = 0.5f;
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (Me.height < Me.maxHeight)
                {
                    SetHeight(Me.height + 1);
                    attackCD = 0.5f;
                }
            }
        }
        else
        {
            if (Input.GetAxis("GoUp") > 0.3f)
            {
                if (Me.height > 1)
                {
                    SetHeight(Me.height - 1);
                    attackCD = 0.5f;
                }
            }
            else if (Input.GetAxis("GoDown") > 0.3f)
            {
                if (Me.height < Me.maxHeight)
                {
                    SetHeight(Me.height + 1);
                    attackCD = 0.5f;
                }
            }
        }
        elipticRotation();
    }

    public void SetHeight(int targetHeight)
    {
        if (!changedHeight && targetHeight != me.height)
            changedHeight = true;
        if (targetHeight > Me.height)
            Me.PlayClip(Me.upClip);
        else if (targetHeight < Me.height)
            Me.PlayClip(Me.downClip);
        Me.height = targetHeight;
        gameObject.layer = CHARACTER1_LAYER + Me.height - 1;
        anim.SetInteger("height", Me.height);
        heightIndicator.color = levelColor[Me.height];
    }
}
