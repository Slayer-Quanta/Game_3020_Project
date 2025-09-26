using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string tutorialScene = "Tutorial";

    public void LoadTutorial()
    {
        SceneManager.LoadScene(tutorialScene);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}