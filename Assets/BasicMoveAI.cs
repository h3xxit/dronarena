using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMoveAI : MoveScript
{
    [HideInInspector]
    public PlayerMove target;
    private RaycastHit2D[] raycastHit2D = new RaycastHit2D[5];
    private List<LayerMask> layersToHit = new List<LayerMask> { (1 << 8 | 1 << 11 | 1 << 14 | 1 << 9 | 1 << 12 | 1 << 15 | 1 << 10 | 1 << 12 | 1 << 16), (1 << 9 | 1 << 12 | 1 << 15 | 1 << 10 | 1 << 12 | 1 << 16), (1 << 10 | 1 << 12 | 1 << 16) };
    private ContactFilter2D contactFilter;
    private float minDist;
    private Vector2 finalDir, lastDir;
    private int sign;

    protected override void OnStart()
    {
        InvokeRepeating("changeSign", 0, 4);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        GetNewTarget();
        angle = Vector2.SignedAngle(Vector2.up, target.transform.position - transform.position);
        Movement();
        Attack();
    }

    private void changeSign()
    {
        sign = Random.Range(0, 2) == 1 ? 1 : -1;
    }

    const float DESTINATION_WEIGHT = 1f;
    const float LAST_DIR_WEIGHT = 1f;
    const float RAYCAST_DIST = 0.8f;
    const float OBSTACLE_WEIGHT = 1f;

    private void calculateFinalDir()
    {
        Vector2 v = (Vector2)target.transform.position - rigid.position;
        v = v.normalized;

        finalDir = lastDir * LAST_DIR_WEIGHT;
        finalDir += v * DESTINATION_WEIGHT * Time.deltaTime;
        Vector2 rayDir = Vector2.right;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(layersToHit[Me.height - 1]);
        for (int i = 0; i < 12; i++)
        {
            int cnt = collider.Raycast(rayDir, contactFilter, raycastHit2D, RAYCAST_DIST);
            if (cnt > 0)
            {
                finalDir += -rayDir * Mathf.Min((RAYCAST_DIST - raycastHit2D[0].distance) * OBSTACLE_WEIGHT, 1) * Time.deltaTime;
                finalDir += sign * (Vector2)(Quaternion.Euler(0, 0, 90) * rayDir) * Mathf.Min((RAYCAST_DIST - raycastHit2D[0].distance) * OBSTACLE_WEIGHT, 1) * Time.deltaTime;
            }
            rayDir = Quaternion.AngleAxis(30, Vector3.forward) * rayDir;
        }
        finalDir.Normalize();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, finalDir);
    }

    private void Movement()
    {
        calculateFinalDir();
        
        rigid.AddForce(finalDir * Me.speed * Time.deltaTime);

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
        elipticRotation();
    }

    private void GetNewTarget()
    {
        minDist = 1000000;
        float dist;
        foreach (PlayerMove player in MecanicsManager.instance.players)
        {
            dist = Vector2.Distance(player.transform.position, transform.position);
            if (dist < minDist)
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
