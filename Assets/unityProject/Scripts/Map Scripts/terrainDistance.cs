using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainDistance : MonoBehaviour {

    public int detailDistance;
	
	// Update is called once per frame
	void Update () {
        Terrain.activeTerrain.detailObjectDistance = detailDistance;
    }
}
