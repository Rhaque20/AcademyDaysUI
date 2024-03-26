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

                Debug.Log("Adding to display "+status.statusName);
                g.GetComponent<StatusIcon>().SetStatus(status);
                effectIndex.Add(status.statusName,i);
                break;

            }
        }
    }

}
