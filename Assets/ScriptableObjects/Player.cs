using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "New Player", menuName = "Player")]
public class Player : ScriptableObject
{
    public string charName;
    public Sprite portrait;
    public EnumLibrary.Element attribute;
    public EnumLibrary.Race species1,species2;
    public enum UnitType{Player,Enemy,SpellUnit,Reinforcement};
    public bool useName = true;
    public UnitType unitType;

    public int level = 1;

    //0 = HP,1 = PATK,2 = PDEF,3 = MATK,4 = MDEF, 5 = DEX,6 = CRIT
    [NamedArrayAttribute (new string[] {"HP","PATK","PDEF","MATK","MDEF","TEC","AGI","CRIT","LUCK","MOV","JUMP"})]
    public float[] stats = new float[11];

    [NamedArrayAttribute (new string[] {"HP","PATK","PDEF","MATK","MDEF","TEC","AGI","CRIT","LUCK","MOV","JUMP"})]
    public float[] statBoosts = new float[11];

    [NamedArrayAttribute (new string[] {"HIT UP","EVA UP","CRIT UP","Cast Rate Red","Guts Chance","HP Cost Down"})]
    public float[] otherStats = new float[6];

    public List<Skill> skillsList = new List<Skill>();
    
    [SerializeField]private int quicken = 0, delay = 0, stop = 0, _cyclesNeeded = 0;
    private int quickenDelayDiff = 0;
    [NamedArrayAttribute (new string[] {"Fire","Wind","Earth","Water","Light","Dark"})]
    public float[] elementalBoost = new float [6];
    [NamedArrayAttribute (new string[] {"Fire","Wind","Earth","Water","Light","Dark"})]
    public float[] elementalRes = new float [6];
    [NamedArrayAttribute (new string[] {"Slash","Impact","Pierce","Missle","Magic","NonType"})]
    public float[] damagePowerBoost = new float [5];
    [NamedArrayAttribute (new string[] {"Slash","Impact","Pierce","Missle","Magic","NonType"})]
    public float[] damagePowerRes = new float [5];

    private float _actionValue = 999f;

    private float _focus = 0f;

    public float actionValue
    {
        get{return _actionValue;}
    }

    public int cyclesNeeded
    {
        get{return _cyclesNeeded;}
    }

    public float speed
    {
        get{return MathF.Max(GetFinalStat((int)EnumLibrary.Stats.AGI),50);}
        set{
            Debug.Log("Setting speed");
        }
    }
    public Dictionary<string,StatusEffect> statuses = new Dictionary<string,StatusEffect>();

    public int elementInt
    {
        get{return (int)attribute;}
    }

    public float CRGain
    {
        get{return speed/10f;}
    }

    public float focus
    {
        get{return _focus;}
    }

    // public Player()
    // {
    //     statuses = new Dictionary<string,StatusEffect>();
    // }

    public void DupePlayer(Player mainPlayer)
    {
        Debug.Log("Calling dupe!");
        charName = mainPlayer.charName;
        //speed = mainPlayer.speed;
        portrait = mainPlayer.portrait;
        stats = new float[mainPlayer.stats.Length];
        otherStats = new float[mainPlayer.otherStats.Length];
        Array.Copy(mainPlayer.stats,stats,mainPlayer.stats.Length);
        Array.Copy(mainPlayer.otherStats,otherStats,mainPlayer.otherStats.Length);
        attribute = mainPlayer.attribute;
        species1 = mainPlayer.species1;
        species2 = mainPlayer.species2;
        unitType = mainPlayer.unitType;
        skillsList = new List<Skill>(mainPlayer.skillsList);
        unitType = mainPlayer.unitType;
        // Add onto dupe function
    }

