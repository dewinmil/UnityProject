using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSpawnPoints : MonoBehaviour
{

    private GameObject[] spawnPoints;
	// Use this for initialization
	void Start ()
	{
	    spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
	}

    private void MoveSpawnPointsOverTiles()
    {
        
    }
}
