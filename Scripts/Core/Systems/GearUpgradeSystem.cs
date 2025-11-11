using UnityEngine;

public class GearUpgradeSystem : MonoBehaviour
{
    public void UpgradeGear(Gear gear)
    {
        if (gear == null) return;

        gear.Level++;
        gear.PowerBonus += Mathf.RoundToInt(gear.PowerBonus * 0.5f); // Increase by 50%
        Debug.Log($"{gear.GearName} upgraded to Level {gear.Level}, new Power: {gear.PowerBonus}");
    }
}
