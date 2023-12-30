using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIcon : MonoBehaviour
{
    StatusEffect currentStatus;
    Image backDrop,icon;
    TMP_Text duration;
    
    public void StartStatus()
    {
        backDrop = transform.GetChild(0).GetComponent<Image>();
        icon = transform.GetChild(1).GetComponent<Image>();
        duration = transform.GetChild(2).GetComponent<TMP_Text>();
    }

    public void SetStatus(StatusEffect newStatus)
    {
        currentStatus = newStatus;
        this.gameObject.SetActive(true);
    }

    public void ClearStatus()
    {
        this.gameObject.SetActive(false);
    }
}
