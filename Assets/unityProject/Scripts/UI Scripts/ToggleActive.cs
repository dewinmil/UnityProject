using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleActive : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Button>().interactable = false;
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void playerConnected()
    {
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void playerDisconnected()
    {
        gameObject.GetComponent<Button>().interactable = false;
    }
}
