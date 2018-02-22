using System;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Networking;

public class SpellCollision : NetworkBehaviour
{

    public GameObject onHitAnimation;
    private AudioSource source;
    public AudioClip spellSound;
    private float maxVolume;
    private float timeToDie;
    private bool timeToKill;
    private bool hitOnce;

    // Use this for initialization
    void Start()
    {
        hitOnce = false;
        timeToKill = false;
        source = gameObject.AddComponent<AudioSource>();
        source.clip = spellSound;
        source.volume = (float).75;
        maxVolume = source.volume;
        source.loop = true;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {

        if (source.clip.length - source.time <= 1)
        {
            source.volume = maxVolume * (source.clip.length - source.time);
        }
        //if the spell animation is no longer playing destroy it
        if (this.GetComponent<ParticleSystem>().isPlaying == false)
        {
            GameObject.Destroy(gameObject);
        }

        if (source.clip.length - source.time > 1)
        {
            if (source.time <= 1)
            {
                source.volume = maxVolume * source.time;
            }
            else
            {
                source.volume = maxVolume;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //if the spell animation collided with a Unit
        if (other.tag == "Unit")
        {
            //if the script has a second animation for when it hits the target

            if (hitOnce == false)
            {
                if (onHitAnimation)
                {
                    //create the hit animation on top of the target
                    NetworkServer.Spawn(Instantiate(onHitAnimation, transform.position, Quaternion.identity));

                }
                hitOnce = true;
            }
            //destroy the original spell animation to which this script belongs
            if (source.isPlaying)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }


}
