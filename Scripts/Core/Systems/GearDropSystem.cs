using UnityEngine;

public class GearDropSystem : MonoBehaviour
{
    public Gear[] PossibleDrops;  // Assign in inspector
    [Range(0f, 1f)]
    public float DropChance = 0.2f; // 20% chance per wave

    public Gear GetDroppedGear()
    {
        if (PossibleDrops.Length == 0) return null;

        float roll = Random.value; // 0 to 1
        if (roll <= DropChance)
        {
            int index = Random.Range(0, PossibleDrops.Length);
            Gear droppedGear = Instantiate(PossibleDrops[index]);
            Debug.Log($"Gear Dropped: {droppedGear.GearName} (Power {droppedGear.PowerBonus})");
            return droppedGear;
        }
        return null; // nothing dropped
    }
}
