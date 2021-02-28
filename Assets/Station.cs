using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public Animator anim;
    public Animator Anim
    {
        get
        {
            if (anim == null)
                anim = GetComponent<Animator>();
            return anim;
        }
    }
    public int stationNumber;
    private PlayerMove player;
    public bool isStopped = true;
    private bool isDraggingPlayer = false;
    private float lerpVal = 0;
    
    private void Update()
    {
        if(!isStopped && !isDraggingPlayer && Vector2.Distance(MecanicsManager.instance.players[stationNumber].transform.position, transform.position) < 0.4f && MecanicsManager.instance.players[stationNumber].transform.position != transform.position)
        {
            MecanicsManager.instance.players[stationNumber].stunned = true;
            MecanicsManager.instance.players[stationNumber].SetHeight(1);

            isDraggingPlayer = true;
        }
        if(isDraggingPlayer)
        {
            lerpVal += Time.deltaTime;
            MecanicsManager.instance.players[stationNumber].transform.position = Vector2.Lerp(MecanicsManager.instance.players[stationNumber].transform.position, transform.position, lerpVal);
            if (lerpVal >= 1)
            {
                isDraggingPlayer = false;
                lerpVal = 0;
                MecanicsManager.instance.players[stationNumber].rigid.velocity = Vector2.zero;
                MecanicsManager.instance.players[stationNumber].transform.position = transform.position;
            }
        }
    }

    public void StartStation()
    {
        Anim.Play("StartStation");
        isStopped = false;
    }

    public void StopStation()
    {
        Anim.Play("StopStation");
        isStopped = true;
    }
}
