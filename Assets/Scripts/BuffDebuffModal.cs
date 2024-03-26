using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffModal : MonoBehaviour
{
    // Start is called before the first frame update

    const int REDUCEUSE = 0, REDUCEDURATION = 1, REMOVE = 2;
    [SerializeField]GameObject _statusIconPrefab;
    [SerializeField]Transform _iconList,_charList;

    [SerializeField]TMP_Dropdown _dropDown;

    Entity _activeEntity;

    void Start()
    {
        GameObject temp;
        for(int i = 0; i < 10; i++)
        {
            temp = Instantiate(_statusIconPrefab);
            temp.transform.SetParent(_iconList);
            temp.transform.localScale = new Vector2(1f,1f);
            temp.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    public void OpenModal(Entity currentEntity)
    {
        this.gameObject.SetActive(true);
        int i = 0;

        _activeEntity = currentEntity;

        GameObject g;

        foreach (KeyValuePair<string, StatusEffect> entry in currentEntity.getFighterData.statuses)
        {
            g = _iconList.GetChild(i).gameObject;
            g.SetActive(true);
            g.GetComponent<StatusIcon>().SetStatus(entry.Value);
            i++;
        }

        for(int j = i; j < 10; j++)
        {
            _iconList.GetChild(j).gameObject.SetActive(false);
        }
    }

    List<StatusIcon> ReturnSelected()
    {
        List<StatusIcon> temp = new();

        Transform t;

        for(int i = 0; i < 10; i++)
        {
            t = _iconList.GetChild(i);
            if(t.GetComponent<Toggle>().isOn)
            {
                temp.Add(t.GetComponent<StatusIcon>());
            }
        }

        return temp;
    }

    public void PerformEffect()
    {
        List<StatusIcon> statuses = ReturnSelected();
        Debug.Log("Performed magic!!!!");
        switch(_dropDown.value)
        {
            case REDUCEUSE:
                foreach(StatusIcon si in statuses)
                {
                    if (si.currentStatus.onUse)
                    {
                        _activeEntity.DecrementBuff(si.currentStatus.statusName);

                        if(si.currentStatus.duration <= 0)
                            si.gameObject.SetActive(false);
                        else
                            si.UpdateStatus();
                    }
                }
                break;
            case REDUCEDURATION:
                foreach(StatusIcon si in statuses)
                {
                    if (!si.currentStatus.onUse)
                    {
                        _activeEntity.DecrementBuff(si.currentStatus.statusName);

                        if(si.currentStatus.duration <= 0)
                            si.gameObject.SetActive(false);
                        else
                            si.UpdateStatus();
                    }
                }
                break;
            case REMOVE:
                foreach(StatusIcon si in statuses)
                {
                    si.gameObject.SetActive(false);
                    _activeEntity.RemoveEffect(si.currentStatus.statusName);
                }
                break;
        }
    }

    // Update is called once per frame

}
