using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private const string ContinueRequestKey = "NOVA_CONTINUE_REQUEST";

    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject confirmNewGamePanel;

    [Header("Links")]
    [SerializeField] private string webUrl = "URL_AQUI";

    private void Awake()
    {
        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt(ContinueRequestKey, 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenNewGameConfirmation()
    {
        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(true);
        }
    }

    public void CancelNewGame()
    {
        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }
    }

    public void ConfirmNewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void CloseGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenWeb()
    {
        if (!string.IsNullOrWhiteSpace(webUrl))
        {
            Application.OpenURL(webUrl);
        }
    }
}