﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAI : MoveScript
{
    [HideInInspector]
    public PlayerMove target;
    private RaycastHit2D[] raycastHit2D = new RaycastHit2D[5];
    private List<LayerMask> layersToHit = new List<LayerMask> { (1<<8 | 1<<11 | 1<<14 | 1 << 9 | 1 << 12 | 1 << 15 | 1 << 10 | 1 << 12 | 1 << 16), (1 << 9 | 1 << 12 | 1 << 15 | 1 << 10 | 1 << 12 | 1 << 16), (1 << 10 | 1 << 12 | 1 << 16)};
    private ContactFilter2D contactFilter;
    private float minDist;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        GetNewTarget();
        angle = Vector2.SignedAngle(Vector2.up, target.transform.position - transform.position);
        Movement();
        Attack();
    }

    private void Movement()
    {
        if (Random.Range(0, 60) == 0)
        {
            if (target.Me.height < Me.height)
            {
                if (Me.height > 1)
                {
                    Me.height--;
                    gameObject.layer--;
                    anim.SetInteger("height", Me.height);
                    heightIndicator.color = levelColor[Me.height];
                    attackCD = 0.5f;
                }
            }
            else if (target.Me.height > Me.height)
            {
                if (Me.height < 3)
                {
                    Me.height++;
                    gameObject.layer++;
                    anim.SetInteger("height", Me.height);
                    heightIndicator.color = levelColor[Me.height];
                    attackCD = 0.5f;
                }
            }
        }
    }

    private void GetNewTarget()
    {
        minDist = 1000000;
        float dist;
        foreach(PlayerMove player in MecanicsManager.instance.players)
        {
            dist = Vector2.Distance(player.transform.position, transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                target = player;
            }
        }
    }

    private void Attack()
    {
        attackCD = Mathf.Max(0, attackCD - Time.deltaTime);
        contactFilter.SetLayerMask(layersToHit[Me.height - 1]);
        collider.Raycast(target.transform.position - transform.position, contactFilter, raycastHit2D, minDist);
        if (attackCD == 0 && raycastHit2D[0].transform == target.transform)
        {
            StartCoroutine(Shoot(Me.maxBulletChain));
        }
    }
}
