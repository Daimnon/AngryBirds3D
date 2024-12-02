using UnityEngine;

public class MainMenu : MonoBehaviour, ISaveable
{
    [SerializeField] private ASyncLoader _loader;
    [SerializeField] private AudioSource _audioSource;
    private int _lastScore = 0;

    public void MoveToLevel()
    {

    }
    public void Quit()
    {
        Application.Quit();
    }

    public void LoadData(GameData gameData)
    {
        _lastScore = gameData.Score;
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.Score = _lastScore;
    }
}
