using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveInput : MonoBehaviour
{

    //public Transform unit1Pointer;
    //public unitMovement unit1Movement;
    public float minMovRange;
    //public SpriteRenderer cursor1;
    public Ray ray;
    public bool isSelected;
    public bool targetedBySpell;
    public bool castingSpell;
    public Abilities _characterAbilities;
    public Unit _unit;

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
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    //Debugging Ray
                    Debug.DrawLine(ray.origin, hit.point);

                    Vector3 selectedPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                    if (Vector3.Distance(selectedPosition, _characterAbilities.transform.position) <= 2)
                    {
                        if (targetedBySpell == false)
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

                            if (hit.collider.tag == "Unit")
                            {
                                if (castingSpell)
                                {
                                    hit.collider.gameObject.GetComponent<MoveInput>().targetedBySpell = true;
                                    castingSpell = false;
                                }
                                else
                                {
                                    if (_unit.moveToggle == false)
                                    {
                                        isSelected = false;
                                    }
                                }
                            }
                            else
                            {
                                if (castingSpell == false)
                                {
                                    if (_unit.moveToggle == false)
                                    {
                                        isSelected = false;
                                    }
                                }
                                else
                                {
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
