using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{

    public int tileX;
    public int tileZ;
    public TileMap map;

    void OnMouseUp()
    {
        Unit selectedUnit = map._selectedUnit.GetComponent<Unit>();
        if (selectedUnit.moveToggle)
        {
            //see if this tile is withing the moveable range of the unit
            if (selectedUnit.InRangeOfSelectedTile(tileX, tileZ))
            {
                map.GeneratePathTo(tileX, tileZ);
                selectedUnit.BeginMovement();
                selectedUnit.UnhighlightWalkableTiles();
            }
        }
    }
}
