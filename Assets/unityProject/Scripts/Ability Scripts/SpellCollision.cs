using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCollision : MonoBehaviour {

    public GameObject onHitAnimation;
    private bool hitCaster;

	// Use this for initialization
	void Start () {
        hitCaster = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(this.GetComponent<ParticleSystem>().isPlaying == false)
        {
            GameObject.Destroy(this);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (hitCaster)
        {
            if (onHitAnimation)
            {
                Instantiate(onHitAnimation, transform.position, Quaternion.identity);
            }
            GameObject.Destroy(gameObject);
        }
        else
        {
            hitCaster = true;
        }
    }

 
}
