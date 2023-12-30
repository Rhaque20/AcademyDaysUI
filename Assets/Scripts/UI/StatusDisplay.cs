using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusDisplay : MonoBehaviour
{
    const int maxEffect = 10;
    [SerializeField]public GameObject statusPrefab;
    [SerializeField]Sprite onUse;
    public Color buffColor, debuffColor;
    public Dictionary<string,int> effectIndex = new Dictionary<string,int>();
    bool _initialized = false;
    // Start is called before the first frame update
    public void Initialize()
    {
        if(_initialized)
            return;
        
        GameObject g;
        Debug.Log("Initializing Status Display");
        for(int i = 0; i < maxEffect; i++)
        {
            g = Instantiate(statusPrefab);
            g.transform.SetParent(this.transform);
            g.transform.localScale = new Vector3(1f,1f,1f);
            g.SetActive(false);
        }
        _initialized = true;
    }

    public void UpdateDisplay(StatusEffect status)
    {
        if (effectIndex.ContainsKey(status.statusName))
        {
            int childIndex = effectIndex[status.statusName];
            Transform t = transform.GetChild(childIndex);
            t.GetChild(2).GetComponent<TMP_Text>().text = status.duration.ToString();
        }
        else
        {
            Debug.Log("Cannot find "+status.statusName);
            foreach (KeyValuePair<string, int> entry in effectIndex)
            {
                Debug.Log("Buff Type: "+entry.Key);
            }
        }
    }

    public void RemoveFromDisplay(string name)
    {
        if (effectIndex.ContainsKey(name))
        {
            int childIndex = effectIndex[name];
            Transform t = transform.GetChild(childIndex);
            t.gameObject.SetActive(false);
        }
    }


    public void AddDisplay(StatusEffect status)
    {
        Transform g;
        Debug.Log("Adding to display for "+transform.parent.name+" of "+transform.name+" with child Count "+transform.childCount);
        for(int i = 0; i < maxEffect; i++)
        {
            g = transform.GetChild(i);

            if (!g.gameObject.activeSelf)
            {
                Image backDrop = g.GetChild(0).GetComponent<Image>();
                Image icon = g.GetChild(1).GetComponent<Image>();
                TMP_Text duration = g.GetChild(2).GetComponent<TMP_Text>();
                Image shield = g.GetChild(3).GetComponent<Image>();

                if (status.onUse)
                {
                    backDrop.sprite = onUse;
                }
                else
                {
                    backDrop.sprite = null;
                }

                if(status.isBuff)
                {
                    backDrop.color = buffColor;
                }
                else
                {
                    backDrop.color = debuffColor;
                }

                if (status.isElement)
                {
                    icon.sprite = BattleManager.instance.elementIcons[(int)status.attribute];
                }
                else if(!status.isStatUp)
                {
                    icon.sprite = BattleManager.instance.powerIcons[(int)status.power];
                }
                else
                {
                    icon.sprite = BattleManager.instance.statIcons[(int)status.stat];
                }

                shield.gameObject.SetActive(status.isRes);

                Debug.Log("Adding to display "+status.statusName);
                
                duration.text = status.duration.ToString();
                effectIndex.Add(status.statusName,i);
                g.gameObject.SetActive(true);
                break;

            }
        }
    }

}
