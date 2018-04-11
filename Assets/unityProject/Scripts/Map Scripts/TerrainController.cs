using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour {
    public TileMap _map;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        if (_map.charSelect == true)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, -.005f, gameObject.transform.position.z);
        }
        else
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, .005f, gameObject.transform.position.z);
        }

    }
}
