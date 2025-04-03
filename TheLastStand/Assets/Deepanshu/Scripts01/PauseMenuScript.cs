using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{ 
    private Inputs _controls;
    public GameObject pauseMenuCanvas;
    public Slider aimSensitivitySlider;
    public Slider musicVolumeSlider;
    private ShootiController _shootiController;
    private PlayerController _playerController;
    public GameSettings gameSettings;
    public MusicSettings musicSettings;

    private const string AimSensitivityKey = "AimSensitivity";
    private const string MusicVolumeKey = "MusicVolume";

    private MenuNavigation _menuNavigation;
    public LevelSettings levelSettings;
    private void Awake()
    {
        _controls = new Inputs();
        pauseMenuCanvas.SetActive(false);
        _shootiController = FindObjectOfType<ShootiController>();
        _playerController = FindObjectOfType<PlayerController>();
        _menuNavigation = FindObjectOfType<MenuNavigation>();
    }
    private void Start()
    {
        _controls.PlayerUI.Disable();
        LoadDefaultSettings();

        if (aimSensitivitySlider != null)
        {
            aimSensitivitySlider.value = gameSettings.aimSensitivity;
            aimSensitivitySlider.onValueChanged.AddListener(OnAimSensitivityChanged);
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicSettings.musicVolume;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayAudioContinuous(SoundManager.AudioType.Bg, musicSettings.musicVolume);
        }
    }
    private void OnEnable()
    {
        _controls.PlayerMovement.Enable();
        _controls.PlayerUI.Disable();
        _controls.PlayerMovement.OpenMenu.performed += OnOpenMenu;
        _controls.PlayerUI.CloseMenu.performed += OnCloseMenu;
    }
    private void OnDisable()
    {
        _controls.PlayerMovement.OpenMenu.performed -= OnOpenMenu;
        _controls.PlayerUI.CloseMenu.performed -= OnCloseMenu;
        _controls.PlayerMovement.Disable();
        _controls.PlayerUI.Disable();
    }
    private void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (pauseMenuCanvas.activeSelf || GameOverScript.Instance.gameOverCanvas.activeSelf || LevelCompleteScript.Instance.levelCompleteCanvas.activeSelf)
        {
            return;
        }

        if (pauseMenuCanvas.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    private void OnCloseMenu(InputAction.CallbackContext context) => ResumeGame();
    private void OnAimSensitivityChanged(float value)
    {
        gameSettings.aimSensitivity = value;
        PlayerPrefs.SetFloat(AimSensitivityKey, value);
        ApplyAimSensitivity(value);
    }
    private void OnMusicVolumeChanged(float value)
    {
        musicSettings.musicVolume = value;
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        SoundManager.Instance?.SetMusicVolume(value);
    }
    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0;

        _controls.PlayerMovement.Disable();
        _controls.PlayerUI.Enable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _menuNavigation.ActivatePauseMenu();
    }
    public void RestartGame()
    {
        levelSettings.ResetToDefault();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1;

        _controls.PlayerMovement.Enable();
        _controls.PlayerUI.Disable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void LoadDefaultSettings()
    {
        gameSettings.aimSensitivity = PlayerPrefs.GetFloat(AimSensitivityKey, gameSettings.aimSensitivity);
        musicSettings.musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, musicSettings.musicVolume);
    }
    public void MainMenu()
    {
        levelSettings.ResetToDefault();
        SceneManager.LoadScene(0);
    }
    private void ApplyAimSensitivity(float sensitivity)
    {
        if (_playerController != null)
            _playerController.SetAimSensitivity(sensitivity);
    }
    public void OnQuitGame()
    {
        Application.Quit();
    }
    
}
