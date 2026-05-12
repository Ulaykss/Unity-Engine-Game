using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ScenesManager : MonoBehaviour
{
    public GameObject pausePanel, settingsPanel, mainMenuPanel, abilitiesPanel;

    void Start()
    {
        if (mainMenuPanel != null) { mainMenuPanel.SetActive(true);}
        if (abilitiesPanel != null) { abilitiesPanel.SetActive(false);}
        if (pausePanel != null) { pausePanel.SetActive(false);}
        settingsPanel.SetActive(false);
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SettingsMenu()
    {
        if (mainMenuPanel != null) { mainMenuPanel.SetActive(false);}
        if (pausePanel != null) { pausePanel.SetActive(false);}
        settingsPanel.SetActive(true);
    }

    public void AbilitiesMenu()
    {
        mainMenuPanel.SetActive(false);
        abilitiesPanel.SetActive(true);
    }

    public void CloseAbilitiesMenu()
    {
        mainMenuPanel.SetActive(true);
        abilitiesPanel.SetActive(false);
    }

    public void CloseSettingMenu()
    {
        if (mainMenuPanel != null) { mainMenuPanel.SetActive(true);}
        if (pausePanel != null) { pausePanel.SetActive(true);}
        settingsPanel.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Exit executed");
    }
}
