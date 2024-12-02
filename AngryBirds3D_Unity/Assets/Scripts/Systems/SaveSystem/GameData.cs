[System.Serializable]
public class GameData
{
    // settings
    public bool IsSoundOn { get; set; }

    // account
    public int Score { get; set; }

    // quests
    public SerializableDictionary<int, bool> QuestsCompleted { get; set; }
}
