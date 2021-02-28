using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class wallLogic : MonoBehaviour
{
    public Animator anim;
    public Animator Anim
    {
        get
        {
            if(anim == null)
                anim = GetComponent<Animator>();
            return anim;
        }
    }
    public List<Light2D> lights;
    public List<Color> colors;
    
    private int nrOfCollisions;
    private float timer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        timer = 1;
        if (colors.Count > nrOfCollisions + 1)
        {
            nrOfCollisions++;
            setLightsColor(colors[nrOfCollisions]);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                nrOfCollisions = 0;
                setLightsColor(colors[0]);
            }
        }
    }

    void setLightsColor(Color color)
    {
        foreach (Light2D light in lights)
            light.color = color;
    }

    public void OnAnimFinish()
    {
        Destroy(gameObject);
    }
}
