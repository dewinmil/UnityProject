using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{

    RaycastHit hit;
    Ray ray;
    public MoveInput _selected;
    public bool firstTargetSelected;
    bool usingAbility;

    // Use this for initialization
    void Start()
    {
        firstTargetSelected = false;
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (firstTargetSelected)
        {
            if (_selected.GetComponentInChildren<Abilities>().usingAbility)
            {
                usingAbility = true;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<MoveInput>())
                {
                    if (usingAbility)
                    {
                        usingAbility = false;
                    }
                    else
                    {
                        firstTargetSelected = true;
                        _selected = hit.collider.gameObject.GetComponent<MoveInput>();
                    }
                }
            }
        }
        else
        {
            if (firstTargetSelected)
            {
                if (_selected.isSelected)
                {
                    if (_selected.GetComponentInChildren<Abilities>().usingAbility)
                    {
                        transform.GetComponent<SpriteRenderer>().enabled = true;
                        if (Physics.Raycast(ray, out hit, 100))
                        {
                            transform.position = new Vector3(hit.point.x, hit.point.y + (float).02, hit.point.z);

                            float zVal = transform.position.z - _selected.transform.position.z;
                            float xVal = transform.position.x - _selected.transform.position.x;
                            float rotation;
                            float difference;
                            rotation = 0;


                            while (Mathf.Abs(xVal) < 1 || Mathf.Abs(zVal) < 1)
                            {
                                xVal = xVal + xVal;
                                zVal = zVal + zVal;
                            }
                            if (Mathf.Abs(xVal) <= Mathf.Abs(zVal))
                            {
                                difference = Mathf.Abs(xVal) / Mathf.Abs(zVal);
                                rotation = 45 * difference;
                                if (xVal < 0 && zVal < 0)
                                {
                                    rotation += 180;
                                }
                                else if (xVal < 0)
                                {
                                    rotation = 360 - rotation;
                                }
                                else if (zVal < 0)
                                {
                                    rotation = 180 - rotation;
                                }
                            }
                            else
                            {
                                difference = Mathf.Abs(zVal) / Mathf.Abs(xVal);
                                rotation = 45 + (45 - (45 * difference));
                                if (xVal < 0 && zVal < 0)
                                {
                                    rotation += 180;
                                }
                                else if (xVal < 0)
                                {
                                    rotation = 360 - rotation;
                                }
                                else if (zVal < 0)
                                {
                                    rotation = 180 - rotation;
                                }
                            }
                            transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotation, transform.eulerAngles.z);
                        }
                    }
                    else
                    {
                        transform.GetComponent<SpriteRenderer>().enabled = false;
                    }

                }
            }
        }
    }
}