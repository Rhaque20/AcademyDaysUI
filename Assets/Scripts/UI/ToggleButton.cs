using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleButton : GenericToggle
{
    [SerializeField]Image _buttonIcon,_shield;
    public StatusModal statusWindow;
    [SerializeField]bool isBuff = true;
    [SerializeField]bool isResistance = false, isElement = true;
    
    public EnumLibrary.Element attribute;
    public EnumLibrary.DamagePower power;
    public EnumLibrary.Stats stat;

    public bool resistance
    {
        get{return isResistance;}
    }
    public bool elemental
    {
        get{return isElement;}
    }
    public bool toggledBuff
    {
        get{return isBuff;}
    }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (isElement)
            _buttonIcon.sprite = BattleManager.instance.elementIcons[(int)attribute];
        else if(power != EnumLibrary.DamagePower.NonType)
            _buttonIcon.sprite = BattleManager.instance.powerIcons[(int)power];
        else
            _buttonIcon.sprite = BattleManager.instance.statIcons[(int)stat];
    
        // toggle.onValueChanged.AddListener(OnToggleValueChanged);
        if (isResistance)
            _shield.gameObject.SetActive(true);
    }

    public void SummonListItem()
    {
        OnToggleValueChanged();
        statusWindow.SummonListItem(GetComponent<ToggleButton>(),_toggle.isOn);
    }

    public override void OnToggleValueChanged()
    {
        isBuff = (statusWindow.dropDownVal == 0);
        Debug.Log(gameObject.name+"'s toggle has changed!");
        if (_toggle.isOn)
            _buttonBack.color = (isBuff ? statusWindow.buffColor:statusWindow.debuffColor);
        else
            _buttonBack.color = Color.white;
    }
}
