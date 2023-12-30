using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusModal : MonoBehaviour
{
    Transform applyStatus,currentStatus;
    public Color buffColor, debuffColor;
    [SerializeField]Transform powerButton,resistanceButton, statButton, afflictionButton,effectListContent;
    public enum CurrentState{Off,applyStatus,currentStatus};
    CurrentState activeState = CurrentState.applyStatus;
    TMP_Dropdown statusType;
    bool initializedCurrent = false;
    [SerializeField]Toggle onUse, elementPowerToggle;
    Player activeFighter;

    [SerializeField]TMP_InputField turnDuration, useAmount;

    TMP_Text name;
    Image profile;

    [SerializeField]GameObject selectables;

    public int dropDownVal
    {
        get{
            return statusType.value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        name = transform.GetChild(2).GetComponent<TMP_Text>();
        profile = transform.GetChild(3).GetComponent<Image>();
        applyStatus = transform.GetChild(0);
        currentStatus = transform.GetChild(1);
        statusType = applyStatus.GetChild(0).GetComponent<TMP_Dropdown>();
        activeFighter = BattleManager.instance.ReturnFighter();

        ToggleButton temp;
        int i = 0;
        for(i = 0; i < powerButton.childCount; i++)
        {
            temp = powerButton.GetChild(i).GetComponent<ToggleButton>();
            if (temp == null)
                break;
            
            temp.statusWindow = GetComponent<StatusModal>();
            
            temp = resistanceButton.GetChild(i).GetComponent<ToggleButton>();
            temp.statusWindow = GetComponent<StatusModal>();

            temp = statButton.GetChild(i).GetComponent<ToggleButton>();
            temp.statusWindow = GetComponent<StatusModal>();
        }

        this.gameObject.SetActive(false);
    }

    public void InitializeCurrentStatus()
    {
        Debug.Log("initializing status area");
        Transform area = currentStatus.GetChild(0);//Elemental Area
        Transform powerTrack = area.GetChild(0),resTrack = area.GetChild(1), statTrack = currentStatus.GetChild(2);
        Image iconVar;
        TMP_Text statusVal;
        int i;

        for(i = 0; i < powerTrack.childCount; i++)
        {
            iconVar = powerTrack.GetChild(i).GetChild(0).GetComponent<Image>();
            statusVal = powerTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            powerTrack.GetChild(i).GetChild(2).gameObject.SetActive(false);
            iconVar.sprite = BattleManager.instance.elementIcons[i];
            GenerateValueText(statusVal,i,true,false);

            iconVar = resTrack.GetChild(i).GetChild(0).GetComponent<Image>();
            statusVal = resTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            iconVar.sprite = BattleManager.instance.elementIcons[i];
            GenerateValueText(statusVal,i,true,true);
        }

        area = currentStatus.GetChild(1);
        powerTrack = area.GetChild(0);
        resTrack = area.GetChild(1);

        for (i = 0; i < powerTrack.childCount; i++)
        {
            iconVar = powerTrack.GetChild(i).GetChild(0).GetComponent<Image>();
            statusVal = powerTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            powerTrack.GetChild(i).GetChild(2).gameObject.SetActive(false);
            iconVar.sprite = BattleManager.instance.powerIcons[i];
            GenerateValueText(statusVal,i,false,false);

            iconVar = resTrack.GetChild(i).GetChild(0).GetComponent<Image>();
            statusVal = resTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            iconVar.sprite = BattleManager.instance.powerIcons[i];
            GenerateValueText(statusVal,i,false,true);
        }

        for(i = 0; i < 11; i++)
        {
            iconVar = statTrack.GetChild(i).GetChild(0).GetComponent<Image>();
            statusVal = statTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            statTrack.GetChild(i).GetChild(2).gameObject.SetActive(false);

            iconVar.sprite = BattleManager.instance.statIcons[i];
            GenerateStatValueText(statusVal,i);
        }

        initializedCurrent = true;
    }

    public void SwitchStatuses()
    {
        powerButton.gameObject.SetActive(elementPowerToggle.isOn);
        resistanceButton.gameObject.SetActive(elementPowerToggle.isOn);
        statButton.gameObject.SetActive(!elementPowerToggle.isOn);
    }

    public void RefreshCurrent()
    {
        Debug.Log("Refreshing current");
        Transform area = currentStatus.GetChild(0);//Elemental Area
        Transform powerTrack = area.GetChild(0),resTrack = area.GetChild(1),statTrack = currentStatus.GetChild(2);
        TMP_Text statusVal;
        int i;

        for(i = 0; i < powerTrack.childCount; i++)
        {
            statusVal = powerTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            GenerateValueText(statusVal,i,true,false);
            statusVal = resTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            GenerateValueText(statusVal,i,true,true);
        }

        area = currentStatus.GetChild(1);
        powerTrack = area.GetChild(0);
        resTrack = area.GetChild(1);

        for (i = 0; i < powerTrack.childCount; i++)
        {
            statusVal = powerTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            GenerateValueText(statusVal,i,false,false);
            statusVal = resTrack.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
            GenerateValueText(statusVal,i,false,true);
        }

        for(i = 0; i < 11; i++)
        {
            statusVal = statTrack.GetChild(i).GetComponent<TMP_Text>();
            GenerateStatValueText(statusVal,i);
        }

        
    }

    public void GenerateStatValueText(TMP_Text statusVal, int i)
    {
        float finalStat = activeFighter.GetFinalStat(i);

        statusVal.text = finalStat.ToString();
    }

    public void GenerateValueText(TMP_Text statusVal,int i,bool isElement,bool isRes)
    {
        float finalStat;

        if (isElement)
        {
            finalStat = activeFighter.GetFinalElemental(i,isRes);
            Debug.Log("Final stat is "+finalStat+" for "+(EnumLibrary.Element)i);
        }
        else
        {
            finalStat = activeFighter.GetFinalPower(i,isRes);
            Debug.Log("Final stat is "+finalStat+" for "+(EnumLibrary.DamagePower)i);
        }

        

        if (finalStat > 0f)
        {
            statusVal.color = buffColor;
            statusVal.text = "+";
        }
        else if (finalStat < 0f)
        {
            statusVal.color = debuffColor;
            statusVal.text = "";
        }
        else
        {
            statusVal.color = Color.black;
            statusVal.text = "";
        }

        finalStat *= 100f;

        statusVal.text += finalStat.ToString()+"%";
    }

    public void SummonListItem(ToggleButton toggleData, bool isOn)
    {
        Debug.Log("Summoning list item!");
        GameObject temp;
        if (isOn)
        {
            for(int i = 0; i < effectListContent.childCount;i++)
            {
                temp = effectListContent.GetChild(i).gameObject;
                if (!temp.activeSelf)
                {
                    temp.SetActive(true);
                    temp.GetComponent<StatusInput>().SetUp(toggleData,(toggleData.toggledBuff ? buffColor:debuffColor),onUse.isOn);
                    break;
                }
            }
        }
        else
        {
            for(int i = 0; i < effectListContent.childCount;i++)
            {
                temp = effectListContent.GetChild(i).gameObject;
                if (temp.activeSelf)
                {
                    StatusInput status = temp.GetComponent<StatusInput>();

                    if (status.isElement && toggleData.elemental)
                    {
                        if (status.attribute == toggleData.attribute)
                        {
                            status.ClearData();
                            break;
                        }
                    }
                    else if (!status.isElement && !toggleData.elemental)
                    {
                        if (status.power == toggleData.power)
                        {
                            status.ClearData();
                            break;
                        }
                    }
                }
            }
        }
    }

    public void BringUpSelectionModal()
    {
        selectables.SetActive(true);
    }

    public List<StatusEffect> ApplyData()
    {
        List<StatusEffect> statuses = new List<StatusEffect>();
        int i = 0;

        GameObject g;
        StatusInput effectConvert;
        StatusEffect status;
        string statusName = new string("");
        string tempstring = new string("");
        bool isStatUp = false;

        for (i = 0; i < effectListContent.childCount; i++)
        {
            g = effectListContent.GetChild(i).gameObject;
            if (g.activeSelf)
            {
                effectConvert = g.GetComponent<StatusInput>();
                if (effectConvert.isElement)
                {
                    statusName = effectConvert.attribute.ToString();
                }
                else if (!effectConvert.isStatUp)
                {
                    statusName = effectConvert.power.ToString();
                }
                else
                {
                    statusName = effectConvert.stats.ToString();
                    isStatUp = true;
                }

                if(!isStatUp)
                    statusName +=(effectConvert.isResistance ? " Res ":" Power ")+(effectConvert.storedBuff ? "Up":"Down");
                else
                    statusName += (effectConvert.storedBuff ? " Up":" Down");
                
                if (effectConvert.onlyOnUse)
                {
                    tempstring = useAmount.text;
                    if (tempstring.Equals(""))
                        tempstring = "1";
                    statusName += " (On Use)";
                }
                else
                {
                    tempstring = turnDuration.text;
                    if (tempstring.Equals(""))
                        tempstring = "3";
                }
                status = new StatusEffect(statusName,effectConvert,int.Parse(tempstring));
                statuses.Add(status);
            }
        }

        return statuses;
    }

    public void ChangeState(int i)
    {
        switch(i)
        {
            case 0:
                ApplyData();
                this.gameObject.SetActive(false);
                applyStatus.gameObject.SetActive(false);
                activeState = CurrentState.Off;
                break;
            case 1:
                this.gameObject.SetActive(true);
                applyStatus.gameObject.SetActive(true);
                currentStatus.gameObject.SetActive(false);
                activeState = CurrentState.applyStatus;
                activeFighter = BattleManager.instance.ReturnFighter();
                name.text = activeFighter.charName;
                profile.sprite = activeFighter.portrait;
                break;
            case 2:
                this.gameObject.SetActive(true);
                applyStatus.gameObject.SetActive(false);
                currentStatus.gameObject.SetActive(true);
                activeState = CurrentState.currentStatus;
                activeFighter = BattleManager.instance.ReturnFighter();
                if (!initializedCurrent)
                {
                    InitializeCurrentStatus();
                }
                else
                    RefreshCurrent();

                name.text = activeFighter.charName;
                profile.sprite = activeFighter.portrait;
                break;
            
        }
    }

}
