using UnityEngine;

public class IdleManager : MonoBehaviour
{
    public float tickInterval = 2f;
    private float tickTimer = 0f;

    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            CombatManager.Instance?.AutoResolveWave();
        }
    }
}
