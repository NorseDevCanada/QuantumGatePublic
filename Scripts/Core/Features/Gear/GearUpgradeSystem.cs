using UnityEngine;

public class GearUpgradeSystem : MonoBehaviour
{
    public void UpgradeGear(Gear gear)
    {
        if (gear == null) return;

        gear.GearLevel++;
        gear.PowerBonus += Mathf.RoundToInt(gear.PowerBonus * 0.5f);
        Debug.Log($"{gear.GearName} upgraded to Level {gear.GearLevel}, new Power: {gear.PowerBonus}");
    }
}
