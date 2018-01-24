using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Abilities : MonoBehaviour {

    public bool usingAbility;
    int abilityUsed;
    public Ray ray;
    public CharacterStatus _casterStatus;
    public MoveInput _casterMoveInput;
    public Unit _unit;

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

    public void toggleMovement()
    {
        _unit.toggleMovement();
    }

    // Update is called once per frame
    void Update () {
        if(usingAbility == true)
        {
            if (Input.GetButton("Fire1"))
            {
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

                        }
                    }
                    usingAbility = false;
                    abilityUsed = 0;
                }
                
            }
        }
    }

    void ability1(CharacterStatus target)
    {
        //note character and caster are the same unit / just different scripts
        _casterMoveInput.castingSpell = true;
        if (_casterStatus.currentAction >= 3)
        {
            _casterStatus.currentAction -= 3;
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
        _casterMoveInput.castingSpell = true;
        if (_casterStatus.currentAction >= 3)
        {
            _casterStatus.currentAction -= 3;
            target.currentHealth += 3;
            if (target.currentHealth >= target.maxHealth)
            {
                target.currentHealth = target.maxHealth;
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
