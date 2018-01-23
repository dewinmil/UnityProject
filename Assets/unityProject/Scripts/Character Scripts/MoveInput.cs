using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveInput : MonoBehaviour
{

    public Transform unit1Pointer;
    public unitMovement unit1Movement;
    public float minMovRange;
    public SpriteRenderer cursor1;
    public Ray ray;
    public bool isSelected;
    public bool targetedBySpell;
    public bool castingSpell;
    public Abilities character;
    public Unit unit;

    // Update is called once per frame
    void Update()
    {
        //Get Input
        if (targetedBySpell)
        {
            isSelected = false;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                //Debugging Ray
                Debug.DrawLine(ray.origin, hit.point);

                Vector3 selectedPosition = new Vector3(hit.point.x, unit1Pointer.position.y, hit.point.z);

                if (Vector3.Distance(selectedPosition, unit1Movement.transform.position) <= 2)
                {
                    if(targetedBySpell == false)
                    {
                        isSelected = true;
                    }
                    else
                    {
                        targetedBySpell = false;
                    }
                }
                else
                {
                    if (isSelected)
                    {
                        if (EventSystem.current.IsPointerOverGameObject() == false)
                        {
                            if (hit.collider.tag == "Unit")
                            {
                                if (castingSpell)
                                {
                                    hit.collider.gameObject.GetComponent<MoveInput>().targetedBySpell = true;
                                    castingSpell = false;
                                }
                                else
                                {
                                    if (unit.moveToggle == false)
                                    {
                                        isSelected = false;
                                    }
                                }
                            }
                            else
                            {
                                if (unit.moveToggle == false)
                                {
                                    isSelected = false;
                                }
                            }
                        }
                    }
                    
                }
            }
        }
        if (Input.GetButtonUp("Fire2"))
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
                        if (Vector3.Distance(unit1Pointer.position, unit1Movement.transform.position) > 2)
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
