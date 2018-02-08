using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSpell : MonoBehaviour {

    public GameObject abilityAnimation;
    public GameObject abilityHitAnimation;
    public Abilities _caster;
    public bool spellMoves;
    private GameObject currentAnimation;
    private GameObject onHitAnimation;
    private CharacterStatus targetGlobal;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (currentAnimation)
        {
            if (currentAnimation.GetComponent<ParticleSystem>().isPlaying == false)
            {
                GameObject.Destroy(currentAnimation);
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

    public void Cast(CharacterStatus target)
    {
        targetGlobal = target;


        if (spellMoves)
        {
            currentAnimation = Instantiate(abilityAnimation, _caster.transform.position, Quaternion.identity);
            float rotation = getRotation(target);
            currentAnimation.transform.eulerAngles = new Vector3(currentAnimation.transform.eulerAngles.x,
                rotation+ abilityAnimation.transform.eulerAngles.y, currentAnimation.transform.eulerAngles.z);


            Vector3 targetDirection = (target.transform.position - _caster.transform.position).normalized;

            currentAnimation.GetComponent<Rigidbody>().AddForce(targetDirection * 1000);
        }
        else
        {
            currentAnimation = Instantiate(abilityAnimation, target.transform.position, Quaternion.identity);
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
