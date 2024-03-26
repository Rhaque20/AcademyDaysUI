using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionModal : MonoBehaviour,IUserInterface
{
    [SerializeField]Transform effectContent;
    [SerializeField]StatusModal mainModal;
    

    // void Start()
    // {
    //     for(int i = 0; i < effectContent.childCount; i++)
    //     {
    //         effectContent.GetChild(i).GetChild(4).GetComponent<StatusDisplay>().statusPrefab = statusDisplayPrefab;
    //         //effectContent.GetChild(i).GetComponent<ToggleCharacter>().Initialize();
    //     }
    // }
    public void ApplyStatustoSelected()
    {
        ToggleCharacter temp;
        for(int i = 0; i < effectContent.childCount; i++)
        {
            temp = effectContent.GetChild(i).GetComponent<ToggleCharacter>();
            if (temp.isOn)
            {
                if(temp.heldEntity == null)
                {
                    Debug.LogError("Not initialized heldEntity of temp");
                }
                temp.heldEntity.GenerateStatusEffects(mainModal.ApplyData());
            }
            
        }

        this.gameObject.SetActive(false);
    }

    public List<ToggleCharacter> ReturnSelected()
    {
        List<ToggleCharacter> selected = new List<ToggleCharacter>();
        ToggleCharacter temp;

        for(int i = 0; i < effectContent.childCount; i++)
        {
            temp = effectContent.GetChild(i).GetComponent<ToggleCharacter>();
            if (temp.isOn)
            {
                selected.Add(temp);
            }
            
        }

        return selected;
    }

    public void ApplyAVShift(float pushPercent)
    {
        ToggleCharacter temp;
        bool hasShifted = false;
        for(int i = 0; i < effectContent.childCount; i++)
        {
            temp = effectContent.GetChild(i).GetComponent<ToggleCharacter>();
            if (temp.isOn)
            {
                hasShifted = true;
                //temp.heldEntity.getFighterData.AlterActionValue(pushPercent);
                temp.heldEntity.getFighterData.FocusPush(pushPercent);
            }
            
        }

        if(hasShifted)
        {
            TurnOrderManager.instance.GenerateTurnList();
        }
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
