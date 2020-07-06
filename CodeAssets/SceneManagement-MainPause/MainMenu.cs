using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region

    [SerializeField] private GameObject mainMenu = null;
    [SerializeField] private GameObject questionMenu = null;
    [SerializeField] private GameObject creditsMenu = null;

    [SerializeField] private Animator transitionAnim = null;
    [SerializeField] private float transitionTime = 3f;

    #endregion

    public void StartGame()
    {
        StartCoroutine(loadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void SetYesNoActive()
    {
        mainMenu.SetActive(false);
        questionMenu.SetActive(true);
    }

    public void SetMainMenuActive()
    {
        mainMenu.SetActive(true);
        questionMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void SetCreditsActive()
    {
        mainMenu.SetActive(false);
        questionMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator loadLevel(int levelIndex)
    {
        if(transitionAnim != null)
        {
            Debug.Log("triggering fade out");

            transitionAnim.SetTrigger("StartScene");

            yield return new WaitForSeconds(transitionTime);

            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.Log("cross fade anim is null");
        }
    }
}
