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
    private bool spellAlive;
    private GameObject currentAnimation;
    private GameObject onHitAnimation;
    private CharacterStatus spellTarget;
    private int abilityNum;
    private CharacterStatus target;
    private bool iCast;
    public int buttonNum;

    // Use this for initialization
    void Start()
    {
        //no spell has been cast
        spellAlive = false;
    }

    // Update is called once per frame
    void Update()
    {
        //checks to see whether a spell animation exists in the scene
        if (!currentAnimation)
        {
            //checks whether a spell animation used to exist in the scene
            if (spellAlive)
            {
                //spell animation was destroyed so set to false
                spellAlive = false;

                //check whether this spell was supposed to move towards the target
                //if so it was just arrived and was destroyed / we need to deal
                //the spell effect IE: damage, healing, etc to the target.
                if (spellMoves)
                {
                    applyAbilityEffect(abilityNum);
                }
            }
        }
        //there is currently a spell effect in the scene.
        if (currentAnimation)
        {
            //if the spell effect is no longer playing
            if (currentAnimation.GetComponent<ParticleSystem>().isPlaying == false)
            {
                //the spell effect is dead / complete so set to false and remove it from scene
                spellAlive = false;
                GameObject.Destroy(currentAnimation);

                //if the spell was of the type that moves to and detonates on the target
                //we need to apply the spell effect now
                if (spellMoves)
                {
                    applyAbilityEffect(abilityNum);
                }
            }
        }

        //if an onHitAnimation exists
        if (onHitAnimation)
        {
            //checkif the animation is no longer running / finished
            if (onHitAnimation.GetComponent<ParticleSystem>().isPlaying == false)
            {
                //remove "dead" animation from scene
                GameObject.Destroy(onHitAnimation);
            }
        }
    }

    [Command]
    public void CmdSpawn(bool moves, float rotation, Vector3 targetDirection, Vector3 casterPosition,
        Vector3 targetPosition, Vector3 theEulerAngles, Quaternion identity, int _abilityNum, int _buttonNum)
    {
        GameObject serverAnimation;
        float yAngle;
        if (_buttonNum == 1)
        {
            if (moves)
            {
                serverAnimation = Instantiate(_caster.GetComponent<Abilities>().Button1Animation.abilityAnimation,
                    casterPosition, Quaternion.identity);
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
                    casterPosition, Quaternion.identity);
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
                    casterPosition, Quaternion.identity);
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
                    casterPosition, Quaternion.identity);
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
                    casterPosition, Quaternion.identity);
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
            Physics.IgnoreCollision(serverAnimation.GetComponent<SphereCollider>(), gameObject.GetComponentInParent<CapsuleCollider>());
            serverAnimation.transform.eulerAngles = theEulerAngles;
            serverAnimation.GetComponent<Rigidbody>().AddForce(targetDirection * 1000);
            serverAnimation.transform.position = new Vector3(serverAnimation.transform.position.x, serverAnimation.transform.position.y + (float).5, serverAnimation.transform.position.z);
        }
        else
        {
            serverAnimation.transform.position = new Vector3(serverAnimation.transform.position.x, 0, serverAnimation.transform.position.z);
        }

        //serverAnimation.GetComponent<SpellCollision>().isOriginal = true;
        //Instantiate(_caster.GetComponent<Abilities>().Button1Animation.abilityAnimation, _caster.transform.localPosition, Quaternion.identity);
        serverAnimation.transform.parent = gameObject.transform;
        NetworkServer.Spawn(serverAnimation);

        Ray ray = Camera.main.ScreenPointToRay(casterPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.tag == "Unit") 
            {
                //serverAnimation.transform.parent = hit.collider.gameObject.transform;
                //gameObject.transform.parent = hit.collider.gameObject.transform;
                CastSpell temp = gameObject.GetComponent<CastSpell>();
                temp = hit.collider.gameObject.GetComponent<CastSpell>();
                temp._caster = hit.collider.gameObject.GetComponent<Abilities>();
            }
        }
        ray = Camera.main.ScreenPointToRay(targetPosition);
        print("before raycast");
        if (Physics.Raycast(ray, out hit, 100))
        {
            print("raycast");
            if (hit.collider.tag == "Unit")
            {
                print("tagged as unit");
                //serverAnimation.transform.parent = hit.collider.gameObject.transform;
                //gameObject.transform.parent = hit.collider.gameObject.transform;
                spellTarget = hit.collider.gameObject.GetComponent<CharacterStatus>();
                //CastSpell temp = gameObject.GetComponent<CastSpell>();
                //temp = hit.collider.gameObject.GetComponent<CastSpell>();
                //temp.spellTarget = hit.collider.gameObject.GetComponent<CharacterStatus>();
            }
        }


        applyAbilityEffect(_abilityNum);
    }

    public void callCast(CharacterStatus theTarget, int _abilityNum, int _buttonNum)
    {
        buttonNum = _buttonNum;
        target = theTarget;
        cast(_abilityNum);
    }


    /*
    public void cast(int _abilityNum)
    {
        abilityNum = _abilityNum;
        spellTarget = target;

        //check if spell moves towards and applies effect upon reaching target
        if (spellMoves)
        {
            //check if we have enough ap to cast the ability
            if (canCast(abilityNum))
            {

                spellAlive = true;
                //create spell effect on the castors position and ignore collisions with the caster so it does not instantly detonate
                currentAnimation = Instantiate(abilityAnimation, _caster.transform.localPosition, Quaternion.identity);
                currentAnimation.GetComponent<SpellCollision>().abilityUsed = _abilityNum;
                Physics.IgnoreCollision(currentAnimation.GetComponent<SphereCollider>(), gameObject.GetComponentInParent<CapsuleCollider>());

                //rotate the spell in the direction of the target
                float rotation = getRotation(target);
                currentAnimation.transform.eulerAngles = new Vector3(currentAnimation.transform.eulerAngles.x,
                    rotation + abilityAnimation.transform.eulerAngles.y, currentAnimation.transform.eulerAngles.z);

                //find the direction of target
                Vector3 targetDirection = (target.transform.position - _caster.transform.position).normalized;

                //accelerate the spell animation towards the target
                currentAnimation.GetComponent<Rigidbody>().AddForce(targetDirection * 1000);

                if (gameObject.GetComponent<NetworkIdentity>().isServer)
                {
                    print("does think it is host");
                    NetworkServer.Spawn(currentAnimation);
                }
                else
                {
                    print("Doesn't think it is host");
                    CmdSpawn();
                }
                currentAnimation.GetComponent<SpellCollision>().isOriginal = true;
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
                    //create spell effect on the target location
                    currentAnimation = Instantiate(abilityAnimation, target.transform.position, Quaternion.identity);
                    currentAnimation.transform.position = new Vector3(currentAnimation.transform.position.x, 0, currentAnimation.transform.position.z);
                    currentAnimation.GetComponent<SpellCollision>().abilityUsed = abilityNum;

                    if (gameObject.GetComponent<NetworkIdentity>().isServer)
                    {
                        print("does think it is host");
                        NetworkServer.Spawn(currentAnimation);
                    }
                    else
                    {
                        print("Doesn't think it is host");
                        CmdSpawn();
                    }
                    currentAnimation.GetComponent<SpellCollision>().isOriginal = true;
                    //as the spell is at the target immediately apply appropriate spell effect
                    //from the abilities class

                    applyAbilityEffect(abilityNum);
                }
            }
        }
    }
    */

    public void cast(int _abilityNum)
    {
        abilityNum = _abilityNum;
        spellTarget = target;

        //check if spell moves towards and applies effect upon reaching target
        if (spellMoves)
        {
            //check if we have enough ap to cast the ability
            if (canCast(abilityNum))
            {

                spellAlive = true;

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
    public void applyAbilityEffect(int abilityUsed)
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(targetPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.tag == "Unit")
            {
                print("set spell target");
                spellTarget = hit.collider.gameObject.GetComponent<CharacterStatus>();
            }
        }
        */
        if (abilityUsed == 1)
        {
            _caster.castAbility(spellTarget, 3, 0, 3, 0, (float).5, 0, true);
        }
        else if (abilityUsed == 2)
        {
            _caster.castAbility(spellTarget, 7, 0, 6, 0, 0, 0, true);
        }
        else if (abilityUsed == 3)
        {
            _caster.castAbility(spellTarget, 0, 5, 5, 0, 0, 0, true);
        }
        else if (abilityUsed == 4)
        {
            _caster.castAbility(spellTarget, 5, 0, 4, (float).25, 0, 0, false);
        }
        else if (abilityUsed == 5)
        {
            _caster.castAbility(spellTarget, 5, 0, 6, 0, 1, 0, true);
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
        else
        {
            return false;
        }
    }

}
