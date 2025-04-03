using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteScript : MonoBehaviour
{
    public static LevelCompleteScript Instance { get; private set; }
    private Inputs _controls;
    public GameObject levelCompleteCanvas;
    private ShootiController _shootiController;
    private PlayerController _playerController;
    private MenuNavigation _menuNavigation;

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
    public void RestartGame()
    {
        Time.timeScale = 1;
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
}
