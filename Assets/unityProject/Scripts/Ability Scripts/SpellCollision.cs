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
    private bool hitOnce;

    // Use this for initialization
    void Start()
    {
        hitOnce = false;
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

    [Command]
    public void CmdSpawn()
    {
        if (gameObject.GetComponent<NetworkIdentity>().isServer)
        {
            NetworkServer.Spawn(Instantiate(onHitAnimation, transform.position, Quaternion.identity));
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
                    if (gameObject.GetComponent<NetworkIdentity>().isServer)
                    {
                        NetworkServer.Spawn(Instantiate(onHitAnimation, transform.position, Quaternion.identity));
                    }
                    else
                    {
                        CmdSpawn();
                    }

                }
                hitOnce = true;
            }
            //destroy the original spell animation to which this script belongs
            GameObject.Destroy(gameObject);

        }
    }


}

/*
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
    private bool hitOnce;
    private bool hitCaster;
    public int abilityUsed;
    private bool deltEffect;
    public bool isOriginal;

    // Use this for initialization
    void Start()
    {
        hitCaster = false;
        deltEffect = false;
        hitOnce = false;
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
        //print("loops");
        if (deltEffect == false)
        {
            if (gameObject.GetComponent<Collider>().Equals(null) && isOriginal == true)
            {
                deltEffect = true;
                //DO NOT WANT ON HIT TO CALL THIS
                //gameObject.transform.parent.GetComponent<CastSpell>().applyAbilityEffect(abilityUsed);
            }
        }

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

    [Command]
    public void CmdSpawn()
    {
        if (gameObject.GetComponent<NetworkIdentity>().isServer)
        {
            NetworkServer.Spawn(Instantiate(onHitAnimation, transform.position, Quaternion.identity));
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
                if (isOriginal == true)
                {
                    //gameObject.transform.parent.GetComponent<CastSpell>().applyAbilityEffect(abilityUsed);

                    if (onHitAnimation)
                    {
                        //create the hit animation on top of the target
                        //NetworkServer.Spawn(Instantiate(onHitAnimation, transform.position, Quaternion.identity));
                        if (gameObject.GetComponent<NetworkIdentity>().isServer)
                        {
                            NetworkServer.Spawn(Instantiate(onHitAnimation, transform.position, Quaternion.identity));
                        }
                        else
                        {
                            CmdSpawn();
                        }
                    }
                    hitOnce = true;
                }
                else
                {
                    if(hitCaster == true)
                    {
                        //gameObject.transform.parent.GetComponent<CastSpell>().applyAbilityEffect(abilityUsed);

                        if (onHitAnimation)
                        {
                            //create the hit animation on top of the target
                            Instantiate(onHitAnimation, transform.position, Quaternion.identity);
                        }
                        hitOnce = true;
                    }
                }

            }
            if (isOriginal == false)
            {
                if (hitCaster == false)
                {
                    Physics.IgnoreCollision(gameObject.GetComponent<SphereCollider>(), other.GetComponent<CapsuleCollider>());
                    hitCaster = true;
                }
                else
                {
                    GameObject.Destroy(gameObject);
                }
            }
            else
            {
                GameObject.Destroy(gameObject);
            }            
        }
    }
}
*/
