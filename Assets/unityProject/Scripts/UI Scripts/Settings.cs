using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider brightSlider;
    public Light lt;

    // Use this for initialization
    void Start()
    {
        lt = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBrightnessChanged()
    {
        lt.intensity = brightSlider.value;
    }
}
