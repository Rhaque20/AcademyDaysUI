using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{

    public static BattleManager instance;
    public Sprite[] elementIcons = new Sprite[6];
    public Sprite[] powerIcons = new Sprite[6];
    public Sprite[] statIcons = new Sprite[11];
    public TMP_Dropdown dropdown;
    public TMP_InputField damageInput;

    private Fighter attacker,defender;
    private Entity _activeEntity;
    public List<GameObject> fighterList = new List<GameObject>();

    public GameObject attackerUI = null,defenderUI = null, toggleCharPrefab;
    [SerializeField]private GameObject _damageModal;
    [SerializeField]private BuffDebuffModal _buffDebuffModal;
    public Transform listContent,effectApplyToggleList,damageModalToggleList,statusList;
    public TMP_Text display;

    public bool selectingAttacker = false;

    [SerializeField]int baseDamage = 0;

    private void Awake()
    {
        instance = this;
    }

    public Fighter ReturnFighter()
    {
        return attacker;
    }

    public Entity activeEntity
    {
        get{return _activeEntity;}
    }

    // public GameObject Entity(int i)
    // {
    //     if (i == 0)
    //     {
    //         return listContent.GetChild(attackerIndex).gameObject;
    //     }

    //     return listContent.GetChild(defenderIndex).gameObject;
    // }

    public void SelectingAttacker()
    {
        Debug.Log("Selecting attacker is "+selectingAttacker);
        selectingAttacker = !selectingAttacker;
        attackerUI.transform.GetChild(0).gameObject.SetActive(selectingAttacker);
    }

    public void StartSetUp(List<Entity> combatants)
    {
        Debug.Log("Starting battlemanager");
        GameObject g,activeTemp;
        ToggleCharacter toggleChar;
        int i = 0;
        foreach(GameObject allies in fighterList)
        {
            // g = Instantiate(allies);
            // g.transform.SetParent(listContent);
            // g.GetComponent<Transform>().localScale = new Vector3(1f,1f,1f);
            // activeTemp = listContent.GetChild(listContent.childCount - 1).gameObject;
            g = Instantiate(toggleCharPrefab);
            toggleChar = g.GetComponent<ToggleCharacter>();
            toggleChar.Initialize(combatants[i],ToggleCharacter.ToggleType.Character);
            g.transform.SetParent(effectApplyToggleList);
            g.GetComponent<Transform>().localScale = new Vector3(1f,1f,1f);

            g = Instantiate(toggleCharPrefab);
            toggleChar = g.GetComponent<ToggleCharacter>();
            toggleChar.Initialize(combatants[i],ToggleCharacter.ToggleType.DamageCalc);
            g.transform.SetParent(damageModalToggleList);
            g.GetComponent<Transform>().localScale = new Vector3(1f,1f,1f);

            g = Instantiate(toggleCharPrefab);
            toggleChar = g.GetComponent<ToggleCharacter>();
            toggleChar.Initialize(combatants[i],ToggleCharacter.ToggleType.Character);
            g.transform.SetParent(statusList);
            g.GetComponent<Transform>().localScale = new Vector3(1f,1f,1f);

            //activeTemp.GetComponent<Entity>().index = i;
            i++;
        }

        attacker = combatants[0].getFighterData;
        defender = combatants[1].getFighterData;
        SetFighter(combatants[0]);
        _damageModal.GetComponent<DamageModal>().Initialize(combatants);
        _damageModal.GetComponent<DamageModal>().SetAttackerProfile(combatants[0].getFighterData);
        _damageModal.SetActive(false);
    }

    public void SetFighter(Entity SelectedEntity)
    {
        GameObject uiElement;
        Fighter fighter = SelectedEntity.getFighterData;
        _activeEntity = SelectedEntity;
        if (dropdown.value == 0)
        {
            uiElement = attackerUI;
            attacker = fighter;
        }
        else
        {
            uiElement = defenderUI;
            defender = fighter;
        }

        uiElement.transform.GetChild(2).GetComponent<Image>().sprite = fighter.portrait;
        Debug.Log("Name was "+uiElement.transform.GetChild(3).GetComponent<TMP_Text>().text);
        uiElement.transform.GetChild(3).GetComponent<TMP_Text>().text = fighter.charName;
        uiElement.transform.GetChild(4).GetComponent<Image>().sprite = elementIcons[(int)fighter.attribute];

        _damageModal.GetComponent<DamageModal>().SetAttackerProfile(fighter);

        
    }

    // public void DecrementBuff(int i)
    // {
    //     bool onUse = false, defense = false;
    //     Debug.Log("Calling Decrement");

    //     switch (i)
    //     {
    //         //Decrement standard buffs that go down by turns
    //         case 1:
    //             onUse = false;
    //             defense = false;
    //             break;
    //         //Decrement buffs that go down on attack
    //         case 2:
    //             onUse = true;
    //             defense = false;
    //             break;
    //         case 3:
    //         // Decrement buffs that go down on being attacked
    //             onUse = true;
    //             defense = true;
    //             break;

    //     }
        
    //     Entity(dropdown.value).GetComponent<Entity>().DecrementBuff(onUse,defense);
    // }

    public void SwapCharacter()
    {
        if (_activeEntity != null)
        {
            _activeEntity.SwitchFighter();
            TurnOrderManager.instance.AdvanceTurn();
        }
    }

    public void TurnOnDamageModal(bool modal)
    {
        _damageModal.SetActive(modal);
    }

    public void TurnOnBuffDebuffModal()
    {
        _buffDebuffModal.OpenModal(_activeEntity);
    }

    public void DecrementBuff(int i, Entity e)
    {
        bool onUse = false, defense = false;
        Debug.Log("Calling Decrement");

        switch (i)
        {
            //Decrement standard buffs that go down by turns
            case 1:
                onUse = false;
                defense = false;
                break;
            //Decrement buffs that go down on attack
            case 2:
                onUse = true;
                defense = false;
                break;
            case 3:
            // Decrement buffs that go down on being attacked
                onUse = true;
                defense = true;
                break;

        }

        e.DecrementBuff(onUse,defense);
    }
}
