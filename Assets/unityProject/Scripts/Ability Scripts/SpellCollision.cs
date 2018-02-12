using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCollision : MonoBehaviour
{

    public GameObject onHitAnimation;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if the spell animation is no longer playing destroy it
        if (this.GetComponent<ParticleSystem>().isPlaying == false)
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the spell animation collided with a Unit
        if (other.tag == "Unit")
        {
            //if the script has a second animation for when it hits the target
            if (onHitAnimation)
            {
                //create the hit animation on top of the target
                Instantiate(onHitAnimation, transform.position, Quaternion.identity);
            }

            //destroy the original spell animation to which this script belongs
            GameObject.Destroy(gameObject);
        }
    }


}
