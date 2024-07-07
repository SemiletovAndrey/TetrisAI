using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _textPause;
    [SerializeField] private TMP_Text _textGameOver;
    [SerializeField] private Slider _sliderMusic;
    private bool _isGameOver = false;

    [SerializeField] private bool canAI;

    private void Start()
    {
        if (canAI ==false)
        {
            _sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume");
        }
    }
    public void Open()
    {
        Debug.Log(gameObject.name);
        gameObject.SetActive(true);
        PauseGame();
    }
    public void Close()
    {
        gameObject.SetActive(false);
        ContinueGame();
    }
    public void OnSoundValue(float volume)
    {
        Managers.AudioManager.MusicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (canAI == false)
        {
            if (!_isGameOver)
            {
                _textPause.gameObject.SetActive(true);
                _textGameOver.gameObject.SetActive(false);
            }
            else
            {
                _textPause.gameObject.SetActive(false);
                _textGameOver.gameObject.SetActive(true);
            }
        }
    }
    public void ContinueGame()
    {
        Time.timeScale = 1f;
        _textPause.gameObject.SetActive(false);
    }

    public void Reload()
    {
        EventManager.ClearGameOverEvent();
        EventManager.ClearScoringEvent();
        SceneManager.LoadScene("TetrisGame");
    }
    public void ExitMainMenu()
    {
        SceneManager.LoadScene("FirstScene");
    }
    
    public void Exit()
    {
        Application.Quit();
    }

    public void OnGameOverHandler()
    {
        _isGameOver = true;
        Open();
        Debug.Log("GameOver");
    }
}
