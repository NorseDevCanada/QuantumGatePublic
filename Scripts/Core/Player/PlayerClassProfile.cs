using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassProfile", menuName = "AFKIdle/ClassProfile")]
public class PlayerClassProfile : ScriptableObject
{
    public string ClassName;
    public Dictionary<StatType, float> StatWeights = new();
    
    public float GetWeight(StatType type)
    {
        return StatWeights.ContainsKey(type) ? StatWeights[type] : 0f;
    }

    public static PlayerClassProfile CreatePistolProfile()
    {
        var profile = ScriptableObject.CreateInstance<PlayerClassProfile>();
        profile.ClassName = "Pistol";
        profile.StatWeights = new Dictionary<StatType, float>
        {
            { StatType.ComboMultiplier, 1.0f },
            { StatType.CritRate, 0.8f },
            { StatType.ATKSPD, 0.5f },
            { StatType.Evasion, 0.3f }
        };
        return profile;
    }
}
