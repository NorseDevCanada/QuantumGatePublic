using UnityEngine;

public class GearDropSystem : MonoBehaviour
{
    public Gear[] PossibleDrops;
    [Range(0f, 1f)]
    public float DropChance = 0.2f;

    public Gear GetDroppedGear()
    {
        if (PossibleDrops.Length == 0) return null;

        float roll = Random.value;
        if (roll <= DropChance)
        {
            int index = Random.Range(0, PossibleDrops.Length);
            Gear droppedGear = Instantiate(PossibleDrops[index]);
            Debug.Log($"Gear Dropped: {droppedGear.GearName} (Power {droppedGear.PowerBonus})");
            return droppedGear;
        }
        return null;
    }
}
