using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment")]
public class Equipment : ScriptableObject
{
    
    public EnumLibrary.Stats[] stats = new EnumLibrary.Stats[1];
    public float[] statValues = new float[1];
}