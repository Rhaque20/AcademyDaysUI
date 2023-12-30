using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public EnumLibrary.DamagePower powerType;
    public EnumLibrary.Element attribute;
    public EnumLibrary.ATKScale atkScale;
    public EnumLibrary.DEFScale defScale;
    public float skillPower = 1f;
    public bool isMultiHit = false;
    public float comboDMGRate = 0f;
    public int comboCount = 0;
}
