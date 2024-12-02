[System.Serializable]
public class GameData
{
    // settings
    public bool IsSoundOn;

    // account
    public int Score;

    // quests
    public SerializableDictionary<int, bool> QuestsCompleted;

    public GameData()
    {
        IsSoundOn = true;
        Score = 0;
        QuestsCompleted = new SerializableDictionary<int, bool>();
    }
}
