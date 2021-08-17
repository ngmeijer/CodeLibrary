using Mono.Cecil;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    private GameObject pauseMenu;
    private GameObject gameSavesMenu;

    private void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.SetActive(false);
        
        gameSavesMenu = GameObject.Find("Saved Games Data");
        gameSavesMenu.SetActive(false); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.Inst.PauseGameKey)) HandleMainPauseMenu(!pauseMenu.activeInHierarchy);
    }

    public void HandleMainPauseMenu(bool pUIActiveState)
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = pUIActiveState;
        
        InputManager.Inst.CanInteract = !pUIActiveState;
        pauseMenu.SetActive(pUIActiveState);
        gameSavesMenu.SetActive(false);
    }
}