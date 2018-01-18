using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInput : MonoBehaviour {

  public Transform unit1Pointer;
  public unitMovement unit1Movement;
  public float minMovRange;
  public SpriteRenderer cursor1;
  public Ray ray;
  public bool isSelected;
	
  // Update is called once per frame
  void Update () {
    //Get Input
    if (Input.GetButton("Fire1"))
    {
      ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, 100))
      {
        //Debugging Ray
        Debug.DrawLine(ray.origin, hit.point);
          
        //Move The Pointer
        Vector3 selectedPosition = new Vector3(hit.point.x, unit1Pointer.position.y, hit.point.z);

        if (Vector3.Distance(selectedPosition, unit1Movement.transform.position) <= minMovRange)
        {
          isSelected = true;
        }
        else
        {
          isSelected = false;
        }
      }
    }
    if (Input.GetButton("Fire2"))
    {
      //Cast ray to get position of cursor on Terrain
      ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (isSelected == true)
      {
        if (Physics.Raycast(ray, out hit, 100))
        {
          if (hit.collider.tag == "Terrain")
          {
            //Debugging Ray
            Debug.DrawLine(ray.origin, hit.point);
          
            //Move The Pointer
            unit1Pointer.position = new Vector3(hit.point.x, unit1Pointer.position.y, hit.point.z);

            //Only moves if the distance between the pointer and the unit is big enough
            if (Vector3.Distance(unit1Pointer.position, unit1Movement.transform.position) > minMovRange)
            {
              unit1Movement.moving = true;
              cursor1.enabled = true;
            }
          }
        }
      }
    }
  }
}
