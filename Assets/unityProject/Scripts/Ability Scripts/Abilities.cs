using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class Abilities : NetworkBehaviour
{
    public bool usingAbility;

    public int abilityUsed;
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
    public GameObject winScreen;
    public GameObject loseScreen;
    int buttonPressed;
    bool rangeCheck;
    AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        rangeCheck = false;
        winScreen = GameObject.FindWithTag("winScreen");
        loseScreen = GameObject.FindWithTag("loseScreen");
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

    // called on buttonpress
    public void useAbility(int ability)
    {
        audioManager.uiSelected();
        //toggle casting determins whether your next click will cast a spell or not
        //it is a toggle so you can reselect a spell to cancel casting
        rangeCheck = true;
        int spellRange = canCast(null, ability);
        rangeCheck = false;
        if (spellRange != -1)
        {
            Unit _unit = gameObject.GetComponentInParent<CharacterStatus>()._unit;
            FindObjectOfType<TileMap>().HighlightTargetableTiles(_unit.tileX, _unit.tileZ, spellRange);
            toggleCasting();
            //sets the last ability selected
            abilityUsed = ability;
        }
    }
    
    //this function actually applies the spell effect to the target
    public void castAbility(CharacterStatus target, float damage, float healing, float apCost, float armorPen, float magicPen, float buff, bool isMagic)
    {
        _unit.UnhighlightWalkableTiles();
        _unit.transform.LookAt(target.transform.position);
        target.transform.LookAt(_unit.transform.position);
        //if the target is not dead
        if (target.currentHealth > 0)
        {
            float resistance = 0;
            //check if the caster has enough AP to cast a spell - currently redundant
            if (_casterStatus.currentAction >= apCost)
            {
                //use casters ability points
                _casterStatus.CmdLoseAction(apCost);

                //check if the spell effect is reduced by magic resistance or armor
                if (isMagic)
                {
                    magicPen += _casterStatus.tempMagicPen;
                    if (magicPen == 0)
                    {
                        resistance = target.magicArmor + target.tempPhysicalArmor;
                    }
                    else
                    {
                        resistance = (target.magicArmor + target.tempMagicArmor) * (1 - magicPen);
                    }
                    //ensure penetration doesnt add damage and that resistance doesn't cause healing
                    if (resistance < 0)
                    {
                        resistance = 0;
                    }
                    if(resistance >= 1)
                    {
                        resistance = 1;
                    }
                    target.tempMagicArmor -= .1f;
                    _casterStatus.tempMagicPen -= .1f;
                    if (target.tempMagicArmor < 0)
                    {
                        target.tempMagicArmor = 0;
                    }
                    if (_casterStatus.tempMagicPen < 0)
                    {
                        _casterStatus.tempMagicPen = 0;
                    }
                    //deal damage wieghted by resistance
                    target.loseHealth(damage * (1 - resistance));
                }
                else
                {
                    armorPen += _casterStatus.tempArmorPen;
                    if (armorPen == 0)
                    {
                        resistance = target.physicalArmor + target.tempPhysicalArmor;
                    }
                    else
                    {
                        resistance = target.physicalArmor * (1 - armorPen);
                    }

                    //ensure penetration doesnt add damage and that resistance doesn't cause healing
                    if (resistance < 0)
                    {
                        resistance = 0;
                    }
                    if(resistance >= 1)
                    {
                        resistance = 1;
                    }

                    target.tempPhysicalArmor -= .1f;
                    _casterStatus.tempArmorPen -= .1f;
                    if (target.tempPhysicalArmor < 0)
                    {
                        target.tempPhysicalArmor = 0;
                    }
                    if (_casterStatus.tempArmorPen < 0)
                    {
                        _casterStatus.tempArmorPen = 0;
                    }

                    //deal damage weighted by resistance
                    target.loseHealth(damage * (1 - resistance));
                }

                //check if target is dead - may have received damage before healing
                if (target.currentHealth <= 0)
                {
                    target.GetComponent<CapsuleCollider>().enabled = false;
                    target.currentHealth = 0;
                    if (target.isLeader)
                    {
                        _casterStatus.CmdEndGame(target.teamNum);
                    }
                }
                else
                {
                    //if alive heal the target
                    target.gainHealth(healing);
                    if (target.currentHealth > target.maxHealth)
                    {
                        target.currentHealth = target.maxHealth;
                    }
                    if(buff > 0)
                    {
                        if (isMagic)
                        {
                            target.maxAction += buff;
                        }
                        else
                        {
                            target.maxHealth += buff;
                        }
                    }
                }

                //ability is finished set boolean
                usingAbility = false;

            }
            else
            {
                //not enough AP - set boolean
                usingAbility = false;
            }

            //syncs the server values with those of the clinet
            target.CmdSyncValues(target.teamNum, target.maxAction, target.currentAction,
                target.maxHealth, target.currentHealth, target.physicalArmor, target.magicArmor);
        }

    }

    public void toggleMovement()
    {
        audioManager.uiSelected();
        //if an ability is being used cancel the ability
        if (usingAbility)
        {
            _unit.moveToggle = false;
            usingAbility = false;
            _unit.UnhighlightWalkableTiles();
        }

        //otherwise toggle moement on the unity -
        else
        {
            //NOTE - THIS IS A DIFFERENT TOGGLE MOVEMENT FUNCTION BELONGING TO THE UNIT CLASS
            //toggles movement boolean between true or false
            _unit.toggleMovement();
            _unit.HighlightWalkableTiles();
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
                buttonPressed = 1;
                Button1Animation.GetComponentInParent<Button>().onClick.Invoke();

            }
            else if (Input.GetKeyUp(spellHotkey2))
            {
                Button2Animation.GetComponentInParent<Button>().onClick.Invoke();
                buttonPressed = 2;
            }
            else if (Input.GetKeyUp(spellHotkey3))
            {
                Button3Animation.GetComponentInParent<Button>().onClick.Invoke();
                buttonPressed = 3;
            }
            else if (Input.GetKeyUp(spellHotkey4))
            {
                Button4Animation.GetComponentInParent<Button>().onClick.Invoke();
                buttonPressed = 4;
            }
            else if (Input.GetKeyUp(spellHotkey5))
            {
                Button5Animation.GetComponentInParent<Button>().onClick.Invoke();
                buttonPressed = 5;
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
                            //_unit.abil = abilityUsed;

                            if (canCast(hit.collider.gameObject.GetComponent<CharacterStatus>(), abilityUsed) != -1)
                            {
                                //cast an ability
                                if (buttonPressed == 1)
                                {
                                    copyInfo(Button1Animation);
                                    gameObject.GetComponentInParent<CastSpell>().callCast(hit.collider.gameObject.GetComponent<CharacterStatus>(),
                                        abilityUsed, 1);
                                }
                                if (buttonPressed == 2)
                                {
                                    copyInfo(Button2Animation);
                                    gameObject.GetComponentInParent<CastSpell>().callCast(hit.collider.gameObject.GetComponent<CharacterStatus>(),
                                        abilityUsed, 2);
                                }
                                if (buttonPressed == 3)
                                {
                                    copyInfo(Button3Animation);
                                    gameObject.GetComponentInParent<CastSpell>().callCast(hit.collider.gameObject.GetComponent<CharacterStatus>(),
                                        abilityUsed, 3);
                                }
                                if (buttonPressed == 4)
                                {
                                    copyInfo(Button4Animation);
                                    gameObject.GetComponentInParent<CastSpell>().callCast(hit.collider.gameObject.GetComponent<CharacterStatus>(),
                                        abilityUsed, 4);
                                }
                                if (buttonPressed == 5)
                                {
                                    copyInfo(Button5Animation);
                                    gameObject.GetComponentInParent<CastSpell>().callCast(hit.collider.gameObject.GetComponent<CharacterStatus>(),
                                        abilityUsed, 5);
                                }
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
            //set cast indicator to be invisible
            FindObjectOfType<SpellIndicator>().clearList();
            _unit.moveToggle = false;
        }
        else
        {
            //if a unit had an ability selected - set state to deselected
            if (usingAbility)
            {
                _unit.UnhighlightWalkableTiles();
                FindObjectOfType<SpellIndicator>().clearList();
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

    public void copyInfo(CastSpell copiedSpell)
    {
        //spell must be cast from root object so copy values to that root object for casting
        gameObject.GetComponentInParent<CastSpell>().abilityAnimation = copiedSpell.abilityAnimation;
        gameObject.GetComponentInParent<CastSpell>().abilityHitAnimation = copiedSpell.abilityHitAnimation;
        gameObject.GetComponentInParent<CastSpell>().spellMoves = copiedSpell.spellMoves;
    }

    public void setButton(int _buttonPressed)
    {
        buttonPressed = _buttonPressed;
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



    public int canCast(CharacterStatus theTarget, int _abilityNum)
    {
        if (theTarget == null)
        {
            theTarget = _casterStatus;
        }
        //fireball
        if (_abilityNum == 1)
        {
            if (_casterStatus.currentAction > 3 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 3 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 3)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 1;
                    }
                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //flamecircle
        if (_abilityNum == 2)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 3 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 3)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 2;
                    }
                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //healing circle
        if (_abilityNum == 3)
        {
            if (_casterStatus.currentAction > 5 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 3 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 3)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 3;
                    }
                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //cleave
        if (_abilityNum == 4)
        {
            if (_casterStatus.currentAction > 4 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 3 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 3)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 4;
                    }
                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //dark circle
        if (_abilityNum == 5)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 3 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 3)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 5;
                    }
                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //smite
        if (_abilityNum == 6)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 3 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 3)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 1;
                    }
                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //ap buff
        if (_abilityNum == 7)
        {
            if (_casterStatus.currentAction > 5 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 3 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 3)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 2;
                    }
                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //healthbuff
        if (_abilityNum == 8)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 3 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 3)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 2;
                    }
                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //attack 1
        if (_abilityNum == 9)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 7;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //attack 2
        if (_abilityNum == 10)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 8;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                    return -1;
            }
        }
        //attack 3
        if (_abilityNum == 11)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 9;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //attack 1
        if (_abilityNum == 12)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 7;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //attack 2
        if (_abilityNum == 13)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 8;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //attack 3
        if (_abilityNum == 14)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 9;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //attack 1
        if (_abilityNum == 15)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 7;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //attack 2
        if (_abilityNum == 16)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 8;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //attack 3
        if (_abilityNum == 17)
        {
            if (_casterStatus.currentAction > 6 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 1 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 1)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 9;
                    }
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //healing circle
        if (_abilityNum == 18)
        {
            if (_casterStatus.currentAction > 5 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 0)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 3;
                    }
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        //Armor Buff
        if (_abilityNum == 19)
        {
            if (_casterStatus.currentAction > 5 && theTarget.currentHealth > 0 && _casterStatus.currentHealth > 0)
            {
                print(Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX));
                print(Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ));

                print(Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 0);

                if (Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) <= 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) == 0 ||
                    Math.Abs(_unit.tileX - theTarget.GetComponent<Unit>().tileX) == 0 &&
                    Math.Abs(_unit.tileZ - theTarget.GetComponent<Unit>().tileZ) <= 0)
                {
                    if (!rangeCheck)
                    {
                        _unit.abil = 3;
                    }
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return -1;
        }
    }

}
