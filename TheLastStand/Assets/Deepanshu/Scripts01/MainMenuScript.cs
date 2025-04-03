using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
  private Inputs _controls;
  private void Awake()
  {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    _controls = new Inputs();
  }
  private void OnEnable()
  {
    _controls.PlayerUI.Enable();
    _controls.PlayerUI.Submit.performed += OnStartGame;
    _controls.PlayerUI.Cancel.performed += OnQuitGame;
  }
  private void OnDisable()
  {
    _controls.PlayerUI.Submit.performed -= OnStartGame;
    _controls.PlayerUI.Cancel.performed -= OnQuitGame;
    _controls.PlayerUI.Disable();
  }
  private void OnStartGame(InputAction.CallbackContext context)
  {
    SceneManager.LoadScene(1);
  }
  public void OnStartGame()
  {
    SceneManager.LoadScene(1);
  }
  private void OnQuitGame(InputAction.CallbackContext context)
  {
    Application.Quit();
  }
}
