using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour {

    public bool usingAbility;
    int abilityUsed;
    public Ray ray;
    //public MoveInput character;

    void Start()
    {
        abilityUsed = 0;
        usingAbility = false;
        //print("used ability 1");
        //character.isSelected = true;
    }

    // Use this for initialization
    public void useAbility(int ability) {
        print("used ability 1");
        usingAbility = true;
        abilityUsed = ability;
    }

    // Update is called once per frame
    void Update () {
        if(usingAbility == true)
        {
            if (Input.GetButton("Fire1"))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.tag == "Unit")
                    {
                        if (abilityUsed == 1)
                        {
                            ability1(hit.collider.gameObject.GetComponent<CharacterStatus>());
                        }
                        if (abilityUsed == 2)
                        {
                            ability2(hit.collider.gameObject.GetComponent<CharacterStatus>());
                        }

                    }
                }
                usingAbility = false;
                abilityUsed = 0;
            }
        }
    }

    void ability1(CharacterStatus status)
    {
        if (status.currentAction >= 3)
        {
            status.currentAction -= 3;
            status.currentHealth -= 3;
            if (status.currentHealth <= 0)
            {
                status.currentHealth = 0;
            }
            usingAbility = false;
        }
        else
        {
            //print not enough AP
            usingAbility = false;
        }
    }

    void ability2(CharacterStatus status)
    {
        if (status.currentAction >= 3)
        {
            status.currentAction -= 3;
            status.currentHealth += 3;
            if (status.currentHealth >= status.maxHealth)
            {
                status.currentHealth = status.maxHealth;
            }
            usingAbility = false;
        }
        else
        {
            //print not enough AP
            usingAbility = false;
        }
    }
}
