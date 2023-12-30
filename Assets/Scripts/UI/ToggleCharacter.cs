using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleCharacter : MonoBehaviour
{
    //public GameObject statusPrefab;
    Image backDrop,profileIcon,elementIcon;
    TMP_Text name,actionVal;
    GameObject statusList;
    Toggle curToggle;
    public Entity heldEntity;
    public int toggleType = 0;
    public bool hasEntity = false;
    // Start is called before the first frame update

    public bool isOn
    {
        get{return curToggle.isOn;}
    }

    void Start()
    {
        GetParts();
        statusList.GetComponent<StatusDisplay>().Initialize();
    }
    void GetParts()
    {
        backDrop = transform.GetChild(0).GetComponent<Image>();
        profileIcon = transform.GetChild(1).GetComponent<Image>();
        elementIcon = transform.GetChild(2).GetComponent<Image>();
        name = transform.GetChild(3).GetComponent<TMP_Text>();
        statusList = transform.GetChild(4).gameObject;
        actionVal = transform.GetChild(5).GetComponent<TMP_Text>();
        curToggle = GetComponent<Toggle>();
    }

    public void Initialize(Entity entireEntity)
    {
        GetParts();
        InputData(entireEntity);
    }

    public void Selected()
    {
        if (curToggle.isOn)
        {
            backDrop.color = new Color(0.9716981f,0.6569657f,0.6569657f,1.0f);
        }
        else
        {
            backDrop.color = Color.white;
        }
        if (toggleType == 1 && BattleManager.instance.selectingAttacker)
        {
            BattleManager.instance.SetFighter(heldEntity.getFighterData);
            //BattleManager.instance.SelectingAttacker();
        }
    }

    public void SetUpSpellUnit(Entity spell)
    {
        GetParts();
        //Caster details
        Player combatant = spell.entityOrigin.getFighterData;
        profileIcon.sprite = combatant.portrait;
        elementIcon.sprite = BattleManager.instance.elementIcons[combatant.elementInt];
        // Get spell data
        name.SetText(spell.getFighterData.charName);
        heldEntity = spell;
        actionVal.SetText(spell.getFighterData.focus.ToString());
        spell.getFighterData.FocusPush(-999);
    }


    public void InputData(Entity entireEntity)
    {
        Player combatant = entireEntity.getFighterData;
        if(combatant == null)
        {
            Debug.Log("No combatant in "+entireEntity.name);
        }
        profileIcon.sprite = combatant.portrait;
        elementIcon.sprite = BattleManager.instance.elementIcons[combatant.elementInt];
        name.text = combatant.charName;
        heldEntity = entireEntity;
        actionVal.SetText(entireEntity.getFighterData.focus.ToString("F0"));
    }



}
