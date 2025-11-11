[System.Serializable]
public class Companion
{
    public string Name;
    public int Power;
    public int Level = 1;

    public int GetTotalPower() => Power * Level;
}
