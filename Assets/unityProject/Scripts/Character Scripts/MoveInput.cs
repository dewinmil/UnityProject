using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveInput : MonoBehaviour
{
    public float minMovRange;
    public Ray ray;
    public bool isSelected;
    public bool targetedBySpell;
    public bool castingSpell;
    public Abilities _characterAbilities;
    public Unit _unit;


    // Update is called once per frame
    void Update()
    {
        //check whether a spell has been cast upon the owner of this script and if so
        //ensure that they were not selected as the spell was cast
        if (targetedBySpell)
        {
            isSelected = false;
        }

        //if the left mouse button is presset
        if (Input.GetButtonUp("Fire1"))
        {
            //if the cursor is not over a UI element
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                //cast a ray from the main camera to the cursor
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    
                    Debug.DrawLine(ray.origin, hit.point);
                    Vector3 selectedPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                    //if we did selected an area right on top of / near the owner of this script
                    if (Vector3.Distance(selectedPosition, _characterAbilities.transform.position) <= 2)
                    {
                        //if they are not having an ability cast upon them
                        if (targetedBySpell == false)
                        {
                            //select the unit
                            isSelected = true;
                            _unit.SelectedUnitChanged();
                        }
                        //someone just cast an ability on this unit - do not select this unit
                        //and the spell has been cast so we are no longer being targeted by an ability
                        else
                        {
                            targetedBySpell = false;
                        }
                    }
                    //if we selected a position away from the unit possessing this script
                    else
                    {
                        //if the owner of this scrip is selected
                        if (isSelected)
                        {
                            //if we selected a different unit
                            if (hit.collider.tag == "Unit")
                            {
                                //if we are casting a ability
                                if (castingSpell)
                                {
                                    //inform targeted unit that it is being casted upon so that it
                                    //does not set itself as selected
                                    hit.collider.gameObject.GetComponent<MoveInput>().targetedBySpell = true;

                                    //we finished casting our ability
                                    castingSpell = false;
                                }
                                //if we were not casting an ability
                                else
                                {
                                    //if we were not attempting to move
                                    if (_unit.moveToggle == false)
                                    {
                                        //de-select this unit
                                        isSelected = false;
                                    }
                                }
                            }
                            //if we selcted the ground somewhere
                            else
                            {
                                //if we were not using an ability
                                if (castingSpell == false)
                                {
                                    //if we were not trying to move
                                    if (_unit.moveToggle == false)
                                    {
                                        //de-select this unit
                                        isSelected = false;
                                    }
                                }
                                //if we were casting an ability
                                else
                                {
                                    //de-select the ability
                                    castingSpell = false;
                                }
                            }

                        }

                    }
                }
            }

        }
    }
}
