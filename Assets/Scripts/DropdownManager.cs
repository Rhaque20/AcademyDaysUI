using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]List<GameObject> content = new List<GameObject>();
    List<int> fighterindices = new List<int>();
    Timeline tl;
    public enum States{applyBuff,applyCRPush};
    public States dropState = States.applyBuff;

    //List<bool> inUse = new List<bool>();
    public void Initialize(Timeline t)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            content.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).gameObject.SetActive(false);
            //inUse.Add(false);
            content[i].GetComponent<ListOption>().Initialize(GetComponent<DropdownManager>());
        }
        tl = t;
    }

    public void ClearInUse()
    {
        for(int i = 0; i < content.Count; i++)
        {
            //inUse[i] = false;
            content[i].SetActive(false);
        }
        fighterindices.Clear();
    }

    public void SetState(int i)
    {
        dropState = (States)i;
    }

    public void SetUpCharacterList(Dictionary<string,GameObject> characters, bool searchPlayer)
    {
        Character c;
        GameObject g;
        int i = 0;
        foreach(KeyValuePair<string, GameObject> kvp in characters)
        {
            g = kvp.Value;
            c = g.GetComponent<Character>();
            if (!c.isSpell)
            {
                if (searchPlayer == c.isPlayer)
                {
                    //Debug.Log("Adding: "+c.name);
                    //fighterindices.Add(c.index);
                    content[i].GetComponent<ListOption>().SetUpContent(c.charName,null,c.activePortrait,i);
                    i++;
                    //inUse[i] = true;
                }
            }

            if (i >= content.Count)
                break;
            
        }
    }

    public void CRPush(string s,float CRPush)
    {
        tl.CRPush(s,CRPush);
    }

    public void ApplyTimeEffect(string s)
    {
        tl.ApplyTimeEffect(s);
    }

    public void SetUpSkillList()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
