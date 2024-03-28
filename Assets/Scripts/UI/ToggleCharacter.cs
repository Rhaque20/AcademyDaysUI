using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleCharacter : MonoBehaviour
{
    //public GameObject statusPrefab;

    public enum ToggleType{TurnOrderToggle,Character,DamageCalc}
    Image backDrop,profileIcon,elementIcon;
    TMP_Text name,actionVal;
    GameObject statusList;
    Toggle curToggle;
    public Entity heldEntity;
    public bool hasEntity = false;
    [SerializeField]private ToggleType _toggleType = ToggleType.TurnOrderToggle;
    [SerializeField]private GameObject _damageCalcDropDowns;
    TMP_Dropdown _stabPosition,_heightPosition;
    // Start is called before the first frame update

    public bool isOn
    {
        get{return curToggle.isOn;}
    }

    public ToggleType toggleType
    {
        get{return _toggleType;}
        set{_toggleType = value;}
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
        _damageCalcDropDowns = transform.GetChild(6).gameObject;
        _stabPosition = _damageCalcDropDowns.transform.GetChild(0).GetComponent<TMP_Dropdown>();
        _heightPosition = _damageCalcDropDowns.transform.GetChild(1).GetComponent<TMP_Dropdown>();
        curToggle = GetComponent<Toggle>();
    }

    public void Initialize(Entity entireEntity)
    {
        Debug.Log("Called regular initiatlize");
        GetParts();
        InputData(entireEntity);
    }

    public void Initialize(Entity entireEntity, ToggleType toggleType)
    {
        GetParts();
        InputData(entireEntity);

        _toggleType = toggleType;
        Debug.Log("Setting this toggle to damagecalc "+(_toggleType == ToggleType.DamageCalc));
        _damageCalcDropDowns.SetActive(_toggleType == ToggleType.DamageCalc);
    }

    public int GetDropDownValue(int index)
    {
        if(index == 0)
            return _stabPosition.value;
        else
            return _heightPosition.value;
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
        if (_toggleType == ToggleType.TurnOrderToggle && BattleManager.instance.selectingAttacker)
        {
            BattleManager.instance.SetFighter(heldEntity);
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
        toggleType = ToggleType.Character;
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
