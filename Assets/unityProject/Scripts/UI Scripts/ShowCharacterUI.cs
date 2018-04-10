using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCharacterUI : MonoBehaviour {


    public MoveInput character;
	
	// Update is called once per frame
	void Update () {
        if (character.isSelected == true)
        {
            if (gameObject.GetComponent<Canvas>().enabled == false)
            {
                gameObject.GetComponent<Canvas>().enabled = true;
            }
        }
        else
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }
		
	}
}
