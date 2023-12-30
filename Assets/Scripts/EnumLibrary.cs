using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnumLibrary
{
    public enum Element{Fire,Wind,Earth,Water,Light,Dark,Pure};
    public enum DamagePower{Slash,Impact,Pierce,Missile,Magic,NonType};
    public enum Race{Empty,Hyoman,Beast,Aquatic,Undead,Fairy,Plant,Demon,Dragon,Golem,Bug};
    public enum Stats{HP,PATK,PDEF,MATK,MDEF,TEC,AGI,CRIT,LUCK,MOV,JUMP};
    public enum OtherStats{HITUP,EVAUP,CRITUP,CastRateRed,GutsChance,HPCostDown}
    public enum ATKScale{Flat,NormalWarrior,NormalMage,HeavyWarrior,HeavyMage,DEFScale,NimbleWarrior,NimbleThief,Archer,Gunner};
    public enum DEFScale{None,Physical,Magical};
}
