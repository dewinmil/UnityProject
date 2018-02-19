using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class StartMenu : MonoBehaviour
    {

        public void Click_ButtonExit() { Application.Quit(); }

        public void Click_ButtonSettings() { SceneManager.LoadScene("SettingsMenu"); }

        public void Click_ButtonPlay() { }
    }
}