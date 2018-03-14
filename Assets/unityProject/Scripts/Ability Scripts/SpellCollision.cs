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
    private bool hitParent;
    public bool spellMoves;
    public bool serverSpell;
    public CastSpell _parentCastSpell;

    // Use this for initialization
    void Start()
    {
        if (gameObject.transform.parent != null)
        {
            _parentCastSpell = gameObject.transform.parent.GetComponent<CastSpell>();
        }
        gameObject.transform.parent = null;
        hitOnce = false;
        hitParent = false;
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
        if (hitOnce == false)
        {
            if (!spellMoves)
            {
                if (serverSpell)
                {
                    hitOnce = true;
                    _parentCastSpell.applyAbilityEffect(_parentCastSpell.abilityNum);
                    //gameObject.transform.parent = null;
                }
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
            if (spellMoves)
            {
                if (hitParent == false)
                {
                    hitParent = true;
                    Physics.IgnoreCollision(gameObject.GetComponent<SphereCollider>(), other);
                }
                else
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
                                if (serverSpell)
                                {
                                    CmdSpawn();
                                }
                            }
                        }
                        hitOnce = true;
                    }
                    //destroy the original spell animation to which this script belongs
                    if (serverSpell)
                    {
                        _parentCastSpell.applyAbilityEffect(_parentCastSpell.abilityNum, other.GetComponent<CharacterStatus>());
                    }
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }


}
