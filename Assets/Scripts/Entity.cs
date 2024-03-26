using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Entity : MonoBehaviour,IComparable<Entity>
{
    [SerializeField]Player mainFighter,subFighter;
    bool mainChar = true;
    public int index;
    TMP_Text subName;
    Image icon,elementIcon;
    public EnumLibrary.Element newAttr = EnumLibrary.Element.Pure;
    public bool overrideAttr = false, initializeData = true, isSpellUnit = false;
    [SerializeField]StatusDisplay statusDisplay;


    // Used for spell units
    private Entity _entityOrigin;

    public Entity entityOrigin
    {
        get{return _entityOrigin;}
    }

    private float _castSpeed = 0f;

    public float actionValue
    {
        get{return getFighterData.actionValue;}
    }

    public Player getFighterData
    {
        get
        {
            if (mainChar)
                return mainFighter;
            else
                return subFighter;
        }
    }

    public Entity()
    {
        
    }

    public void CreateSpellUnit(Entity origin, string spellName, float spellSpeed)
    {
        
        mainFighter = origin.getFighterData;
        subFighter = Player.CreateInstance<Player>();
        subFighter.CreateSpellUnit(spellName,spellSpeed);
        mainChar = false;
        isSpellUnit = true;
        _entityOrigin = origin;
    }

    public void EditSpellUnit(Player caster, string spellName, float spellSpeed)
    {
        mainFighter = caster;
        subFighter.charName = spellName;
        subFighter.speed = spellSpeed;
    }

    public void SetUnit(Player dupeTarget)
    {
        mainFighter = dupeTarget;
    }

    // Start is called before the first frame update
    public void StartEntity()
    {
        if (initializeData)
        {
            Debug.Log("Initializing Entity");
            // if (getFighterData.unitType == Player.UnitType.Enemy)
            //     mainFighter = Player.CreateInstance<Player>();
            
            subName = transform.GetChild(3).GetComponent<TMP_Text>();
            icon = transform.GetChild(1).GetComponent<Image>();
            elementIcon = transform.GetChild(2).GetComponent<Image>();
            //statusDisplay = transform.GetChild(3).GetComponent<StatusDisplay>();
            statusDisplay.Initialize();

            icon.sprite = mainFighter.portrait;
            subName.SetText(mainFighter.charName);
            if (mainFighter.attribute != EnumLibrary.Element.Pure)
            {
                int attributeNumber = (int)mainFighter.attribute;
                elementIcon.sprite = BattleManager.instance.elementIcons[attributeNumber];
            }
        }
        
        //mainFighter.GenerateActionValue();

    }

    public void DecrementBuff(bool onUse, bool defense)
    {
        Dictionary<string,StatusEffect> statuses = getFighterData.statuses;
        List<string> entrytoDelete = new List<string>();
        foreach (KeyValuePair<string, StatusEffect> entry in statuses)
        {
            if (entry.Value.onUse == true && onUse == true)
            {
                if (defense == entry.Value.isRes)
                {
                    entry.Value.duration--;
                    statusDisplay.UpdateDisplay(entry.Value);

                    if (entry.Value.duration <= 0)
                    {
                        entrytoDelete.Add(entry.Key);
                    }
                }
            }
            else if (entry.Value.onUse == false && onUse == false)
            {
                Debug.Log("Decreasing "+entry.Key);
                entry.Value.duration--;
                statusDisplay.UpdateDisplay(entry.Value);

                if (entry.Value.duration <= 0)
                {
                    entrytoDelete.Add(entry.Key);
                }
            }
        }

        foreach(string s in entrytoDelete)
        {
            statusDisplay.RemoveFromDisplay(s);
            statuses.Remove(s);
        }
    }

    public void DecrementBuff(string statusName)
    {
        if(getFighterData.statuses.ContainsKey(statusName))
        {
            
            getFighterData.statuses[statusName].duration--;
            statusDisplay.UpdateDisplay(getFighterData.statuses[statusName]);

            if (getFighterData.statuses[statusName].duration <= 0)
            {
                getFighterData.statuses.Remove(statusName);
                statusDisplay.RemoveFromDisplay(statusName);
            }
        }
    }

    public void RemoveEffect(string statusName)
    {
        if(getFighterData.statuses.ContainsKey(statusName))
        {
            
            statusDisplay.RemoveFromDisplay(statusName);
            getFighterData.statuses.Remove(statusName);

        }
    }

    public void SetFighter()
    {
        BattleManager.instance.SetFighter(GetComponent<Entity>());
    }

    private void SetDisplay()
    {
        icon.sprite = getFighterData.portrait;
        subName.SetText(getFighterData.charName);
        if (getFighterData.attribute != EnumLibrary.Element.Pure)
        {
            int attributeNumber = (int)getFighterData.attribute;
            elementIcon.sprite = BattleManager.instance.elementIcons[attributeNumber];
        }
    }

    public void GenerateStatusEffects(List<StatusEffect> statlist)
    {
        StatusEffect temp;
        Dictionary<string,StatusEffect> statuses = getFighterData.statuses;

        foreach(StatusEffect status in statlist)
        {
            Debug.Log("Attempting to add in "+status.statusName);
            if(statuses.ContainsKey(status.statusName))
            {
                temp = statuses[status.statusName];

                if (status.efficacy >= temp.efficacy)
                {
                    temp.efficacy = status.efficacy;
                    temp.duration = status.duration;
                    statusDisplay.UpdateDisplay(temp);
                }
                continue;
            }
            else
            {
                Debug.Log("Adding in "+status.statusName+" with efficacy of "+status.efficacy+" to "+getFighterData.charName);
                statuses.Add(status.statusName,status);
                if (statusDisplay == null)
                    Debug.Log("Display is null");
                statusDisplay.AddDisplay(status);
            }

        }
    }

    public void SwitchFighter()
    {
        if (subFighter == null)
        {
            return;
        }

        getFighterData.ResetFocus();
        mainChar = !mainChar;
        SetDisplay();
        getFighterData.ResetFocus();
    }

    public int CompareTo(Entity other)
    {
        // if(getFighterData.actionValue < other.getFighterData.actionValue)
        //     return -1;
        // else if (getFighterData.actionValue > other.getFighterData.actionValue)
        //     return 1;
        
        // if (getFighterData.speed < other.getFighterData.speed)
        //     return -1;
        // else if (getFighterData.speed > other.getFighterData.speed)
        //     return 1;
        // // Add speed comparison here.
        // return 0;

        if(getFighterData.focus < other.getFighterData.focus)
            return -1;
        else if (getFighterData.focus > other.getFighterData.focus)
            return 1;
        
        if (getFighterData.speed < other.getFighterData.speed)
            return -1;
        else if (getFighterData.speed > other.getFighterData.speed)
            return 1;
        // Add speed comparison here.
        return 0;
    }
}
