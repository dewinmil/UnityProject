using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSelectedSprite : MonoBehaviour {

    public MoveInput _characterMoveInput;


    // Update is called once per frame
    void Update()
    {
        if (_characterMoveInput.isSelected == true)
        {
            if (gameObject.GetComponent<SpriteRenderer>().enabled == false)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

    }
}
