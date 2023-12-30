using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class StatusInput : MonoBehaviour
{
    public TMP_InputField inputField;
    private Image backDrop,shield, icon;
    public bool isResistance;
    public EnumLibrary.Element attribute;
    public EnumLibrary.DamagePower power;
    public EnumLibrary.Stats stats;
    public bool isElement = false;
    public bool isStatUp = false;
    [SerializeField]bool isBuff = true, onUse = false;
    TMP_Text onUseNotification;

    public bool storedBuff
    {
        get{return isBuff;}
    }

    public bool onlyOnUse
    {
        get{return onUse;}
    }

    // Start is called before the first frame update
    void Start()
    {
        backDrop = transform.GetChild(0).GetComponent<Image>();
        inputField = transform.GetChild(1).GetComponent<TMP_InputField>();
        icon = transform.GetChild(2).GetComponent<Image>();
        shield = transform.GetChild(3).GetComponent<Image>();
        onUseNotification = transform.GetChild(4).GetComponent<TMP_Text>();

        shield.gameObject.SetActive(isResistance);
        this.gameObject.SetActive(false);
        onUseNotification.gameObject.SetActive(false);
    }

    public void ClearData()
    {
        inputField.text = "0";
        this.gameObject.SetActive(false);
    }

    public void SetUp(ToggleButton status, Color backColor, bool onUse)
    {
        isElement = status.elemental;
        if(isElement)
        {
            attribute = status.attribute;
            icon.sprite = BattleManager.instance.elementIcons[(int)attribute];

        }
        else if(status.power != EnumLibrary.DamagePower.NonType)
        {
            power = status.power;
            icon.sprite = BattleManager.instance.powerIcons[(int)power];
        }
        else
        {
            stats = status.stat;
            icon.sprite = BattleManager.instance.statIcons[(int)stats];
            isStatUp = true;
        }

        shield.gameObject.SetActive(status.resistance);
        backDrop.color = backColor;
        isBuff = status.toggledBuff;
        this.onUse = onUse;
        onUseNotification.gameObject.SetActive(onUse);
        isResistance = status.resistance;
    }
}
