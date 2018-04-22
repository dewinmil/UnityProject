using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamera : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }

}