    public void CreateSpellUnit(string spellName, float spellSpeed)
    {
        charName = spellName;
        stats[(int)EnumLibrary.Stats.AGI] = spellSpeed;
        unitType = UnitType.SpellUnit;
        GenerateActionValue();
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

    public float CycleswithQuicken(float curFocus)
    {
        return Mathf.Ceil((100 - curFocus)/(CRGain * 1.5f));
    }

    public float CycleswithDelay(float curFocus)
    {
        return Mathf.Ceil((100 - curFocus)/(CRGain * 0.5f));
    }

    public void GenerateActionValue()
    {
        _focus = 0f;
        _actionValue = 10000/speed;
        _actionValue = Mathf.Round(_actionValue *100f)/100.0f;
    }

    public void FocusPush(float CT)
    {
        _focus += CT;
        if (_focus < 0f)
            _focus = 0;
    }

    public void ResetFocus()
    {
        _focus = 0;
    }

    public void AlterActionValue(float push)
    {
        _actionValue *= (1 - push/100f);
    }

    public void AlterActionValue()
    {

    }

    public void DecreaseActionValue(float decrease)
    {
        _actionValue -= decrease;
        if (_actionValue < 1f)
        {
            _actionValue = 1;
        }
    }

    

    public bool AddFocus(int cycles)
    {
        // if (cycles < stop)
        // {
        //     stop -= cycles;
        //     return false;
        // }

        // int cyclesRemaining = cycles - stop;
        // stop = 0;
        
        // if (quickenDelayDiff > 0)
        // {
        //     if (delay >= cyclesRemaining)
        //     {
        //         delay -= cyclesRemaining;
        //         quicken = (int)Mathf.Max(0,quicken - cyclesRemaining);
        //         _focus += (CRGain * cyclesRemaining);
        //     }
        //     else
        //     {
        //         cyclesRemaining -= delay;
        //         _focus += (CRGain * delay);
        //         delay = 0;

        //         if(quicken >= cyclesRemaining)
        //         {
        //             _focus += (CRGain * cyclesRemaining * 1.5f);
        //             quicken -= cyclesRemaining;
        //             cyclesRemaining = 0;
        //         }
        //         else
        //         {
        //             _focus += (CRGain * quicken * 1.5f) + (CRGain * (cyclesRemaining - quicken));
        //             // cyclesRemaining = (int) Mathf.Ceil((100 - focus)/CRGain);
        //             // Debug.Log("Cycles remaining after applying quicken: "+cyclesRemaining);
        //             // focus += (CRGain * cyclesRemaining);
        //             quicken = 0;
        //         }
        //     }
        // }
        // else if (quickenDelayDiff < 0)
        // {
        //     if (quicken >= cyclesRemaining)
        //     {
        //         quicken -= cyclesRemaining;
        //         delay = (int)Mathf.Max(0,delay - cyclesRemaining);
        //         _focus += (CRGain * cyclesRemaining);
        //     }
        //     else
        //     {
        //         cyclesRemaining -= quicken;
        //         _focus += (CRGain * quicken);
        //         quicken = 0;

        //         if(delay >= cyclesRemaining)
        //         {
        //             _focus += (CRGain * cyclesRemaining * 0.5f);
        //             delay -= cyclesRemaining;
        //             cyclesRemaining = 0;
        //         }
        //         else
        //         {
        //             _focus += (CRGain * delay * 0.5f) + (CRGain * (cyclesRemaining - delay));
        //             quicken = 0;
        //         }
        //     }
        // }
        // else
        // {
        //     _focus += (CRGain*cycles);
        // }

        _focus += (CRGain*cycles);

        return (_focus >= 100f);
    }

    public int MinCyclesNeeded()
    {
        float focusRemaining = (100f - _focus);
        float cyclesRemaining = Mathf.Ceil(focusRemaining/CRGain);
        float tempFocus = _focus;

        int tempQuick = quicken, tempDelay = delay;

        // if (focusRemaining <= 0)
        // {
        //     _cyclesNeeded = 0;
        //     return 0;
        // }
            


        // quickenDelayDiff = quicken - delay;

        // if (quickenDelayDiff > 0)// More quicken than delay
        // {
        //     if (delay >= cyclesRemaining)// If the duration for delay >= standard cycles remaining return standard cycles remaining
        //     {
        //         _cyclesNeeded = (int)cyclesRemaining + stop;
        //         return _cyclesNeeded;
        //     }    

        //     tempFocus += (delay * CRGain); // Gain _focus from number of delay cycles
        //     tempQuick -= delay;// Decrement quick duration by number of delay cycles passed

        //     int quickenCycles = (int)CycleswithQuicken(tempFocus);// Get how many quicken cycles will be needed to get a turn
        //     Debug.Log("Quicken Cycles are "+quickenCycles);

        //     if (quickenCycles > tempQuick)// If quicken cycles exceeds the quicken's duration
        //     {
        //         tempFocus += (1.5f * CRGain * tempQuick);// Gain _focus based on remaining quicken duration
        //         _cyclesNeeded = (int)Mathf.Ceil((100 - tempFocus)/CRGain) + stop + quicken;
        //         return _cyclesNeeded;// Return remaining standard cycles to get a turn
        //     }
        //     else
        //     {
        //         _cyclesNeeded = (int)quickenCycles + stop + quicken;
        //         return _cyclesNeeded;
        //     }
        //         // Return duration of quicken in the event the
        //         // number of quicken cycles is less than or equal to the duration of quicken
            

        // }
        // else if (quickenDelayDiff < 0) // More delay than quicken
        // {
        //     if(quicken >= cyclesRemaining)// If the duration for quicken >= standard cycles remaining
        //     {
        //         _cyclesNeeded = (int)cyclesRemaining + stop;
        //         return _cyclesNeeded;
        //     }
                
            
        //     tempFocus += quicken * CRGain;
        //     tempDelay -= quicken;

        //     int delayCycles = (int)CycleswithDelay(tempFocus);

        //     if(delayCycles > tempDelay)
        //     {
        //         tempFocus += (0.5f * CRGain * tempDelay);
        //         _cyclesNeeded = (int)Mathf.Ceil((100 - tempFocus)/CRGain) + stop + delay;
        //         return _cyclesNeeded;// Return remaining standard cycles to get a turn
        //     }
        //     else
        //     {
        //         _cyclesNeeded = delayCycles + stop;
        //         return _cyclesNeeded;
        //     }
                

        // }
        _cyclesNeeded = (int)cyclesRemaining;
        Debug.Log("Cycles needed for "+charName+" is "+_cyclesNeeded);
        return _cyclesNeeded;
    }

    public float GetStat(int i)
    {
        return stats[i];
    }


    public float GetFinalElemental(int i,bool isRes)
    {
        string statusName = ((EnumLibrary.Element)i).ToString();
        float additionalMod = 0f;

        if (isRes)
        {
            statusName += " Res ";
            if (statuses.ContainsKey(statusName +"Up"))
            {
                additionalMod += statuses[statusName +"Up"].efficacy;
            }

            if (statuses.ContainsKey(statusName +"Down"))
            {
                additionalMod += statuses[statusName +"Down"].efficacy;
            }

            return elementalRes[i] + additionalMod;
        }
        else
        {
            statusName += " Power ";
            if (statuses.ContainsKey(statusName +"Up"))
            {
                Debug.Log(i+": Contains elemental "+statusName);
                additionalMod += statuses[statusName +"Up"].efficacy;
            }

            if (statuses.ContainsKey(statusName +"Down"))
            {
                additionalMod += statuses[statusName +"Down"].efficacy;
            }

            return elementalBoost[i] + additionalMod;
        }

        return 0f;
    }

    public float GetFinalStat(int i)
    {
        string statusName = ((EnumLibrary.Stats)i).ToString();
        float additionalMod = 0f;

        string boostStatus = statusName+" Up";
        if(statuses.ContainsKey(boostStatus))
        {
            Debug.Log("Contains "+boostStatus);
            additionalMod += statuses[boostStatus].efficacy;
        }
        
        string dropStatus = statusName + " Down";
        if(statuses.ContainsKey(dropStatus))
        {
            additionalMod -= statuses[dropStatus].efficacy;
        }

        return stats[i] * (1 + additionalMod);
    }


    public float GetFinalPower(int i,bool isRes)
    {
        string statusName = ((EnumLibrary.DamagePower)i).ToString();
        float additionalMod = 0f;

        if (isRes)
        {
            statusName += " Res ";
            if (statuses.ContainsKey(statusName +"Up"))
            {
                additionalMod += statuses[statusName +"Up"].efficacy;
            }

            if (statuses.ContainsKey(statusName +"Down"))
            {
                additionalMod += statuses[statusName +"Down"].efficacy;
            }

            return damagePowerRes[i] + additionalMod;
        }
        else
        {
            statusName += " Power ";
            if (statuses.ContainsKey(statusName +"Up"))
            {
                Debug.Log(i+": Contains "+statusName+" of efficacy "+statuses[statusName +"Up"].efficacy);
                additionalMod += statuses[statusName +"Up"].efficacy;
            }

            if (statuses.ContainsKey(statusName +"Down"))
            {
                additionalMod += statuses[statusName +"Down"].efficacy;
            }

            return damagePowerBoost[i] + additionalMod;
        }

        return 0f;
    }
}
