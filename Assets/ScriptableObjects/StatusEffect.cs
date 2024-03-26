using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Status Effect", menuName = "Status Effect")]
public class StatusEffect : ScriptableObject
{
    public string statusName;
    public int duration = 3, useCount = 0;

    public bool isBuff = true;
    public bool onUse = false;
    public bool isElement = true;
    public bool isStatUp = false;
    public float efficacy = 0f;
    public bool isRes = false;

    public Sprite icon;
    public EnumLibrary.Element attribute;
    public EnumLibrary.DamagePower power;
    public EnumLibrary.Stats stat;

    StatusEffect(string statusName, bool isBuff, bool onHit,EnumLibrary.Element attribute,bool isElement)
    {
        this.statusName = statusName;
        this.isBuff = isBuff;
        onUse = onHit;
        this.isElement = isElement;
        this.attribute = attribute;
    }

    StatusEffect(string statusName, bool isBuff, bool onHit,EnumLibrary.DamagePower power,bool isElement)
    {
        this.statusName = statusName;
        this.isBuff = isBuff;
        onUse = onHit;
        this.isElement = isElement;
        this.power = power;
    }

    public StatusEffect(string statusName, StatusInput input, int duration)
    {
        this.statusName = statusName;
        isBuff = input.storedBuff;
        onUse = input.onlyOnUse;
        this.duration = duration;
        isElement = input.isElement;
        if (isElement)
        {
            attribute = input.attribute;
        }
        else if(!input.isStatUp)
        {
            power = input.power;
        }
        else
        {
            stat = input.stats;
            isStatUp = true;
        }

        int tempEfficacy = int.Parse(input.inputField.text);

        if (tempEfficacy < 0)
            tempEfficacy *= -1;
        
        if (isBuff)
        {
            efficacy = (float)(tempEfficacy/100f);
        }
        else
        {
            efficacy = (float)(tempEfficacy/-100f);
        }

        isRes = this.statusName.Contains("Res");

    }

}
