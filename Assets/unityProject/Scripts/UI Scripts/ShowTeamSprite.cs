using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTeamSprite : MonoBehaviour {

    public MoveInput _characterMoveInput;


    // Update is called once per frame
    void Update()
    {
        if (_characterMoveInput.isSelected == true)
        {
            if (gameObject.GetComponent<SpriteRenderer>().enabled == true)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }

    }
}

