using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    // Core attributes
    public float HP;
    public float ATK;
    public float DEF;
    public float ATKSPD;

    // Dictionary for advanced & special stats
    public Dictionary<StatType, float> ExtraStats = new();

    public float PowerRating => 
        (ATK * 0.6f) + (DEF * 0.25f) + (HP * 0.15f);
}
