using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool PauseMenuActive = false;

    [SerializeField] private GameObject pauseMenu = null;

    void Update()
    {
        showPauseMenu();
    }

    private void showPauseMenu()
    {
        if (pauseMenu != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PauseMenuActive)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    public void PauseGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        PauseMenuActive = true;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        PauseMenuActive = false;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
