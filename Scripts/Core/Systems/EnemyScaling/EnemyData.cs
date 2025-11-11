using UnityEngine;

public enum EnemyType
{
    Normal,
    Elite,
    Boss
}

[System.Serializable]
public class EnemyData
{
    [Header("Dynamic Stats")]
    public int StageIndex;
    public EnemyType Type;
    public int Power;
    public int Health;
    public int XPReward;
    public int CreditReward;

    public string EnemyName;

    public EnemyData(int stage, EnemyType type, int playerCombatPower)
    {
        StageIndex = stage;
        Type = type;

        // multipliers by type (LoM style)
        float typeMult = type switch
        {
            EnemyType.Elite => 2.5f,
            EnemyType.Boss => 6.0f,
            _ => 1.0f
        };

        // stage scaling curve (superlinear, idle-like)
        float stageScale = Mathf.Pow(1.05f, stage);

        // base power scales from player combat power but weighted by stage
        Power = Mathf.RoundToInt(playerCombatPower * 0.75f * stageScale * typeMult);
        Health = Mathf.RoundToInt(100 * Mathf.Pow(stage, 1.25f) * typeMult);

        // rewards
        XPReward = Mathf.RoundToInt(Power * 0.15f);
        CreditReward = Mathf.RoundToInt(Power * 1.25f);

        EnemyName = $"{type} (Stage {stage})";
    }

    public override string ToString()
    {
        return $"{EnemyName} - Power:{Power} HP:{Health} XP:{XPReward} Cr:{CreditReward}";
    }
}
