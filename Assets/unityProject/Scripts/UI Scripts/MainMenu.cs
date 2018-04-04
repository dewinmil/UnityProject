using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour
{
    public GameMaster _gameMaster;

    public void PlayGame()
    {
        //tutorial had me tack a +1 after build index / don't know why it isn't
        //starting at 0, - may have to change later if we encounter problems.
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void BackToMenu()
    {
        //tutorial had me tack a +1 after build index / don't know why it isn't
        //starting at 0, - may have to change later if we encounter problems.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void RefreshGame()
    {
        NetworkManager.Shutdown();
        SceneManager.LoadScene(0);
    }

}
