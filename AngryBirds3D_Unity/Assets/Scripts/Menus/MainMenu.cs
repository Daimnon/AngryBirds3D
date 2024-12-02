using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour, ISaveable
{
    [SerializeField] private ASyncLoader _loader;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private TextMeshProUGUI _lastScoreTMP;

    public void MoveToLevel()
    {

    }
    public void Quit()
    {
        Application.Quit();
    }

    public void LoadData(GameData gameData)
    {
        _lastScoreTMP.text = "Last Score:      " + gameData.Score.ToString();
    }
    public void SaveData(ref GameData gameData)
    {
        
    }
}
