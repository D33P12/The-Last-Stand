using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{ 
    public static GameOverScript Instance { get; private set; }
    private Inputs _controls;
    public GameObject gameOverCanvas;
    private ShootiController _shootiController;
    private PlayerController _playerController;
    public float gameDuration = 60f;
    private float _timer;
    public TextMeshProUGUI timerText;
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
        gameOverCanvas.SetActive(false);
        _shootiController = FindObjectOfType<ShootiController>();
        _playerController = FindObjectOfType<PlayerController>();
        _timer = gameDuration;
        _menuNavigation = FindObjectOfType<MenuNavigation>();
    }
    private void Start()
    {
        StartCoroutine(GameTimer());
    }
    private void OnEnable()
    {
        _controls.PlayerMovement.Enable();
        _controls.PlayerUI.Disable();
    }
    private void OnDisable()
    {
        _controls.PlayerMovement.Disable();
        _controls.PlayerUI.Disable();
    }
    private void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (!gameOverCanvas.activeSelf && Time.timeScale != 0)
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        _playerController.IsPaused = true;
        _shootiController.SetCanShoot(false);
        EnablePlayerUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverCanvas.SetActive(true);
    }
    public void RestartGame()
    {
        levelSettings.ResetToDefault();
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
    private System.Collections.IEnumerator GameTimer()
    {
        while (_timer > 0)
        {
            yield return new WaitForSeconds(1f);
            _timer--;
            UpdateTimerText();
        }
        GameOver();
    }
    internal void GameOver()
    {
        gameOverCanvas.SetActive(true);
        _playerController.IsPaused = true;
        _shootiController.SetCanShoot(false);
        EnablePlayerUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;

        _menuNavigation.ActivateGameOverMenu();
    }
    public void MainMenu()
    {
        levelSettings.ResetToDefault();
        SceneManager.LoadScene(0);
    }
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.FloorToInt(_timer).ToString();
        }
    }
    public void OnQuitGame()
    {
        Application.Quit();
    }
}
