using UnityEngine;

public class ScoreManager : MonoBehaviour, ISaveable
{
    private static ScoreManager _instance = null;
    public static ScoreManager Instance => _instance;

    private int _score = 0;
    public int Score => _score;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;
    }

    public void AddScore(int score)
    {
        _score += score;
    }

    public void LoadData(GameData gameData)
    {
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.Score = _score;
    }
}
