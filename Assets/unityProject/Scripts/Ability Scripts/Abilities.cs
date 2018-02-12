﻿using System.Collections;
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
    public CastSpell Button1Animation;
    public CastSpell Button2Animation;
    public CastSpell Button3Animation;
    public CastSpell Button4Animation;
    public CastSpell Button5Animation;
    public CastSpell Button6Animation;
    private KeyCode spellHotkey1 = KeyCode.Alpha1;//number 1
    private KeyCode spellHotkey2 = KeyCode.Alpha2;//number 2
    private KeyCode spellHotkey3 = KeyCode.Alpha3;//number 3
    private KeyCode spellHotkey4 = KeyCode.Alpha4;//number 4
    private KeyCode spellHotkey5 = KeyCode.Alpha5;//number 5
    private KeyCode spellHotkey6 = KeyCode.Space;//number spacebar
    public List<int> usedId = new List<int>();
    
    void Start()
    {
        abilityUsed = 0;
        usingAbility = false;

        if (this._unit.getUnitId() == -1)
        {
            for (int i = 0; i < 50; i++)
            {
                if (! usedId.Contains(i))
                {
                    usedId.Add(i);
                    this._unit.setUnitId(i);
                    break;
                }
            }
        }
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
        //if (usingAbility)
        //{
            if (target.currentHealth > 0)
            {
                float resistance = 0;
                if (_casterStatus.currentAction >= apCost)
                {
                    _casterStatus.loseAction(apCost);

                    if (isMagic)
                    {
                        resistance = target.magicArmor * magicPen;
                        if (resistance < 0 || resistance >= 1) //bad spell
                        {
                            resistance = 0;
                        }
                        target.loseHealth(damage * (1 - resistance));
                    }
                    else
                    {
                        resistance = target.physicalArmor * armorPen;
                        if (resistance < 0 || resistance >= 1)//bad spell
                        {
                            resistance = 0;
                        }
                        target.loseHealth(damage * (1 - resistance));
                    }
                    target.gainHealth(healing);

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
            
       // }

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
                                Button1Animation.Cast(hit.collider.gameObject.GetComponent<CharacterStatus>(), 1);
                            }
                            if (abilityUsed == 2)
                            {
                                Button2Animation.Cast(hit.collider.gameObject.GetComponent<CharacterStatus>(), 2);
                            }
                            if (abilityUsed == 3)
                            {
                                Button3Animation.Cast(hit.collider.gameObject.GetComponent<CharacterStatus>(), 3);
                            }
                            if (abilityUsed == 4)
                            {
                                Button4Animation.Cast(hit.collider.gameObject.GetComponent<CharacterStatus>(), 4);
                            }
                            if (abilityUsed == 5)
                            {
                                Button5Animation.Cast(hit.collider.gameObject.GetComponent<CharacterStatus>(), 5);
                            }
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


    public void ability1(CharacterStatus target)
    {
        castAbility(target, 3, 0, 3, (float).5, 0, 0, false);
    }


    public void ability2(CharacterStatus target)
    {
        castAbility(target, 3, 0, 3, (float).5, 0, 0, false);
    }

    public void ability3(CharacterStatus target)
    {
        castAbility(target, 0, 3, 3, 0, 0, 0, false);
    }

    public void ability4(CharacterStatus target)
    {
        castAbility(target, 3, 0, 3, (float).5, 0, 0, false);
    }

    public void ability5(CharacterStatus target)
    {
        castAbility(target, 3, 0, 3, (float).5, 0, 0, false);
    }

}
