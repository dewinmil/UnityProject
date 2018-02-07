using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour {

    public bool usingAbility;
    int abilityUsed;
    public Ray ray;
    public CharacterStatus character;
    public MoveInput caster;

    void Start()
    {
        abilityUsed = 0;
        usingAbility = false;
        //print("used ability 1");
        //character.isSelected = true;
    }

    // Use this for initialization
    public void useAbility(int ability) {
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

    void ability1(CharacterStatus target)
    {
        //note character and caster are the same unit / just different scripts
        caster.castingSpell = true;
        if (character.currentAction >= 3)
        {
            character.currentAction -= 3;
            target.currentHealth -= 3;
            if (target.currentHealth <= 0)
            {
                target.currentHealth = 0;
            }
            usingAbility = false;
        }
        else
        {
            //print not enough AP
            usingAbility = false;
        }
    }

    void ability2(CharacterStatus target)
    {
        //note character and caster are the same unit / just different scripts
        caster.castingSpell = true;
        if (character.currentAction >= 3)
        {
            character.currentAction -= 3;
            target.currentHealth += 3;
            if (target.currentHealth >= target.maxHealth)
            {
<<<<<<< Updated upstream
                target.currentHealth = target.maxHealth;
=======
                if (EventSystem.current.IsPointerOverGameObject() == false)
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
                            if (abilityUsed == 3)
                            {
                                ability3(hit.collider.gameObject.GetComponent<CharacterStatus>());
                            }
                            if (abilityUsed == 4)
                            {
                                ability4(hit.collider.gameObject.GetComponent<CharacterStatus>());
                            }
                            if (abilityUsed == 5)
                            {
                            //   ability5(hit.collider.gameObject.GetComponent<CharacterStatus>());
                            }
                        }
                    }
                    usingAbility = false;
                    abilityUsed = 0;
                }
                
>>>>>>> Stashed changes
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
