using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum LevelType
{
    TitleScreen,
    MainMenu,
    LevelOne,
    LevelTwo,
}

public class ASyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private TextMeshProUGUI _precentageText;
    [SerializeField] private GameObject _objectToDeactivate;
    [SerializeField] private bool _isTitleScreen = false;
    /* For testings only!
    [SerializeField][Range(0.0f, 1.0f)] private float _testValue;
    private void Update()
    {
        UpdateProgress(_testValue);
    }*/

    private void Start()
    {
        if (_isTitleScreen) LoadLevel(LevelType.MainMenu);
    }

    private IEnumerator LoadLevelASync(int levelType)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelType);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            UpdateProgress(progressValue);
            yield return null;
        }
    }
    private void UpdateProgress(float progressValue)
    {
        int percentage = Mathf.RoundToInt(progressValue * 100);
        _precentageText.text = percentage.ToString() + "%";
    }

    /// <summary>
    /// <para>'levelType' behind the scenes is 'LevelType' enum.
    /// <br>0 = mainMenu, 1 = levelOne, 2 = levelTwo.</br></para>
    /// </summary>
    /// <param name="levelType"></param>
    public void LoadLevel(int levelType)
    {
        _loadingScreen.SetActive(true);
        _objectToDeactivate.SetActive(false);

        StartCoroutine(LoadLevelASync(levelType));
    }

    /// <summary>
    /// <para><br>0 = mainMenu, 1 = levelOne, 2 = levelTwo.</br></para>
    /// </summary>
    /// <param name="levelType"></param>
    public void LoadLevel(LevelType levelType)
    {
        _loadingScreen.SetActive(true);
        _objectToDeactivate.SetActive(false);

        StartCoroutine(LoadLevelASync((int)levelType));
    }
}