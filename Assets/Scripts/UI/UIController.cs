using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private SettingMenu _settingMenu;
    private void Start()
    {
        _settingMenu.Close();
        EventManager.GameOverEvent += _settingMenu.OnGameOverHandler;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isShowing = _settingMenu.gameObject.activeSelf;
            _settingMenu.gameObject.SetActive(!isShowing);
            _settingMenu.PauseGame();
            if (isShowing)
            {
                _settingMenu.ContinueGame();
            }
            else
            {
            }
        }
    }
    public void OnOpenSettings()
    {
        _settingMenu.Open();
    }
}
