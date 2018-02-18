using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A class that instantiates a spell animation and directs it to the target
 * before calling the abilities class to deal the spell effect (damage,etc)
 * to the target
 */
public class CastSpell : MonoBehaviour
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

    public void Cast(CharacterStatus target, int _abilityNum)
    {
        abilityNum = _abilityNum;
        spellTarget = target;

        //check if spell moves towards and applies effect upon reaching target
        if (spellMoves)
        {
            //check if we have enough ap to cast the ability
            if (canCast(abilityNum))
            {

                //create spell effect on the castors position and ignore collisions with the caster so it does not instantly detonate
                Vector3 startPos = new Vector3(_caster.transform.position.x, _caster.transform.position.y + (float).5, _caster.transform.position.z);
                currentAnimation = Instantiate(abilityAnimation, _caster.transform.position, Quaternion.identity);
                Physics.IgnoreCollision(currentAnimation.GetComponent<Collider>(), gameObject.GetComponentInParent<Collider>());

                //rotate the spell in the direction of the target
                float rotation = getRotation(target);
                currentAnimation.transform.eulerAngles = new Vector3(currentAnimation.transform.eulerAngles.x,
                    rotation + abilityAnimation.transform.eulerAngles.y, currentAnimation.transform.eulerAngles.z);

                //find the direction of target
                Vector3 targetDirection = (target.transform.position - _caster.transform.position).normalized;

                //accelerate the spell animation towards the target
                currentAnimation.GetComponent<Rigidbody>().AddForce(targetDirection * 1000);
            }
        }
        //this spell appears on the targets location
        else
        {
            //check if we have enough ap to cast the ability
            if (canCast(abilityNum))
            {

                //create spell effect on the target location
                currentAnimation = Instantiate(abilityAnimation, target.transform.position, Quaternion.identity);
                currentAnimation.transform.position = new Vector3(currentAnimation.transform.position.x, 0, currentAnimation.transform.position.z);

                //as the spell is at the target immediately apply appropriate spell effect
                //from the abilities class

                applyAbilityEffect(abilityNum);
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

            if (_caster._casterStatus.currentAction > 3 && spellTarget.currentHealth > 0 &&_caster._casterStatus.currentHealth > 0)

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
