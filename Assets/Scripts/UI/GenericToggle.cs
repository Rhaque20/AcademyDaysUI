using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenericToggle : MonoBehaviour
{
    protected Toggle _toggle;
    [SerializeField]
    private Color32 _selectedColor, _unselectedColor;

    [SerializeField]
    protected Image _buttonBack;

    protected void Start()
    {
        _toggle = GetComponent<Toggle>();

        if(_buttonBack == null)
            _buttonBack = transform.GetChild(0).GetComponent<Image>();
    }
    public virtual void OnToggleValueChanged()
    {
        _buttonBack.color = (_toggle.isOn ? _selectedColor : _unselectedColor);
    }
}