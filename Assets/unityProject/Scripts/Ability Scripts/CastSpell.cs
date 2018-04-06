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
    public bool spellMoves;
    private GameObject currentAnimation;
    private GameObject onHitAnimation;
    private CharacterStatus spellTarget;
    [SyncVar]
    public int abilityNum;
    private CharacterStatus target;
    private bool iCast;
    public int buttonNum;
    private bool deltEffect;

    // Use this for initialization
    void Start()
    {
        
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
        if (_buttonNum == 1)
        {
            if (moves)
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button1Animation.abilityAnimation,
                    moveSpellPostion, Quaternion.identity);
            }
            else
            {
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
            serverAnimation.transform.eulerAngles = theEulerAngles;
            serverAnimation.GetComponent<Rigidbody>().AddForce(targetDirection * 1000);
        }
        else
        {
            serverAnimation.transform.position = new Vector3(serverAnimation.transform.position.x, 0, serverAnimation.transform.position.z);
        }
        serverAnimation.transform.parent = gameObject.transform;

        //don't cast a moving spell onto self
        if(moves && Vector3.Distance(casterPosition, targetPosition) <= .5)
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

        Collider[] c = Physics.OverlapSphere(targetPosition, 1f);
        foreach (var collider in c)
        {
            var obj = collider.gameObject; //This is the game object you collided with
            if (obj == gameObject) continue; //Skip the object itself
            if(obj.tag == "Unit")
            {
                spellTarget = obj.GetComponent<CharacterStatus>();
            }
        }

    }

    public void callCast(CharacterStatus theTarget, int _abilityNum, int _buttonNum)
    {
        buttonNum = _buttonNum;
        target = theTarget;
        inRange(theTarget, _abilityNum);

    }

    public void cast(int _abilityNum)
    {
        abilityNum = _abilityNum;
        spellTarget = target;

        //check if spell moves towards and applies effect upon reaching target
        if (spellMoves)
        {
            //check if we have enough ap to cast the ability
            if (canCast(_abilityNum))
            {
                //rotate the spell in the direction of the target
                float rotation = getRotation(target);

                //find the direction of target
                currentAnimation = Instantiate(abilityAnimation, target.transform.position, Quaternion.identity);
                Vector3 targetDirection = (target.transform.position - _caster.transform.position).normalized;
                Vector3 theEulerAngles = new Vector3(currentAnimation.transform.eulerAngles.x,
                    rotation + abilityAnimation.transform.eulerAngles.y, currentAnimation.transform.eulerAngles.z);
                GameObject.Destroy(currentAnimation);
                CmdSpawn(spellMoves, rotation, targetDirection, _caster.transform.position, target.transform.position, theEulerAngles, Quaternion.identity, _abilityNum, buttonNum);
            }
        }
        //this spell appears on the targets location
        else
        {
            //check if we have enough ap to cast the ability
            if (_caster.Equals(null))
            {
                //do nothing
            }
            else
            {
                if (canCast(abilityNum))
                {
                    Vector3 empty = new Vector3(0, 0, 0);
                    CmdSpawn(spellMoves, 0, empty, _caster.transform.position, target.transform.position, empty, Quaternion.identity, _abilityNum, buttonNum);
                }
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

    //applies the spell effect to the target
    public void applyAbilityEffect(int abilityUsed, CharacterStatus targetCharacterStatus = null)
    {
        if(targetCharacterStatus == null)
        {
            targetCharacterStatus = spellTarget;
        }
        if (abilityUsed == 1)
        {
            _caster.castAbility(targetCharacterStatus, 3, 0, 3, 0, (float).5, 0, true);
        }
        else if (abilityUsed == 2)
        {
            _caster.castAbility(targetCharacterStatus, 7, 0, 6, 0, 0, 0, true);
        }
        else if (abilityUsed == 3)
        {
            _caster.castAbility(targetCharacterStatus, 0, 5, 5, 0, 0, 0, true);
        }
        else if (abilityUsed == 4)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 4, (float).25, 0, 0, false);
        }
        else if (abilityUsed == 5)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 6, 0, 1, 0, true);
        }
        else if (abilityUsed == 6)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 6, 0, 1, 0, true);
        }
        else if (abilityUsed == 7)
        {
            _caster.castAbility(targetCharacterStatus, 4, 0, 5, .25f, .25f, 0, true);
        }
        else if (abilityUsed == 8)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 6, 0, 1, 0, true);
        }
        else if (abilityUsed == 9)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 6, 0, 1, 0, true);
        }
        else if (abilityUsed == 10)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 6, 0, 1, 0, true);
        }
        else if (abilityUsed == 11)
        {
            _caster.castAbility(targetCharacterStatus, 5, 0, 6, 0, 1, 0, true);
        }
    }

    //determines if the caster has enough ability points to cast a spell
    public bool canCast(int abilityUsed)
    {
        if (abilityUsed == 1)
        {
            if (_caster._casterStatus.currentAction > 3 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)

            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 2)
        {
            if (_caster._casterStatus.currentAction > 6 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 3)
        {

            if (_caster._casterStatus.currentAction > 5 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 4)
        {

            if (_caster._casterStatus.currentAction > 4 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 5)
        {

            if (_caster._casterStatus.currentAction > 6 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 6)
        {

            if (_caster._casterStatus.currentAction > 6 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 7)
        {

            if (_caster._casterStatus.currentAction > 6 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 8)
        {

            if (_caster._casterStatus.currentAction > 6 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 9)
        {

            if (_caster._casterStatus.currentAction > 6 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 10)
        {

            if (_caster._casterStatus.currentAction > 6 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (abilityUsed == 11)
        {

            if (_caster._casterStatus.currentAction > 6 && spellTarget.currentHealth > 0 && _caster._casterStatus.currentHealth > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void inRange(CharacterStatus theTarget, int _abilityNum)
    {
        print(Vector3.Distance(gameObject.transform.position, theTarget.transform.position));
        if (_abilityNum == 1)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 12)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 2)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 3)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 4)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 9)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 5)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 6)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 7)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 8)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 9)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 10)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
        if (_abilityNum == 11)
        {
            if (Vector3.Distance(gameObject.transform.position, theTarget.transform.position) < 7)
            {
                cast(_abilityNum);
            }
        }
    }
}
