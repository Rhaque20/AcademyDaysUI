using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Character : MonoBehaviour
{
    [SerializeField]Fighter p = null;
    [SerializeField]protected float speed, focus = 0, drainrate = 0.01f;
    [SerializeField]protected float quicken = 0, delay = 0, stop = 0;

    protected float _actionValue = 0f;

    public float actionValue
    {
        get{return _actionValue;}
    }
    [SerializeField]protected Sprite portrait1,portrait2;
    GameObject timeline;
    [SerializeField]Timeline tl;
    protected float maxCycle = 0, interval = 0f;
    public int index = 0;

    public string charName;
    [SerializeField]protected bool moving = false, usingActive = true;
    public bool isSpell = false;
    public bool isPlayer = true;

    public float CR
    {
        get{return focus;}
        set{
            focus = value;
            if (focus < 0f)
                focus = 0f;
        }
    }

    public float AGI
    {
        get{return speed;}
    }

    public bool isReady
    {
        get{return !moving;}
    }

    public void GrantBuff(int i)
    {
        switch(i)
        {
            case 0:
                quicken = 35;
                break;
            case 1:
                delay = 35;
                break;
            case 2:
                stop = 35;
                break;
        }
    }

    public Sprite activePortrait
    {
        get{
            if (usingActive)
                return portrait1;
            else
                return portrait2;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int cycles = 0;
        // if (p != null)
        // {
        //     speed = p.speed;
        //     if (p.useName)
        //         charName = new string(p.charName);
        //     portrait1 = p.portrait;
        //     isPlayer = p.isPlayer;
        // }
        // timeline = GameObject.Find("Timeline");
        // tl = timeline.GetComponent<Timeline>();
        // transform.localPosition = new Vector2(tl.returnPosition(focus*0.01f),transform.localPosition.y);
        // transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = portrait1;

        if (p != null)
        {
            // cycles = p.MinCyclesNeeded();
            // Debug.Log(p.charName+" has "+cycles+" cycles remaining.");
            // p.AddFocus(cycles);
            //Debug.Log(p.charName+"'s Focus: "+p.focus);
        }

    }

    public void StartRace(int value)
    {
        maxCycle = (float)value;
        moving = true;

    }

    public float calcFocus(int cycle)
    {
        return (cycle * speed/10f);
    }

    public int RemainingCycles()
    {
        if (focus >= 100f)
            return 0;
        
        float cycles = (100f - focus)/(speed/10f);
        int maxCycle = (int)Mathf.Ceil(cycles);
        int tempDelay = (int)delay, tempQuicken = (int)quicken;
        bool frozen = false;
        Debug.Log(charName+"'s maxCycle is "+maxCycle);

        if (tempQuicken != 0 || tempDelay != 0)
        {
            float tempfocus = this.focus;
            int cycle = 0;

            while (tempfocus < 100f)
            {
                if (tempQuicken != 0)
                {
                    if (tempDelay == 0)
                    {
                        tempfocus += (3*speed/20f);
                    }
                    else
                    {
                        tempfocus += (speed/10f);
                        tempDelay--;
                    }
                    tempQuicken--;
                }
                else if (tempDelay != 0)
                {
                    tempfocus += (speed/20f);
                    tempDelay--;
                }
                else
                {
                    tempfocus += (speed/10f);
                }
                cycle++;
            }

            maxCycle = cycle;
        }

        if (stop != 0)// If afflicted with stop
        {
            maxCycle += (int)stop;
        }

        return maxCycle;
    }

    public void CRChange(float CRshift)
    {
        focus += CRshift;
        if (focus < 0f)
            focus = 0f;
        transform.localPosition = new Vector2(tl.returnPosition(focus*0.01f),transform.localPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        
        // if (maxCycle > 0f)
        // {
        //     maxCycle -= drainrate;
        //     interval += drainrate;
            
        //     if (stop != 0f)
        //     {
        //         stop -= drainrate;
        //         if (stop <= 0f)
        //             stop = 0;
        //         return;
        //     }

        //     if (quicken != 0f)
        //     {
        //         if (delay == 0f)
        //         {
        //             focus += (speed *0.1f) * drainrate * 1.5f;
        //         }
        //         else
        //         {
        //             focus += (speed *0.1f) * drainrate;
        //             delay -= drainrate;
        //         }
        //         quicken -= drainrate;
        //     }
        //     else if (delay != 0f)
        //     {
        //         focus += (speed *0.1f) * 0.01f * 0.5f;
        //         delay -= drainrate;
        //         if (delay < 0f)
        //             delay = 0f;
        //     }
        //     else
        //     {
        //         focus += (speed *0.1f) * 0.01f;
        //     }

        //     if (maxCycle <= 0f)
        //     {
        //         maxCycle = 0f;
        //         moving = false;
        //         tl.CheckIfDone();
        //     }
        //     transform.localPosition = new Vector2(tl.returnPosition(focus*0.01f),transform.localPosition.y);
        // }
        
    }

}
