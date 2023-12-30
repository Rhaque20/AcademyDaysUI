using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleButton : MonoBehaviour
{
    [SerializeField]Image buttonIcon,shield;
    public StatusModal statusWindow;
    [SerializeField]bool isBuff = true;
    [SerializeField]bool isResistance = false, isElement = true;
    Toggle toggle;
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
        if (isElement)
            buttonIcon.sprite = BattleManager.instance.elementIcons[(int)attribute];
        else if(power != EnumLibrary.DamagePower.NonType)
            buttonIcon.sprite = BattleManager.instance.powerIcons[(int)power];
        else
            buttonIcon.sprite = BattleManager.instance.statIcons[(int)stat];
        
        toggle = GetComponent<Toggle>();
        // toggle.onValueChanged.AddListener(OnToggleValueChanged);
        if (isResistance)
            shield.gameObject.SetActive(true);
    }

    public void SummonListItem()
    {
        OnToggleValueChanged();
        statusWindow.SummonListItem(GetComponent<ToggleButton>(),toggle.isOn);
    }

    private void OnToggleValueChanged()
    {
        isBuff = (statusWindow.dropDownVal == 0);
        Debug.Log(gameObject.name+"'s toggle has changed!");
        Image buttonBack = transform.GetChild(0).GetComponent<Image>();
        if (toggle.isOn)
            buttonBack.color = (isBuff ? statusWindow.buffColor:statusWindow.debuffColor);
        else
            buttonBack.color = Color.white;
    }
}
