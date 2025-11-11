using UnityEngine;

public static class XPProgression
{
    public const int MAX_LEVEL = 300;
    private const float GrowthFactor = 10.5f;

    public static int GetXPForLevel(int level)
    {
        if (level < 1) level = 1;

        float raw = level + 300f * Mathf.Pow(2f, level / GrowthFactor);
        int xp = Mathf.FloorToInt(raw / 4f);

        if (level >= 160)
            xp = Mathf.FloorToInt(xp * 1.5f);
        else if (level >= 120)
            xp = Mathf.FloorToInt(xp * 1.25f);

        return xp;
    }

    public static int GetTotalXPToReach(int level)
    {
        int total = 0;
        for (int i = 1; i < level; i++)
            total += GetXPForLevel(i);
        return total;
    }
}
