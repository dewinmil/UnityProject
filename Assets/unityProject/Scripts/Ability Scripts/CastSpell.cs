using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        spellAlive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentAnimation)
        {
            if (spellAlive)
            {
                spellAlive = false;
                if (spellMoves)
                {
                    if (abilityNum == 1)
                    {
                        _caster.ability1(spellTarget);
                    }
                    else if (abilityNum == 2)
                    {
                        _caster.ability2(spellTarget);
                    }
                    else if (abilityNum == 3)
                    {
                        _caster.ability3(spellTarget);
                    }
                    else if (abilityNum == 4)
                    {
                        _caster.ability4(spellTarget);
                    }
                    else if (abilityNum == 5)
                    {
                        _caster.ability5(spellTarget);
                    }
                }

            }
        }
        if (currentAnimation)
        {
            if (currentAnimation.GetComponent<ParticleSystem>().isPlaying == false)
            {
                spellAlive = false;
                GameObject.Destroy(currentAnimation);
                if (spellMoves)
                {
                    if (abilityNum == 1)
                    {
                        _caster.ability1(spellTarget);
                    }
                    else if (abilityNum == 2)
                    {
                        _caster.ability2(spellTarget);
                    }
                    else if (abilityNum == 3)
                    {
                        _caster.ability3(spellTarget);
                    }
                    else if (abilityNum == 4)
                    {
                        _caster.ability4(spellTarget);
                    }
                    else if (abilityNum == 5)
                    {
                        _caster.ability5(spellTarget);
                    }
                }
            }
        }
        if (onHitAnimation)
        {
            if (onHitAnimation.GetComponent<ParticleSystem>().isPlaying == false)
            {
                GameObject.Destroy(onHitAnimation);
            }
        }
    }

    public void Cast(CharacterStatus target, int _abilityNum)
    {
        abilityNum = _abilityNum;
        spellTarget = target;
        spellAlive = true;

        if (spellMoves)
        {
            currentAnimation = Instantiate(abilityAnimation, _caster.transform.position, Quaternion.identity);
            Physics.IgnoreCollision(currentAnimation.GetComponent<Collider>(), gameObject.GetComponentInParent<Collider>());
            float rotation = getRotation(target);
            currentAnimation.transform.eulerAngles = new Vector3(currentAnimation.transform.eulerAngles.x,
                rotation + abilityAnimation.transform.eulerAngles.y, currentAnimation.transform.eulerAngles.z);


            Vector3 targetDirection = (target.transform.position - _caster.transform.position).normalized;

            currentAnimation.GetComponent<Rigidbody>().AddForce(targetDirection * 1000);
        }
        else
        {
            currentAnimation = Instantiate(abilityAnimation, target.transform.position, Quaternion.identity);
            currentAnimation.transform.position = new Vector3(currentAnimation.transform.position.x, 0, currentAnimation.transform.position.z);

            if (abilityNum == 1)
            {
                _caster.ability1(spellTarget);
            }
            else if (abilityNum == 2)
            {
                _caster.ability2(spellTarget);
            }
            else if (abilityNum == 3)
            {
                _caster.ability3(spellTarget);
            }
            else if (abilityNum == 4)
            {
                _caster.ability4(spellTarget);
            }
            else if (abilityNum == 5)
            {
                _caster.ability5(spellTarget);
            }

        }
    }


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

}
