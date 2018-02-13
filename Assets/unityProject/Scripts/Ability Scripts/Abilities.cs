using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Abilities : MonoBehaviour
{

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
                if (!usedId.Contains(i))
                {
                    usedId.Add(i);
                    this._unit.setUnitId(i);
                    break;
                }
            }
        }
    }

    // Use this for initialization
    public void useAbility(int ability)
    {
        //toggle casting determins whether your next click will cast a spell or not
        //it is a toggle so you can reselect a spell to cancel casting
        toggleCasting();

        //sets the last ability selected
        abilityUsed = ability;
    }

    //this function actually applies the spell effect to the target
    public void castAbility(CharacterStatus target, float damage, float healing, float apCost, float armorPen, float magicPen, float range, bool isMagic)
    {
        //if the target is not dead
        if (target.currentHealth > 0)
        {
            float resistance = 0;
            //check if the caster has enough AP to cast a spell - currently redundant
            if (_casterStatus.currentAction >= apCost)
            {
                //use casters ability points
                _casterStatus.loseAction(apCost);

                //check if the spell effect is reduced by magic resistance or armor
                if (isMagic)
                {
                    resistance = target.magicArmor * magicPen;

                    //ensure penetration doesnt add damage and that resistance doesn't cause healing
                    if (resistance < 0 || resistance >= 1) //bad spell
                    {
                        resistance = 0;
                    }

                    //deal damage wieghted by resistance
                    target.loseHealth(damage * (1 - resistance));
                }
                else
                {
                    resistance = target.physicalArmor * armorPen;

                    //ensure penetration doesnt add damage and that resistance doesn't cause healing
                    if (resistance < 0 || resistance >= 1)//bad spell
                    {
                        resistance = 0;
                    }

                    //deal damage weighted by resistance
                    target.loseHealth(damage * (1 - resistance));
                }


                //check if target is dead - may have received damage before healing
                if (target.currentHealth <= 0)
                {
                    target.currentHealth = 0;
                }
                else
                {
                    //if alive heal the target
                    target.gainHealth(healing);
                    if(target.currentHealth > target.maxHealth)
                    {
                        target.currentHealth = target.maxHealth;
                    }
                }

                //ability is finished set boolean
                usingAbility = false;
            }
            else
            {
                //print not enough AP - set boolean
                usingAbility = false;
            }
        }

    }

    public void toggleMovement()
    {
        //if an ability is being used cancel the ability
        if (usingAbility)
        {
            _unit.moveToggle = false;
            usingAbility = false;
        }

        //otherwise toggle moement on the unity -
        else
        {
            //NOTE - THIS IS A DIFFERENT TOGGLE MOVEMENT FUNCTION BELONGING TO THE UNIT CLASS
            //toggles movement boolean between true or false
            _unit.toggleMovement();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //check if the unit is selected
        if (_casterMoveInput.isSelected)
        {
            //enable or disable spellcast on keypress
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


        //if the caster has an ability selected
        if (usingAbility == true)
        {
            //if the left mouse button is pressed
            if (Input.GetButtonUp("Fire1"))
            {
                //if the mouse is not over a UI element 
                if (EventSystem.current.IsPointerOverGameObject() == false)
                {
                    //cast a ray from the main camera to the mouse
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        //if the ray hits an object with the Unit tag
                        if (hit.collider.tag == "Unit")
                        {
                            //cast an ability
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
                    //ability has been cast so de-select it
                    usingAbility = false;
                    abilityUsed = 0;
                }

            }
        }
    }

    //toggles whether or not a spell is selected
    void toggleCasting()
    {
        //if a unit had movement selected set state to deselected it
        if (_unit.moveToggle)
        {
            _unit.moveToggle = false;
        }
        else
        {
            //if a unit had an ability selected - set state to deselected
            if (usingAbility)
            {
                usingAbility = false;
                _casterMoveInput.castingSpell = false;
            }
            //if the unit did not have an ability selected - set state to casting / selected
            else
            {
                usingAbility = true;
                _casterMoveInput.castingSpell = true;
            }
        }

    }


    //methods to apply different spell effects on a target - these are your spells
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
