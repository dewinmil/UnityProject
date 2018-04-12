using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * A class that instantiates a spell animation and directs it to the target
 * before calling the abilities class to deal the spell effect (damage,etc)
 * to the target
 */
public class CastSpell : NetworkBehaviour
{

    public GameObject abilityAnimation;
    public GameObject abilityHitAnimation;
    public Abilities _caster;
    CharacterStatus _casterStatus;
    public bool spellMoves;
    private GameObject currentAnimation;
    private CharacterStatus spellTarget;
    [SyncVar]
    public int abilityNum;
    private CharacterStatus target;
    public int buttonNum;

    // Use this for initialization
    void Start()
    {
        _casterStatus = _caster.GetComponentInParent<CharacterStatus>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Command]
    public void CmdSpawn(bool moves, float rotation, Vector3 targetDirection, Vector3 casterPosition,
        Vector3 targetPosition, Vector3 theEulerAngles, Quaternion identity, int _abilityNum, int _buttonNum)
    {
        GameObject serverAnimation;
        float yAngle;
        Vector3 moveSpellPostion = new Vector3(casterPosition.x, casterPosition.y + (float).5, casterPosition.z);
        spellMoves = moves;
        abilityNum = _abilityNum;

        //case for each button / unfortunantly cannot reduce into single function
        if (_buttonNum == 1)
        {
            //check if spell moves from caster to target
            if (moves)
            {
                //create spell animation
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button1Animation.abilityAnimation,
                    moveSpellPostion, Quaternion.identity);
            }
            else
            {
                //create spell animation
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button1Animation.abilityAnimation,
                    targetPosition, Quaternion.identity);
            }
            yAngle = _caster.GetComponent<Abilities>().Button1Animation.abilityAnimation.transform.eulerAngles.y;
        }
        else if (_buttonNum == 2)
        {
            if (moves)
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button2Animation.abilityAnimation,
                    moveSpellPostion, Quaternion.identity);
            }
            else
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button2Animation.abilityAnimation,
                    targetPosition, Quaternion.identity);
            }
            yAngle = _caster.GetComponent<Abilities>().Button1Animation.abilityAnimation.transform.eulerAngles.y;
        }
        else if (_buttonNum == 3)
        {
            if (moves)
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button3Animation.abilityAnimation,
                    moveSpellPostion, Quaternion.identity);
            }
            else
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button3Animation.abilityAnimation,
                    targetPosition, Quaternion.identity);
            }
            yAngle = _caster.GetComponent<Abilities>().Button1Animation.abilityAnimation.transform.eulerAngles.y;
        }
        else if (_buttonNum == 4)
        {
            if (moves)
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button4Animation.abilityAnimation,
                    moveSpellPostion, Quaternion.identity);
            }
            else
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button4Animation.abilityAnimation,
                    targetPosition, Quaternion.identity);
            }
            yAngle = _caster.GetComponent<Abilities>().Button1Animation.abilityAnimation.transform.eulerAngles.y;
        }
        else
        {
            if (moves)
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button5Animation.abilityAnimation,
                    moveSpellPostion, Quaternion.identity);
            }
            else
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button5Animation.abilityAnimation,
                    targetPosition, Quaternion.identity);
            }
            yAngle = _caster.GetComponent<Abilities>().Button1Animation.abilityAnimation.transform.eulerAngles.y;
        }
        if (moves)
        {
            //if the spell moves rotate spell and add force in appropriate direction
            serverAnimation.transform.eulerAngles = theEulerAngles;
            serverAnimation.GetComponent<Rigidbody>().AddForce(targetDirection * 1000);
        }
        else
        {
            //if spell does not move set it's position to that of the target
            serverAnimation.transform.position = new Vector3(serverAnimation.transform.position.x, -.15f, serverAnimation.transform.position.z);
        }
        serverAnimation.transform.parent = gameObject.transform;

        //don't cast a moving spell onto self
        if (moves && Vector3.Distance(casterPosition, targetPosition) <= .5)
        {
            //don't want to cast
            GameObject.Destroy(serverAnimation);
        }
        else
        {
            //serverAnimation.GetComponent<SpellCollision>()._parentCastSpell = gameObject.GetComponent<CastSpell>();
            NetworkServer.Spawn(serverAnimation);
            serverAnimation.GetComponent<SpellCollision>().serverSpell = true;
        }

        //get character status of target so we can apply spell effect - redundant
        Collider[] c = Physics.OverlapSphere(targetPosition, 1f);
        foreach (var collider in c)
        {
            var obj = collider.gameObject; //This is the game object you collided with
            if (obj == gameObject) continue; //Skip the object itself
            if (obj.tag == "Unit")
            {
                spellTarget = obj.GetComponent<CharacterStatus>();
            }
        }

    }

    public void callCast(CharacterStatus theTarget, int _abilityNum, int _buttonNum)
    {
        buttonNum = _buttonNum;
        target = theTarget;
        if (_caster.Equals(null))
        {
            //do nothing
        }
        else
        {
            gameObject.GetComponentInParent<CastSpell>().cast(_abilityNum);
        }

    }

    public void cast(int _abilityNum)
    {
        abilityNum = _abilityNum;
        spellTarget = target;

        //check if spell moves towards and applies effect upon reaching target
        if (spellMoves)
        {
            //rotate the spell in the direction of the target
            float rotation = getRotation(target);

            //find the direction of target
            currentAnimation = Instantiate(abilityAnimation, target.transform.position, Quaternion.identity);
            Vector3 targetDirection = (target.transform.position - _caster.transform.position).normalized;
            Vector3 theEulerAngles = new Vector3(currentAnimation.transform.eulerAngles.x,
                rotation + abilityAnimation.transform.eulerAngles.y, currentAnimation.transform.eulerAngles.z);
            GameObject.Destroy(currentAnimation);

            //have server spawn spell into scene
            CmdSpawn(spellMoves, rotation, targetDirection, _caster.transform.position, target.transform.position, theEulerAngles, Quaternion.identity, _abilityNum, buttonNum);
        }
        //this spell appears on the targets location
        else
        {
            if (_caster.Equals(null))
            {
                //do nothing
            }
            else
            {
                Vector3 empty = new Vector3(0, 0, 0);
                //have server spawn spell into scene
                CmdSpawn(spellMoves, 0, empty, _caster.transform.position, target.transform.position, empty, Quaternion.identity, _abilityNum, buttonNum);
            }
        }
    }

    //finds the direction pointing from the caster to the target.
    public float getRotation(CharacterStatus target)
    {
        float zVal = target.transform.position.z - _caster.transform.position.z;
        float xVal = target.transform.position.x - _caster.transform.position.x;
        float rotation;
        float difference;
        rotation = 0;

        if(xVal == 0)
        {
            xVal = .1f;
        }
        if (zVal == 0)
        {
            zVal = .1f;
        }
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

        return rotation;

    }

    //applies the spell effect to the target - this is where we "create spells" though the particle effect must be set in the
    //editor
    public void applyAbilityEffect(int abilityUsed, CharacterStatus targetCharacterStatus = null)
    {
        if (targetCharacterStatus == null)
        {
            targetCharacterStatus = spellTarget;
        }
        //fireball
        if (abilityUsed == 1)
        {
            _caster.castAbility(targetCharacterStatus, 3, 0, 3, 0, .2f, 0, true);
        }
        //flame circle
        else if (abilityUsed == 2)
        {
            _caster.castAbility(targetCharacterStatus, 4, 0, 5, 0, 0, .5f, true);
        }
        //healing circle
        else if (abilityUsed == 3)
        {
            _caster.castAbility(targetCharacterStatus, 0, 5, 5, 0, 0, 0, true);
        }
        //shockwave
        else if (abilityUsed == 4)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 4, (float).25, 0, 0, false);
        }
        //dark circle
        else if (abilityUsed == 5)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 7, 0, .8f, 0, true);
        }
        //smite
        else if (abilityUsed == 6)
        {
            _caster.castAbility(targetCharacterStatus, 3, 0, 4, 0, .25f, 0, true);
        }
        //ap buff
        else if (abilityUsed == 7)
        {
            _caster.castAbility(targetCharacterStatus, 0, 0, 12, 0, 0, 5, true);
        }
        //health buff
        else if (abilityUsed == 8)
        {
            _caster.castAbility(targetCharacterStatus, 0, 0, 12, 0, 0, 5, false);
        }
        //knight attack1
        else if (abilityUsed == 9)
        {
            _caster.castAbility(targetCharacterStatus, 8, 0, 3, 0, 0, 0, false);
        }
        //knight attack2
        else if (abilityUsed == 10)
        {
            _caster.castAbility(targetCharacterStatus, 8, 0, 5, .5f, 0, 0, false);
        }
        //knight attack3
        else if (abilityUsed == 11)
        {
            _caster.castAbility(targetCharacterStatus, 8, 0, 8, .8f, 0, 0, false);
        }
        //spearman attack1
        else if (abilityUsed == 12)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 3, 0, 0, 0, false);
        }
        //spearman attack2
        else if (abilityUsed == 13)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 5, .2f, 0, 0, false);
        }
        //spearman attack3
        else if (abilityUsed == 14)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 7, .4f, 0, 0, false);
        }
        //warrior attack3
        else if (abilityUsed == 15)
        {
            _caster.castAbility(targetCharacterStatus, 4, 0, 3, 0, 0, 0, true);
        }
        //warrior attack3
        else if (abilityUsed == 16)
        {
            _caster.castAbility(targetCharacterStatus, 4, 0, 5, .1f, 0, 0, true);
        }
        //warrior attack3
        else if (abilityUsed == 17)
        {
            _caster.castAbility(targetCharacterStatus, 4, 0, 6, .3f, 0, 0, true);
        }
        //self heal
        else if (abilityUsed == 18)
        {
            _caster.castAbility(targetCharacterStatus, 0, 5, 5, 0, 0, 0, true);
        }
        //full armor buff
        else if (abilityUsed == 19)
        {
            if (_casterStatus.currentAction >= 8 && _casterStatus.currentHealth > 0)
            {
                if (_casterStatus.tempMagicArmor < .3f || _casterStatus.tempPhysicalArmor < .3f)
                {
                    _casterStatus.currentAction -= 8;
                    _casterStatus.tempPhysicalArmor = .3f;
                    _casterStatus.tempMagicArmor = .3f;
                }
            }
        }
        //armor pen buff
        else if (abilityUsed == 20)
        {
            if (_casterStatus.currentAction >= 5 && _casterStatus.currentHealth > 0)
            {
                if (_casterStatus.tempArmorPen < .3f)
                {
                    _casterStatus.currentAction -= 5;
                    _casterStatus.tempArmorPen = .3f;
                }
            }
        }
        //keep moving
        else if (abilityUsed == 21)
        {
            if (_casterStatus.currentAction >= 3 && _casterStatus.currentHealth > 0)
            {
                _casterStatus.currentAction -= 3;
                _casterStatus._numMovesRemaining += 3;
            }
        }
        //take a rest
        else if (abilityUsed == 22)
        {
            if(_casterStatus._numMovesRemaining > 0 && _casterStatus.currentAction < _casterStatus.maxAction)
            {
                _casterStatus.currentAction += _casterStatus._numMovesRemaining;
                _casterStatus._numMovesRemaining = 0;
                if(_casterStatus.currentAction > _casterStatus.maxAction)
                {
                    _casterStatus.currentAction = _casterStatus.maxAction;
                }
            }
        }
        //death march
        else if (abilityUsed == 23)
        {
            if (_casterStatus.currentHealth > 5)
            {
                _casterStatus.currentHealth -= 5;
                _casterStatus._numMovesRemaining += 5;
            }
        }
    }
}
