using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellIndicator : MonoBehaviour
{

    RaycastHit hit;
    Ray ray;
    public MoveInput _selected;
    public bool firstTargetSelected;
    bool usingAbility;
    private GameObject line;
    List<GameObject> list = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        firstTargetSelected = false;
        line.GetComponent<BoxCollider>().enabled = false;
        line.transform.localScale = new Vector3((float).5, (float).001, 1);
        list.Add(line);
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
                        list[0].transform.GetComponent<MeshRenderer>().enabled = true;
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

                            float zPos = Mathf.Pow(transform.position.z - _selected.transform.position.z, 2);
                            float xPos = Mathf.Pow(transform.position.x - _selected.transform.position.x, 2);
                            float distance = Mathf.Sqrt(zPos + xPos);
                            if (distance >= list.Count + 5)
                            {
                                list.Add(Instantiate(line));
                                list.Add(Instantiate(line));
                            }
                            if (distance < list.Count + 2 && list.Count >=3)
                            {
                                GameObject.Destroy(list[list.Count - 1]);
                                list.RemoveAt(list.Count - 1);
                                GameObject.Destroy(list[list.Count - 1]);
                                list.RemoveAt(list.Count - 1);
                            }
                            if(list.Count == 1)
                            {
                                //halfway berween transform and _selected (but on the same plane as transform)

                                line.transform.position = new Vector3(_selected.transform.position.x + (transform.position.x - _selected.transform.position.x)/2,
                                    transform.position.y,
                                    _selected.transform.position.z + (transform.position.z - _selected.transform.position.z)/2);

                                line.transform.eulerAngles = new Vector3(line.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                            }
                            else
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    list[i].transform.position = new Vector3(_selected.transform.position.x + (transform.position.x - _selected.transform.position.x) * ((float)i / (float)list.Count),
                                        transform.position.y,
                                        _selected.transform.position.z + (transform.position.z - _selected.transform.position.z) * ((float)i / (float)list.Count));

                                    list[i].transform.eulerAngles = new Vector3(line.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                                }
                            }
                        }
                    }
                    else
                    {
                        int max = list.Count;
                        for (int i = 1; i < max; i++)
                        {
                            transform.GetComponent<SpriteRenderer>().enabled = false;
                            list[0].transform.GetComponent<MeshRenderer>().enabled = false;
                            GameObject.Destroy(list[list.Count - 1]);
                            list.RemoveAt(list.Count - 1);
                        }

                    }

                }
            }
        }
    }
}