using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timeline : MonoBehaviour
{
    int fighterCount = 3;
    [SerializeField]float maxLength = 15f, hiFocus = 0f;
    bool ready = false;
    int minCycle = 0, member = -1;
    public int hasMaxCR = 0;
    // Start is called before the first frame update
    Dictionary<string,GameObject> fighters = new Dictionary<string,GameObject>();
    [SerializeField]GameObject activeFighter = null;
    [SerializeField]TMP_Text statusText = null;
    [SerializeField]GameObject spellUnit, Unit, ScrollUI;
    [SerializeField]DropdownManager dm;
    [SerializeField]TMPro.TMP_Dropdown dd, CRTurnEnd;
    [SerializeField]GameObject playerPrefab;
    string nextFighter = null;
    //float CRPush = 0f;


    void Start()
    {
        GameObject g;
        string s;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            g = transform.GetChild(i).gameObject;
            if (!g.activeSelf)
            {
                continue;
            }
            else if (g.CompareTag("Group"))
            {
                BreakDown(g);
            }
            else
            {
                s = g.GetComponent<Character>().charName;
                if (!fighters.ContainsKey(s))
                    fighters.Add(s,g);
            }
        }
        minCycle = -1;
        fighterCount = fighters.Count;
        StartRace();
        dm.Initialize(GetComponent<Timeline>());
    }

    public void PullUpParty(int i)
    {
        if (!CheckIfDone())
            return;
        
        ScrollUI.SetActive(true);
        Character c = activeFighter.GetComponent<Character>();

        dm.SetState(i);
        dm.SetUpCharacterList(fighters,c.isPlayer);
    }

    public void HideScrollElement()
    {
        dm.ClearInUse();
        ScrollUI.SetActive(false);
    }

    public void CastSpellUnit()
    {
        GameObject instance = Instantiate(spellUnit,this.transform);
        SpellUnit su = instance.GetComponent<SpellUnit>();
        su.SpellName = "Rumble Blossom";
        su.name = activeFighter.name;
        
    }

    public void SummonUnit()
    {

    }

    public void ApplyTimeEffect(string i)
    {
        Debug.Log("Index "+i);
        fighters[i].GetComponent<Character>().GrantBuff(dd.value); 
    }

    public int findMaxCR()
    {
        float maxCR = 0f;
        int maxTally = 0;
        string s = activeFighter.GetComponent<Character>().charName;
        Debug.Log("active Fighter is "+s);
        Character c;
        foreach(KeyValuePair<string, GameObject> kvp in fighters)
        {
            c = kvp.Value.GetComponent<Character>();
            if (kvp.Value == activeFighter)
                continue;
            if (c.CR >= 100f)
            {
                nextFighter = c.charName;
                maxTally++;
                
                if (c.CR > maxCR)
                    maxCR = c.CR;
            }
        }

        return maxTally;
    }

    public void CRPush(string i, float CRPush)
    {
        
        Debug.Log("CR pushing "+i);
        fighters[i].GetComponent<Character>().CRChange(CRPush);
        hasMaxCR = findMaxCR();
        
    }

    public float returnPosition(float ratio)
    {
        //Debug.Log("Position is "+(ratio*maxLength));
        if (ratio > 1f)
            ratio = 1f;
        return ratio * maxLength;
    }

    public void SetFighter()
    {
        activeFighter = fighters[nextFighter];
        statusText.text = "It's "+activeFighter.GetComponent<Character>().charName+"'s turn!";
        nextFighter = null;
    }

    public bool CheckIfDone()
    {
        bool evaluate = true;
        string index = null;
        float highestCR = 0f;
        Character c;

        foreach(KeyValuePair<string, GameObject> kvp in fighters)
        {
            c = kvp.Value.GetComponent<Character>();
            evaluate = c.isReady;
            if (index == null)
                index = kvp.Key;
            
            if (!evaluate)
            {
                return evaluate;
            }
            
            if (c.CR >= highestCR)
            {
                if (c.CR == highestCR)
                {
                    if (!fighters.ContainsKey(index))
                        continue;
                    
                    Character tempc = fighters[index].GetComponent<Character>();
                    if (c.AGI > tempc.AGI)// If the current AGI is the same as the previous AGI, move on
                    {
                        index = kvp.Key;
                        highestCR = c.CR;
                    }
                }
                else
                {
                    index = c.charName;
                    highestCR = c.CR;
                }
                
            }
        }

        activeFighter = fighters[index];
        statusText.text = "It's "+fighters[index].GetComponent<Character>().charName+"'s turn!";

        return evaluate;
    }

    public void BreakDown(GameObject parent)
    {
        GameObject g;
        string i;
        while (parent.transform.childCount > 0)
        {
            g = parent.transform.GetChild(0).gameObject;
            i = g.GetComponent<Character>().charName;
            fighters.Add(i,g);
            //i = fighters.Count - 1;
            //fighters[i].GetComponent<Character>().index = i;
            fighters[i].transform.localPosition = new Vector2(0f,0.4f);
            g.transform.SetParent(this.transform,false);
            //setMinCycle(g);
            fighterCount++;
        }
        Destroy(parent);
    }

    public int BreakDown(GameObject parent, int dummy)
    {
        GameObject g;
        string i;
        int temp = -1, curMin = -1;
        bool setInitial = false;
        while (parent.transform.childCount > 0)
        {
            g = parent.transform.GetChild(0).gameObject;
            i = g.GetComponent<Character>().charName;
            fighters.Add(i,g);
            //i = fighters.Count - 1;
            //fighters[i].GetComponent<Character>().index = i;
            fighters[i].transform.localPosition = new Vector2(0f,0.4f);
            g.transform.SetParent(this.transform,false);
            temp = setMinCycle(g);

            if (temp == -1)
                fighterCount++;
            else if (temp < curMin || !setInitial)
            {
                curMin = temp;
                Debug.Log("Minimum Cycles in Children "+curMin+ "from: "+i);
                setInitial = true;
            }

            fighterCount++;
        }
        Destroy(parent);
        return curMin;
    }

    public int setMinCycle(GameObject g)
    {
        Character c;
        string s;
        int temp = 0;
        s = g.GetComponent<Character>().charName;
        Debug.Log("s is "+s);
        if (s == null)
            return -1;

        if (fighters.ContainsKey(s))
            c = fighters[s].GetComponent<Character>();
        else
            return -1;

        temp = c.RemainingCycles();

        Debug.Log("Temp in setMin is "+temp);

        return temp;
    }

    public void StartRace()
    {
        Character c;
        //GameObject g;
        string s;
        
        int i, temp = 0;
        bool setInitial = false;
        
        Debug.Log("Minimum Cycles is "+minCycle);

        for(i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf)
            {
                c = transform.GetChild(i).gameObject.GetComponent<Character>();
                if (c != null && fighters.ContainsKey(c.charName))
                {
                    fighters.Remove(c.charName);
                }
                continue;
            }  
            else if (transform.GetChild(i).gameObject.CompareTag("Group"))
            {
                temp = BreakDown(transform.GetChild(i).gameObject,1);
            }
            else
            {
                temp = setMinCycle(transform.GetChild(i).gameObject);
                c = transform.GetChild(i).gameObject.GetComponent<Character>();

                if (!fighters.ContainsKey(c.charName))
                {
                    fighters.Add(c.charName,transform.GetChild(i).gameObject);
                }
            }

            if (temp == -1)
                continue;
            if (temp < minCycle || !setInitial)
            {
                minCycle = temp;
                s = transform.GetChild(i).gameObject.GetComponent<Character>().charName;
                //Debug.Log("Minimum Cycles is "+temp+ "from: "+s+" from iteration "+i);
                setInitial = true;
            }
            else
            {
                Debug.Log("Temp: "+temp+" > curMin: "+minCycle);
            }
    }

        //Debug.Log("Maximum Cycles is "+minCycle);

        foreach(KeyValuePair<string, GameObject> kvp in fighters)
        {
            // if (!kvp.Value.activeSelf)
            //     continue;
            
            c = kvp.Value.GetComponent<Character>();
            c.StartRace(minCycle);
        }

    }

    public void NextTurn()
    {
        float drain;
        int i;
        if (CheckIfDone())
        {
            i = CRTurnEnd.value;

            switch(i)
            {
                case 0:
                    activeFighter.GetComponent<Character>().CRChange(-100f);
                    break;
                case 1:
                    activeFighter.GetComponent<Character>().CRChange(-80f);
                    break;
                case 2:
                    activeFighter.GetComponent<Character>().CRChange(-60f);
                    break;

            }
            
            //activeFighter.GetComponent<Character>().CRChange(-100f);
            hasMaxCR = findMaxCR() - 1;
            if (hasMaxCR >= 0)
                SetFighter();
            else
            {
                statusText.text = "Cycling...";
                StartRace();
            }
        }
        // else if (CheckIfDone())
        // {
        //     i = CRTurnEnd.value;
        //     Debug.Log("CRTurnEnd value is "+i);

        //     switch(i)
        //     {
        //         case 0:
        //             activeFighter.GetComponent<Character>().CRChange(-100f);
        //             break;
        //         case 1:
        //             activeFighter.GetComponent<Character>().CRChange(-80f);
        //             break;
        //         case 2:
        //             activeFighter.GetComponent<Character>().CRChange(-60f);
        //             break;

        //     }

        //     statusText.text = "Cycling...";
        //     //activeFighter.GetComponent<Character>().CRChange(-100f);
        //     StartRace();
        // }
    }

    public void freezeTimeline()
    {
        /**
        for (i = 0; i < transform.childCount; i++)
        {
            c = fighters[i].GetComponent<Character>();
            tempfocus = c.CR;
            if (tempfocus > hiFocus)
            {
                member = i;
                hiFocus = tempfocus;
                c.CR = 0;
            }
        }
        **/
    }
}
