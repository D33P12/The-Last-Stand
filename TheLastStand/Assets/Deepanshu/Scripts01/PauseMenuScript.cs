using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{   
   private Inputs _controls;
    public GameObject pauseMenuCanvas;
    public Slider aimSensitivitySlider;
    public Slider musicVolumeSlider;
    private ShootiController shootiController;
    private PlayerController playerController;
    public GameSettings gameSettings;
    public MusicSettings musicSettings;
    private void Awake()
    {
        _controls = new Inputs();
        pauseMenuCanvas.SetActive(false);
        shootiController = FindObjectOfType<ShootiController>();
        playerController = FindObjectOfType<PlayerController>();
    }
    private void Start()
    {
        SoundManager.Instance.PlayAudioContinuous(SoundManager.AudioType.BG, 0.5f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        }
    }
    private void OnMusicVolumeChanged(float value)
    {
        if (musicSettings != null)
        {
            musicSettings.musicVolume = value;
            SoundManager.Instance.SetMusicVolume(value);
        }
    }
    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true);
        playerController.isPaused = true;
        shootiController.SetCanShoot(false);
        EnablePlayerUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        playerController.isPaused = false;
        shootiController.SetCanShoot(true);
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
}
