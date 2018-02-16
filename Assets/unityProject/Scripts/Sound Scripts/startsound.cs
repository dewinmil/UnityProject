using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startsound : MonoBehaviour {

	// Use this for initialization
	void Start () {
        FindObjectOfType<AudioManager>().Play("Menu Music");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
