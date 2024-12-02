[System.Serializable]
public class GameData
{
    // settings
    public bool IsSoundOn;

    // account
    public int Currency;

    // quests
    public SerializableDictionary<int, bool> QuestsCompleted;
}
