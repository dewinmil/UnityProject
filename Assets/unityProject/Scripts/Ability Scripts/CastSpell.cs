using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSpell : MonoBehaviour {

    public GameObject ability;
    private GameObject current;
    //public prefab ability;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (current)
        {
            if (current.GetComponent<ParticleSystem>().isPlaying == false)
            {
                GameObject.Destroy(current);
            }
        }
    }

    public void Cast()
    {
        current = Instantiate(ability, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
