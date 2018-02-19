using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class SettingsMenu : MonoBehaviour
    {
        public void Click_ButtonReturn() { SceneManager.LoadScene(0); }

        public void ValueChanged_VolumeSlider(float value)
        {
            AudioListener.volume = value;
        }
    }
}