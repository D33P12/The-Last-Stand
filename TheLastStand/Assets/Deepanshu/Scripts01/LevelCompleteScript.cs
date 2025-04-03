using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteScript : MonoBehaviour
{
    public static LevelCompleteScript Instance { get; private set; }
    private Inputs _controls;
    public GameObject levelCompleteCanvas;
    public Button nextLevelButton;
    public Button restartButton;
    private ShootiController _shootiController;
    private PlayerController _playerController;
    private MenuNavigation _menuNavigation;
    public LevelSettings levelSettings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _controls = new Inputs();
        levelCompleteCanvas.SetActive(false);
        _shootiController = FindObjectOfType<ShootiController>();
        _playerController = FindObjectOfType<PlayerController>();
        _menuNavigation = FindObjectOfType<MenuNavigation>();

        nextLevelButton.onClick.AddListener(LoadNextLevelWithIncreasedDifficulty);
        restartButton.onClick.AddListener(RestartGameWithDefaultSettings);
    }
    private void Start()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnLevelComplete += ActivateLevelComplete;
        }
    }
    private void OnEnable()
    {
        _controls.PlayerMovement.Enable();
        _controls.PlayerUI.Disable();
    }
    private void OnDisable()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnLevelComplete -= ActivateLevelComplete;
        }
        _controls.PlayerMovement.Disable();
        _controls.PlayerUI.Disable();
    }
    private void ActivateLevelComplete()
    {
        Time.timeScale = 0;
        _playerController.IsPaused = true;
        _shootiController.SetCanShoot(false);
        EnablePlayerUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        levelCompleteCanvas.SetActive(true);

        _menuNavigation.ActivateLevelCompleteMenu();
    }
    public void MainMenu()
    {
        levelSettings.ResetToDefault();
        SceneManager.LoadScene(0);
    }
    public void LoadNextLevelWithIncreasedDifficulty()
    {
        levelSettings.IncreaseDifficulty();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void RestartGameWithDefaultSettings()
    {
        levelSettings.ResetToDefault();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void EnablePlayerMovement()
    {
        _controls.PlayerMovement.Enable();
        _controls.PlayerUI.Disable();
    }
    public void EnablePlayerUI()
    {
        _controls.PlayerMovement.Disable();
        _controls.PlayerUI.Enable();
    }
    public void OnQuitGame()
    {
        Application.Quit();
    }
}
