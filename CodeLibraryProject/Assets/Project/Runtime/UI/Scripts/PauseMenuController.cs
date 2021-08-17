using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    private GameObject pauseMenu;

    private void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.Inst.PauseGameKey))
            HandleMenuActivation(pauseMenu.activeInHierarchy);
    }

    public void HandleMenuActivation(bool pActiveState)
    {
        InputManager.Inst.CanInteract = pActiveState;
        pauseMenu.SetActive(!pActiveState);
    }
}