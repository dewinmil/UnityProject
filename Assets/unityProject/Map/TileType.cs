using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//have to add this so that it will appear in Unity (muist be done for all 'custom' classes)
[System.Serializable]
//notice the class does NOT inherit from Monobehavior
public class TileType
{

    public string Name;
    public GameObject TileVisuallPrefab;
    public bool IsWalkable;
}
