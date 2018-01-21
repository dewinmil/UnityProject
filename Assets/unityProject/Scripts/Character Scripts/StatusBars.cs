using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusBars : MonoBehaviour {

    CharacterStatus playerHealth;

	// Use this for initialization
	void Start () {
        playerHealth = GetComponentInParent<CharacterStatus>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newRotation = new Vector3 (transform.eulerAngles.x,
            Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
        transform.eulerAngles = newRotation;
    }

}
