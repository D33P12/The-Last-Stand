using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public GameObject gameOverCanvas;
    public GameObject pauseMenuFirstSelected; 
    public GameObject gameOverFirstSelected; 
    private EventSystem eventSystem;
    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }
    public void ActivatePauseMenu()
    {
        pauseMenuCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        eventSystem.SetSelectedGameObject(pauseMenuFirstSelected); 
    }
    public void ActivateGameOverMenu()
    {
        pauseMenuCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        eventSystem.SetSelectedGameObject(gameOverFirstSelected);
    }
}
