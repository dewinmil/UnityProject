using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class MoveInput : NetworkBehaviour
{
    public Ray ray;
    public bool isSelected;
    public bool targetedBySpell;
    public bool castingSpell;
    public Abilities _characterAbilities;
    public Unit _unit;
    public EndTurn endTurn;

    public void Start()
    {
        endTurn = FindObjectOfType<EndTurn>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //deselect unit if it is not their turn
            if (endTurn.turn != gameObject.GetComponent<CharacterStatus>().teamNum
                || gameObject.GetComponent<CharacterStatus>().currentHealth < 0)
            {
                //select the unit
                isSelected = false;
                _unit.SelectedUnitChanged();
            }

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
                        Vector3 selectedPosition = new Vector3(hit.point.x, hit.point.y - hit.point.y - .416f, hit.point.z);

                        //if we did selected an area right on top of / near the owner of this script
                        if (Vector3.Distance(selectedPosition, _characterAbilities.transform.position) <= .9)
                        {
                            //if they are not having an ability cast upon them
                            if (targetedBySpell == false)
                            {
                                if (endTurn.turn == gameObject.GetComponent<CharacterStatus>().teamNum
                                    && gameObject.GetComponent<CharacterStatus>().currentHealth > 0)
                                {
                                    //select the unit
                                    isSelected = true;
                                    _unit.SelectedUnitChanged();
                                    _unit._map.charSelect = true;
                                }
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
                                        if (_unit._isMoving == false)
                                        {
                                            //de-select this unit
                                            isSelected = false;
                                            _unit._map.charSelect = false;
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
                                        if (_unit._isMoving == false)
                                        {
                                            //de-select this unit
                                            isSelected = false;
                                            _unit._map.charSelect = false;
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
}
