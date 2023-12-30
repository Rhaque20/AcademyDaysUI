using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TurnOrderManager : MonoBehaviour
{
    [SerializeField]Player _testEntity = null;
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
            Player temporary = null;
            //e.getFighterData.GenerateActionValue();

            if (e.getFighterData.unitType == Player.UnitType.Enemy)
            {
                temporary = new();

                temporary.DupePlayer(e.getFighterData);
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
            e.getFighterData.FocusPush(Random.Range(0f,5f));
            e.getFighterData.MinCyclesNeeded();
            _combatants.Add(e);

            //temp.GetComponent<ToggleCharacter>().heldEntity = e;
            //temp.GetComponent<ToggleCharacter>().toggleType = 1;
            temp.transform.SetParent(_turnList);
            temp.GetComponent<Transform>().localScale = new Vector3(1f,1f,1f);

            // //Created spell units
            temp = Instantiate(g);
            if(e.getFighterData.unitType == Player.UnitType.Player)
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

        if(_combatants[n].isSpellUnit)
        {
            _spellUnits[_combatants[n].entityOrigin.getFighterData.charName].SetActive(false);
            _combatants.RemoveAt(n);
            n = _combatants.Count - 1;

        }
        else
        {
            _combatants[n].getFighterData.FocusPush(float.Parse(_turnEnd.options[_turnEnd.value].text));
        }
            

        List<int> cycleSort = new List<int>();
        
        for(int i = 0; i < _combatants.Count; i++)
        {
            //_combatants[i].getFighterData.DecreaseActionValue(_combatants[0].getFighterData.actionValue);
            // _combatants[i].getFighterData.AddFocus(_combatants[0].getFighterData.cyclesNeeded);
            cycleSort.Add(_combatants[i].getFighterData.MinCyclesNeeded());
        }

        cycleSort.Sort();

        for(int i = 0; i < _combatants.Count; i++)
        {
            _combatants[i].getFighterData.AddFocus(cycleSort[0]);
        }

        _combatants.Sort();

        if(!_combatants[n].isSpellUnit)
        {
            //_combatants[0].getFighterData.GenerateActionValue();
            BattleManager.instance.DecrementBuff(0,_combatants[n]);
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
            if(j != _combatants.Count - 1)
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