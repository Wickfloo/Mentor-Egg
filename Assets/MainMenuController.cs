using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Exact name of your gameplay scene")]
    [SerializeField] private string gameSceneName = "World1";

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject worldsPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    // 1. Play Button - Loads World1 by name
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // 2. Worlds Button
    public void OpenWorlds()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (worldsPanel != null) worldsPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // 3. Settings Button
    public void OpenSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (worldsPanel != null) worldsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // Helper: Back Button
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (worldsPanel != null) worldsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // 4. Quit Button
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}