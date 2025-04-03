using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public GameObject gameOverCanvas;
    public GameObject levelCompleteCanvas;
    public GameObject pauseMenuFirstSelected;
    public GameObject gameOverFirstSelected;
    public GameObject levelCompleteFirstSelected;
    private EventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = FindObjectOfType<EventSystem>();
    }
    public void ActivatePauseMenu()
    {
        pauseMenuCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        levelCompleteCanvas.SetActive(false);
        _eventSystem.SetSelectedGameObject(pauseMenuFirstSelected);
    }
    public void ActivateGameOverMenu()
    {
        pauseMenuCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        levelCompleteCanvas.SetActive(false);
        _eventSystem.SetSelectedGameObject(gameOverFirstSelected);
    }
    public void ActivateLevelCompleteMenu()
    {
        pauseMenuCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        levelCompleteCanvas.SetActive(true);
        _eventSystem.SetSelectedGameObject(levelCompleteFirstSelected);
    }
}
