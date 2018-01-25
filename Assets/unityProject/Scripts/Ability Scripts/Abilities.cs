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
    private KeyCode spellHotkey1 = KeyCode.Alpha1;//number 1
    private KeyCode spellHotkey2 = KeyCode.Alpha2;//number 2
    private KeyCode spellHotkey3 = KeyCode.Alpha3;//number 3
    private KeyCode spellHotkey4 = KeyCode.Alpha4;//number 4
    private KeyCode spellHotkey5 = KeyCode.Alpha5;//number 5
    private KeyCode spellHotkey6 = KeyCode.Space;//number spacebar
    
    void Start()
    {
        abilityUsed = 0;
        usingAbility = false;

    }

    // Use this for initialization
    public void useAbility(int ability) {
        toggleCasting();
        abilityUsed = ability;
    }

    // Use this for initialization
    public void castAbility(CharacterStatus target, float damage, float healing, float apCost, float armorPen, float magicPen, float range, bool isMagic)
    {
        //toggleCasting();
        if (usingAbility)
        {
            if(target.currentHealth > 0)
            {
                float resistance = 0;
                if (_casterStatus.currentAction >= apCost)
                {
                    _casterStatus.currentAction -= apCost;

                    if (isMagic)
                    {
                        resistance = target.magicArmor * magicPen;
                        if (resistance < 0 || resistance >= 1) //bad spell
                        {
                            resistance = 0;
                        }
                        target.currentHealth -= damage * (1 - resistance);
                    }
                    else
                    {
                        resistance = target.physicalArmor * armorPen;
                        if (resistance < 0 || resistance >= 1)//bad spell
                        {
                            resistance = 0;
                        }
                        target.currentHealth -= damage * (1 - resistance);
                    }
                    target.currentHealth += healing;

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
            
        }

    }

    public void toggleMovement()
    {
        if (usingAbility)
        {
            _unit.moveToggle = false;
            usingAbility = false;
        }
        else
        {
            _unit.toggleMovement();
        }
    }

    // Update is called once per frame
    void Update () {
        if (_casterMoveInput.isSelected)
        {
            if (Input.GetKeyUp(spellHotkey1))
            {
                toggleCasting();
                abilityUsed = 1;
            }
            else if (Input.GetKeyUp(spellHotkey2))
            {
                toggleCasting();
                abilityUsed = 2;
            }
            else if (Input.GetKeyUp(spellHotkey3))
            {
                toggleCasting();
                abilityUsed = 3;
            }
            else if (Input.GetKeyUp(spellHotkey4))
            {
                toggleCasting();
                abilityUsed = 4;
            }
            else if (Input.GetKeyUp(spellHotkey5))
            {
                toggleCasting();
                abilityUsed = 5;
            }
            else if (Input.GetKeyUp(spellHotkey6))
            {
                toggleMovement();
            }
        }



        if (usingAbility == true)
        {
            if (Input.GetButtonUp("Fire1"))
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
                            if (abilityUsed == 3)
                            {
                                ability3(hit.collider.gameObject.GetComponent<CharacterStatus>());
                            }
                            if (abilityUsed == 4)
                            {
                                ability4(hit.collider.gameObject.GetComponent<CharacterStatus>());
                            }
<<<<<<< HEAD
=======
                            if (abilityUsed == 5)
                            {
                                ability5(hit.collider.gameObject.GetComponent<CharacterStatus>());
                            }

>>>>>>> 1bc1eff12ab54b897fa681f365c45821152dc3fe
                        }
                    }
                    usingAbility = false;
                    abilityUsed = 0;
                }
                
            }
        }
    }

    void toggleCasting()
    {
        if (_unit.moveToggle)
        {
            _unit.moveToggle = false;
        }
        else
        {
            if (usingAbility)
            {
                usingAbility = false;
                _casterMoveInput.castingSpell = false;
            }
            else
            {
                usingAbility = true;
                _casterMoveInput.castingSpell = true;
            }
        }
        
    }


    void ability1(CharacterStatus target)
    {
        castAbility(target, 3, 0, 3, (float).5, 0, 0, false);
    }


    void ability2(CharacterStatus target)
    {
        castAbility(target, 3, 0, 5, 0, (float).5, 2, true);
    }

    void ability3(CharacterStatus target)
    {
        castAbility(target, 0, 0, 3, 0, 0, 0, false);
    }

    void ability4(CharacterStatus target)
    {
        castAbility(target, 0, 3, 3, 0, 0, 3, false);
    }

    void ability5(CharacterStatus target)
    {
        castAbility(target, 0, 0, 0, 0, 0, 0, false);
    }

    void ability3(CharacterStatus target)
    {
        castAbility(target, 0, 3, 3, 0, 0, 0, false);
    }

    void ability4(CharacterStatus target)
    {
        castAbility(target, 0, 3, 3, 0, 0, 0, false);
    }

    void ability5(CharacterStatus target)
    {
        castAbility(target, 0, 3, 3, 0, 0, 0, false);
    }
}
