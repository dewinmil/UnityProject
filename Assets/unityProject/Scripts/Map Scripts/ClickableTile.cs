﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{

    public int tileX;
    public int tileZ;
    public TileMap map;

    void OnMouseUp()
    {
        map.GeneratePathTo(tileX, tileZ);
        map._selectedUnit.GetComponent<Unit>().BeginMovement();
    }
}
