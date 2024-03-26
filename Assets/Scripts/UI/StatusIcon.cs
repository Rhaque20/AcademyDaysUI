using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class StatusIcon : MonoBehaviour
{
    StatusEffect _currentStatus;
    TMP_Text _duration;

    [SerializeField]
    private Color buffColor, debuffColor;

    private Image _backDrop,_icon,_shield;

    [SerializeField]
    private Sprite _onUse;

    public StatusEffect currentStatus
    {
        get{return _currentStatus;}
    }
    
    public void StartStatus()
    {
        _backDrop = transform.GetChild(0).GetComponent<Image>();
        _icon = transform.GetChild(1).GetComponent<Image>();
        _duration = transform.GetChild(2).GetComponent<TMP_Text>();
        _shield = transform.GetChild(3).GetComponent<Image>();
    }

    public void UpdateStatus()
    {
        if(_currentStatus == null)
            return;
        
        SetStatus(_currentStatus);
    }

    public void SetStatus(StatusEffect newStatus)
    {
        _currentStatus = newStatus;
        this.gameObject.SetActive(true);

        if(_backDrop == null)
            StartStatus();

        if (newStatus.onUse)
        {
            _backDrop.sprite = _onUse;
        }
        else
        {
            _backDrop.sprite = null;
        }

        if(newStatus.isBuff)
        {
            _backDrop.color = buffColor;
        }
        else
        {
            _backDrop.color = debuffColor;
        }

        if (newStatus.isElement)
        {
            _icon.sprite = BattleManager.instance.elementIcons[(int)newStatus.attribute];
        }
        else if(!newStatus.isStatUp)
        {
            _icon.sprite = BattleManager.instance.powerIcons[(int)newStatus.power];
        }
        else
        {
            _icon.sprite = BattleManager.instance.statIcons[(int)newStatus.stat];
        }

        _shield.gameObject.SetActive(newStatus.isRes);
        _duration.text = newStatus.duration.ToString();
    }

    public void ClearStatus()
    {
        this.gameObject.SetActive(false);
    }
}
