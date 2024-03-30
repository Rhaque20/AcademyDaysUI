using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DamageModal : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField]Transform _targetList;

    const int fireAttr = 0, windAttr = 1, earthAttr = 2, waterAttr = 3, lightAttr = 4, darkAttr = 5;

    [SerializeField]GameObject _damageLogPrefab;
    [SerializeField]Transform _statusBlock;
    [SerializeField]Transform _damageLog;
    public TMP_Dropdown skillAttrDropDown, skillPowerDropDown, skillSelection;
    SelectionModal _selectionModal;

    Fighter _attacker;

    [SerializeField]Toggle _backStabtoggle;
    public void Initialize(List<Entity> combatants)
    {
        _statusBlock = transform.GetChild(1);
        _selectionModal = GetComponent<SelectionModal>();
        GameObject g;

        for(int i = 0; i < combatants.Count; i++)
        {
            g = Instantiate(_damageLogPrefab);
            g.transform.SetParent(_damageLog);
            g.transform.localScale = new Vector2(1f,1f);
            g.SetActive(false);
        }


    }

    public void SetAttackerProfile(Fighter attacker)
    {
        if(_statusBlock == null)
            _statusBlock = transform.GetChild(1);
        
        _statusBlock.GetChild(0).GetComponent<TMP_Text>().SetText(attacker.charName);
        _statusBlock.GetChild(1).GetComponent<Image>().sprite = attacker.portrait;
        _attacker = attacker;
        skillSelection.options.Clear();
        foreach(Skill s in attacker.skillsList)
        {
            skillSelection.options.Add(new TMP_Dropdown.OptionData(s.name,null));
        }
    }

    public float TotalAdvantageMod(int attackerAttr,int defenderAttr)
    {
        float advantageMod = 0f;

        if (attackerAttr == 6)
            return 0f;
        

        if ((attackerAttr == lightAttr && defenderAttr == darkAttr) || (attackerAttr == darkAttr && defenderAttr == lightAttr))
        {
            advantageMod += 0.25f;
        }
        else if (attackerAttr < lightAttr)
        {
            if (defenderAttr == ((attackerAttr + 1) % 4))
                advantageMod += 0.25f;

            int tempAttackerAttr = (attackerAttr - 1) % 4;

            if (tempAttackerAttr < 0)
                tempAttackerAttr = 3;

            if (defenderAttr == tempAttackerAttr)
                advantageMod -= 0.25f;
        }

        return advantageMod;
    }


    public float DamagePowerMod(int i,Fighter defender)
    {
        
        if (i == 6)
            return 0f;
        return _attacker.GetFinalPower(i,false) - defender.GetFinalPower(i,true);
    }

    public float RacialBane(EnumLibrary.Race targetRace, float baneMod, Fighter defender)
    {
        if (targetRace == EnumLibrary.Race.Empty || (defender.species1 != targetRace && defender.species2 != targetRace))
            return 0f;
        
        return baneMod;
    }

    public float ElementPowerMod(int i,Fighter defender)
    {
        if (i == 6)
            return 0f;
        return _attacker.GetFinalElemental(i,false) - defender.GetFinalElemental(i,true) ;
    }

    public float ScaleATK(EnumLibrary.ATKScale scaleFormula, Fighter attacker)
    {
        float attack = 0f;

        float PATK = attacker.GetFinalStat((int)EnumLibrary.Stats.PATK), AGI = attacker.GetFinalStat((int)EnumLibrary.Stats.AGI),DEX = attacker.GetFinalStat((int)EnumLibrary.Stats.TEC);
        float MDEF = attacker.GetFinalStat((int)EnumLibrary.Stats.MDEF);
        float level = attacker.level;

        Debug.Log("Scale Formula is type "+scaleFormula.ToString());

        switch(scaleFormula)
        {
            case EnumLibrary.ATKScale.NormalWarrior:
                attack = 1.2f * attacker.GetFinalStat((int)EnumLibrary.Stats.PATK);
            break;
            case EnumLibrary.ATKScale.NormalMage:
                attack = 1.2f * attacker.GetFinalStat((int)EnumLibrary.Stats.MATK);
            break;
            case EnumLibrary.ATKScale.HeavyWarrior:
                attack = 1.5f * attacker.GetFinalStat((int)EnumLibrary.Stats.PATK);
            break;
            case EnumLibrary.ATKScale.HeavyMage:
                attack = 1.5f * attacker.GetFinalStat((int)EnumLibrary.Stats.MATK);
            break;
            case EnumLibrary.ATKScale.DEFScale:
                attack = 1.5f *  attacker.GetFinalStat((int)EnumLibrary.Stats.PDEF);
            break;
            case EnumLibrary.ATKScale.NimbleWarrior:
                Debug.Log("PATK is "+PATK+" AGI is"+AGI+" DEX is "+DEX);
                attack = 0.75f * (PATK + AGI/2 + DEX + AGI * level/100f + DEX/4);
            break;
            case EnumLibrary.ATKScale.NimbleThief:
                Debug.Log("PATK is "+PATK+" AGI is"+AGI+" DEX is "+DEX);
                attack = 0.65f * (PATK + AGI/2 + DEX + AGI * level/100f + DEX/4);
            break;
            case EnumLibrary.ATKScale.Archer:
                attack = 0.7f * (PATK + DEX);
            break;
            case EnumLibrary.ATKScale.Gunner:
                attack = 0.8f * (PATK +DEX);
            break;
            case EnumLibrary.ATKScale.HalfMDEF:
                attack = 0.8f * MDEF;
            break;
            default:
                attack = 100f;
            break;
        }
        return attack;
    }

    public float ScaleDEF(EnumLibrary.DEFScale defScale, Fighter defender, int position)
    {
        Skill attackingSkill = _attacker.skillsList[skillSelection.value];
        if (defScale == EnumLibrary.DEFScale.Physical)
            return defender.GetFinalStat((int)EnumLibrary.Stats.PDEF) * (1 - attackingSkill.defIgnore[position]);
        else if (defScale == EnumLibrary.DEFScale.Magical)
            return defender.GetFinalStat((int)EnumLibrary.Stats.MDEF) * (1 - attackingSkill.defIgnore[position]);
        return 0f;
    }

    public string FinalAC(Fighter attacker, Fighter defender)
    {
        float finalAC = 4 + (defender.GetFinalStat((int)EnumLibrary.Stats.AGI) - attacker.GetFinalStat((int)EnumLibrary.Stats.TEC)/2f) * 0.12f;
        finalAC /= 100f;
        Debug.Log("EVA UP is "+defender.otherStats[(int)EnumLibrary.OtherStats.EVAUP]);
        finalAC += defender.otherStats[(int)EnumLibrary.OtherStats.EVAUP] - attacker.otherStats[(int)EnumLibrary.OtherStats.HITUP];

        Debug.Log("Final AC for "+defender.charName+" before the D20 is "+finalAC);

        return ((finalAC * 20) + 1f).ToString("F0");
    }

    public string CritChance(Fighter attacker, Fighter defender)
    {
        float finalCrit = Mathf.Sqrt(attacker.GetFinalStat((int)EnumLibrary.Stats.CRIT))*2 - Mathf.Sqrt(defender.GetFinalStat((int)EnumLibrary.Stats.LUCK))/2;
        finalCrit /= 100f;
        finalCrit += attacker.otherStats[(int)EnumLibrary.OtherStats.CRITUP];

        return Mathf.Ceil((1-finalCrit) * 20).ToString("F0");
    }

    public void CalculateDamage()
    {
        float totalModifier = 1f, attack;
        float finalDamage = 0f;
        List<ToggleCharacter> defenderList = _selectionModal.ReturnSelected();
        Skill attackingSkill = _attacker.skillsList[skillSelection.value];

        attack = ScaleATK(attackingSkill.atkScale,_attacker);
        if (defenderList.Count > 0)
        {
            // totalModifier *= (1f + TotalAdvantageMod((_attacker.attribute,) + TotalAdvantageMod(skillAttrDropDown.value)));
            // totalModifier *= (1f + DamagePowerMod());
            // totalModifier *= (1f + ElementPowerMod());

            for(int i = 0; i < _damageLog.childCount; i++)
            {
                if (i >= defenderList.Count)
                {
                    _damageLog.GetChild(i).gameObject.SetActive(false);
                    continue;
                }

                Fighter defender = defenderList[i].heldEntity.getFighterData;

                if (attackingSkill.skillTag1 == EnumLibrary.SkillType.Damage || attackingSkill.skillTag2 == EnumLibrary.SkillType.Damage)
                {
                    totalModifier += (TotalAdvantageMod((int)_attacker.attribute,(int)defender.attribute) + TotalAdvantageMod((int)attackingSkill.attribute,(int)defender.attribute));
                    totalModifier += (DamagePowerMod((int)attackingSkill.powerType,defender));
                    totalModifier += (ElementPowerMod((int)attackingSkill.attribute,defender));
                    totalModifier += RacialBane(attackingSkill.racialBane,attackingSkill.racialBaneMod,defender);
                    Debug.Log("Attack is "+attack+" and final modifier is"+totalModifier+" scaled DEF is "+ScaleDEF(attackingSkill.defScale,defender,defenderList[i].GetDropDownValue(0)));
                    finalDamage = (attack * attackingSkill.skillPower * totalModifier - ScaleDEF(attackingSkill.defScale,defender,defenderList[i].GetDropDownValue(0))) - UnityEngine.Random.Range(1,10f);
                    finalDamage = Mathf.Max(finalDamage,1);
                    _damageLog.GetChild(i).gameObject.SetActive(true);
                    _damageLog.GetChild(i).GetComponent<TMP_Text>().SetText(defender.charName+"'s AC is <b>"+FinalAC(_attacker,defender)+
                    "</b>,projected damage is <b>"+finalDamage.ToString("F0")+"</b>, and crit number is "+CritChance(_attacker,defender));
                }
                else if(attackingSkill.skillTag1 != EnumLibrary.SkillType.None)
                {
                    if(attackingSkill.atkScale == EnumLibrary.ATKScale.HalfMDEF)
                    {
                        finalDamage = Mathf.Max(finalDamage,1);
                        _damageLog.GetChild(i).gameObject.SetActive(true);
                        _damageLog.GetChild(i).GetComponent<TMP_Text>().SetText("Restore "+finalDamage+" HP to "+defender.charName);
                    }
                }
                
            }

            //finalDamage = Mathf.Round(finalDamage * totalModifier);
        }

        //display.text = "Final Damage is "+finalDamage.ToString();
    }


}
