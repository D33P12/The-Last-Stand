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

    private MenuNavigation menuNavigation;

    private void Awake()
    {
        _controls = new Inputs();
        pauseMenuCanvas.SetActive(false);
        _shootiController = FindObjectOfType<ShootiController>();
        _playerController = FindObjectOfType<PlayerController>();

        menuNavigation = FindObjectOfType<MenuNavigation>();
    }

    private void Start()
    {
        _controls.PlayerUI.Disable();
        SoundManager.Instance.PlayAudioContinuous(SoundManager.AudioType.Bg, 0.5f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LoadSettings();

        if (aimSensitivitySlider != null && gameSettings != null)
        {
            aimSensitivitySlider.value = gameSettings.aimSensitivity;
        }
        aimSensitivitySlider.onValueChanged.AddListener(OnAimSensitivityChanged);

        if (musicVolumeSlider != null && musicSettings != null)
        {
            musicVolumeSlider.value = musicSettings.musicVolume;
        }
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
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
    }

    private void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (GameOverScript.Instance.gameOverCanvas.activeSelf)
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

    private void OnCloseMenu(InputAction.CallbackContext context)
    {
        ResumeGame();
    }

    private void OnAimSensitivityChanged(float value)
    {
        if (gameSettings != null)
        {
            gameSettings.aimSensitivity = value;
            PlayerPrefs.SetFloat(AimSensitivityKey, value);
            ApplyAimSensitivity(value);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (musicSettings != null)
        {
            musicSettings.musicVolume = value;
            SoundManager.Instance.SetMusicVolume(value);
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
        }
    }

    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true);
        _playerController.IsPaused = true;
        _shootiController.SetCanShoot(false);
        EnablePlayerUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;

        menuNavigation.ActivatePauseMenu(); 
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        _playerController.IsPaused = false;
        _shootiController.SetCanShoot(true);
        EnablePlayerMovement();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
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

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(AimSensitivityKey))
        {
            gameSettings.aimSensitivity = PlayerPrefs.GetFloat(AimSensitivityKey);
        }

        if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            musicSettings.musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
        }
    }

    private void ApplyAimSensitivity(float sensitivity)
    {
        if (_playerController != null)
        {
            _playerController.SetAimSensitivity(sensitivity);
        }
    }
}
