using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSprite : MonoBehaviour {

    public MoveInput character;


    // Update is called once per frame
    void Update()
    {
        if (character.isSelected == true)
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
