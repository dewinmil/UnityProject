using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour
{
    public GameMaster _gameMaster;


    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void RefreshGame()
    {
        SceneManager.LoadScene(0);
    }

}
