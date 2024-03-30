using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class TurnOrderManager : MonoBehaviour
{
    [SerializeField]Fighter _testEntity = null;
    [SerializeField]Transform _turnList;
    List<Entity> _combatants = new List<Entity>();
    [SerializeField]private TMP_InputField _actionPush,_spellName,_spellSpeed;
    [SerializeField]private TMP_Text _statusText;
    [SerializeField]private SelectionModal _fighterSelection;

    [SerializeField]private TMP_Dropdown _turnEnd;

    private Dictionary<string,GameObject> _spellUnits = new();

    private Dictionary<string,int> _dupeEnemies = new();

    public static TurnOrderManager instance;

    void Awake()
    {
        instance = this;
    }

    public List<Entity> combatants
    {
        get{return _combatants;}
    }

    void Start()
    {
        Debug.Log("Starting TurnOrderManager");
        foreach(GameObject g in BattleManager.instance.fighterList)
        {
            GameObject temp = Instantiate(g);
            Entity e = temp.GetComponent<Entity>();
            Fighter temporary = null;
            //e.getFighterData.GenerateActionValue();

            if (e.getFighterData.unitType == Fighter.UnitType.Enemy)
            {
                temporary = new();

                temporary.DupeFighter(e.getFighterData);
                temporary.GenerateActionValue();
                
                if (_dupeEnemies.ContainsKey(e.getFighterData.charName))
                {
                    _dupeEnemies[e.getFighterData.charName]++;
                    temporary.charName += " "+(char)(_dupeEnemies[e.getFighterData.charName]+65 - 1);
                }
                else
                {
                    _dupeEnemies.Add(e.getFighterData.charName, 1);
                   temporary.charName += " "+(char)(65);
                }
                Debug.Log("temporary's name: "+temporary.charName+" and speed is "+temporary.speed);
                temporary.portrait = e.getFighterData.portrait;
                if (_testEntity == null)
                    _testEntity = temporary;
                e.SetUnit(temporary);
                temp.name = temporary.charName;
            }
            else
            {
                temp.name = e.getFighterData.charName;
            }

            e.StartEntity();
            e.getFighterData.FocusPush(UnityEngine.Random.Range(0f,5f));
            e.getFighterData.MinCyclesNeeded();
            _combatants.Add(e);

            //temp.GetComponent<ToggleCharacter>().heldEntity = e;
            //temp.GetComponent<ToggleCharacter>().toggleType = 1;
            temp.transform.SetParent(_turnList);
            temp.GetComponent<Transform>().localScale = new Vector3(1f,1f,1f);

            // //Created spell units
            temp = Instantiate(g);
            if(e.getFighterData.unitType == Fighter.UnitType.Player)
            {
                temp.name = e.getFighterData.charName+" Spell Unit";
                
            }
            else if (temporary != null)
            {
                temp.name = temporary.charName+" Spell Unit";
            }
            temp.transform.SetParent(_turnList);
            temp.GetComponent<Transform>().localScale = new Vector3(1f,1f,1f);

            // New entities aren't being made here. Fix it
            Entity spellEntity = temp.GetComponent<Entity>();
            spellEntity.CreateSpellUnit(e,"Blank",100f);
            if (spellEntity == null)
            {
                Debug.Log("Spell entity was not intialized");
            }
            temp.GetComponent<ToggleCharacter>().SetUpSpellUnit(spellEntity);
            if (temp.GetComponent<ToggleCharacter>().heldEntity == null)
            {
                Debug.Log("The entity wasn't held in toggle");
            }
            temp.SetActive(false);
            Debug.Log("Creating spell unit for "+e.name);
            _spellUnits.Add(e.name,temp);
        }

        BattleManager.instance.StartSetUp(_combatants);
        _combatants.Sort();
        GenerateTurnList();
    }

    public int ReturnMinimumCycles()
    {
        List<int> cycleSort = new List<int>();
        
        for(int i = 0; i < _combatants.Count; i++)
        {
            cycleSort.Add(_combatants[i].getFighterData.MinCyclesNeeded());
        }

        cycleSort.Sort();
        return cycleSort[0];
    }

    public List<KeyValuePair<Entity,float>> ReturnTurnOrderList(float focusDown)
    {
        Dictionary<Entity,int> cyclesGroup = new();
        List<KeyValuePair<Entity,int>> cyclesList;
        Fighter p;
        Entity activeEntity = _combatants[0];
        for(int i = 0; i < _combatants.Count; i++)
        {
            p = _combatants[i].getFighterData;
            if (i == _combatants.Count - 1)
            {
                Debug.Log("Decremented "+_combatants[i].getFighterData.charName);
                cyclesGroup.Add(_combatants[i],p.MinCyclesNeeded(focusDown));
                activeEntity = _combatants[i];
            }
            else
                cyclesGroup.Add(_combatants[i],p.MinCyclesNeeded());
        }

       

        cyclesList = cyclesGroup.ToList();
        cyclesList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));

        List<KeyValuePair<Entity,float>> finalList = new();

        float focus;

        foreach(KeyValuePair<Entity,int> kvp in cyclesList)
        {
            if (kvp.Key == activeEntity)
            {
                focus = kvp.Key.getFighterData.GetTempFocus(cyclesList[0].Value,focusDown);
            }
            else
            {
                focus = kvp.Key.getFighterData.GetTempFocus(cyclesList[0].Value);
            }

            finalList.Add(new KeyValuePair<Entity, float>(kvp.Key,focus));
        }

        finalList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));


        return finalList;
    }

    public string ForecastStringHelper(float focusDown)
    {
        Dictionary<Entity,int> cyclesGroup = new();
        List<KeyValuePair<Entity,float>> cyclesList = ReturnTurnOrderList(focusDown);

        string finalString = new string("");

        for(int i = cyclesList.Count - 1; i >= 0; i--)
        {
            finalString += cyclesList[i].Key.getFighterData.charName + "("+Mathf.Round(cyclesList[i].Value)+")";
            if (i != 0)
            {
                finalString +="->";
            }
        }

        return finalString;
    }

    public void GenerateForecastTurnString()
    {
        Debug.Log("On -60 Focus: "+ForecastStringHelper(60));
        Debug.Log("On -80 Focus: "+ForecastStringHelper(80));
        Debug.Log("On -100 Focus: "+ForecastStringHelper(100));
    }

    public void CastSpell()
    {
        List<ToggleCharacter> caster = _fighterSelection.ReturnSelected();

        if(caster.Count == 0)
        {
            _statusText.SetText("Please Select a Caster.");
            return;
        }

        if(_spellName.text.CompareTo("") == 0)
        {
            _statusText.SetText("Insert a spell name.");
            return;
        }

        if(_spellSpeed.text.CompareTo("") == 0)
        {
            _statusText.SetText("Set a spell speed.");
            return;
        }
            

        if(caster.Count > 1)
        {
            _statusText.SetText("Select ONE caster");
            return;
        }
        if(caster[0].heldEntity.isSpellUnit)
        {
            _statusText.SetText("Select a caster, not a spell unit "+caster[0].heldEntity.name);
            return;
        }
        _statusText.SetText("Casting "+_spellName.text+" from caster:"+caster[0].heldEntity.getFighterData.charName);
        _spellUnits[caster[0].name].SetActive(true);
        if(_spellUnits.ContainsKey(caster[0].name))
        {
            Debug.Log("It does have exist but held entity is "+ _spellUnits[caster[0].name].GetComponent<ToggleCharacter>().name);
        }
        Entity spellUnit = _spellUnits[caster[0].name].GetComponent<ToggleCharacter>().heldEntity;
        if (spellUnit == null)
            Debug.Log("No spell unit initialized for "+caster[0].name);
        
        spellUnit.EditSpellUnit(caster[0].heldEntity.getFighterData,_spellName.text,float.Parse(_spellSpeed.text));
        _spellUnits[caster[0].name].GetComponent<ToggleCharacter>().SetUpSpellUnit(spellUnit);
        _combatants.Add(spellUnit);
        Debug.Log("Spell unit is "+spellUnit.name);
        _combatants.Sort();
        AdvanceTurn();

    }

    public void AdvanceTurn()
    {
        int n = _combatants.Count - 1;
        float focusPush = float.Parse(_turnEnd.options[_turnEnd.value].text);

        if(_combatants[n].isSpellUnit)
        {
            _spellUnits[_combatants[n].entityOrigin.getFighterData.charName].SetActive(false);
            _combatants.RemoveAt(n);
            n = _combatants.Count - 1;

        }
        else
        {
            _combatants[n].getFighterData.FocusPush(focusPush);
            BattleManager.instance.DecrementBuff(0,_combatants[n]);
        }
            
        int minCycles = ReturnMinimumCycles();

        for(int i = 0; i < _combatants.Count; i++)
        {
            _combatants[i].getFighterData.AddFocus(minCycles);
        }

        _combatants.Sort();

        if(!_combatants[n].isSpellUnit)
        {
            //_combatants[0].getFighterData.GenerateActionValue();
            
        }
        
        GenerateTurnList();
    }

    public void GenerateTurnList()
    {

        string turnOrderString = new string("");
        int j = 0;
        Transform temp;

        // for(int i = 0; i < _turnList.childCount; i++)
        // {
        //     if(_turnList.GetChild(i).gameObject.activeSelf)
        //     {
        //         //_turnList.GetChild(i).GetComponent<ToggleCharacter>().Initialize(_combatants[j]);
        //         temp = _turnList.Find(_combatants[j].gameObject.name);
        //         if(temp != null)
        //         {
        //             Debug.Log("Found "+_combatants[j].gameObject.name);
        //             temp.SetSiblingIndex(j);
        //         }
        //         //_turnList.GetChild(i).SetSiblingIndex(j);
        //         turnOrderString += _combatants[j].getFighterData.charName +"("+_combatants[j].actionValue+")";
        //         if(j != _combatants.Count - 1)
        //         {
        //             turnOrderString +="->";
        //         }
        //         else
        //             break;
        //         j++;
        //     }
            
        // }

        for(j = _combatants.Count - 1; j >= 0; j--)
        {
            Debug.Log("Trying to find "+_combatants[j]);
            temp = _turnList.Find(_combatants[j].gameObject.name);
            if(temp != null)
            {
                Debug.Log("Found "+_combatants[j].gameObject.name);
                temp.SetSiblingIndex(_combatants.Count - 1 - j);
                temp.GetComponent<ToggleCharacter>().Initialize(_combatants[j]);
            }
            //_turnList.GetChild(i).SetSiblingIndex(j);
            turnOrderString += _combatants[j].getFighterData.charName +"("+_combatants[j].getFighterData.cyclesNeeded+")";
            if(j != 0)
            {
                turnOrderString +="->";
            }
        }

        //Debug.Log("At the end, Name is currently "+_turnList.GetChild(_turnList.childCount - 1).GetChild(0).GetComponent<TMP_Text>().text);

        Debug.Log(turnOrderString);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && _actionPush.text.CompareTo("") != 0) 
        { 
            float pushVal = float.Parse(_actionPush.text);
            _fighterSelection.ApplyAVShift(pushVal);
            GenerateTurnList();
        }
    }
}