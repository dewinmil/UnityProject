using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class unitMoveStop : NetworkBehaviour {
  
  
  public float rotoSpeed;
  public unitMovement unitMovement;
  public SpriteRenderer cursorSprite;

  // Use this for initialization
  void Start()
  {
    cursorSprite.enabled = false;
  }
  
  // Update is called once per frame
  void Update () {
    //Just a visual effect for the pointer sprite
    transform.Rotate(Vector3.forward * rotoSpeed * Time.deltaTime);
  }
  
  void OnTriggerEnter(Collider other)
  {
    //if(other.tag == unitMovement.tag)
    {
            if (unitMovement.moving)
            {
                unitMovement.moving = false;
                cursorSprite.enabled = false;
            }
            
    }
  }
}
