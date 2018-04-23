using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableTile : MonoBehaviour
{

    public int tileX;
    public int tileZ;
    public Ray ray;
    public TileMap map;
    //the unit occupying this tile
    public GameObject _occupyingUnit;

    void OnMouseUp()
    {
        Unit selectedUnit = map._selectedUnit.GetComponent<Unit>();
        if (selectedUnit.moveToggle && EventSystem.current.IsPointerOverGameObject() == false)
        {
            //see if this tile is withing the moveable range of the unit
            if (selectedUnit.InRangeOfSelectedTile(tileX, tileZ))
            {
                map.GeneratePathTo(tileX, tileZ);
                selectedUnit.BeginMovement();
                selectedUnit.UnhighlightWalkableTiles();
            }
        }
        else if (_occupyingUnit != null)
        {
            MoveInput mi = _occupyingUnit.GetComponent<MoveInput>();
            Unit unit = _occupyingUnit.GetComponent<Unit>();
            if (mi.targetedBySpell == false)
            {
                if (mi.endTurn.turn == _occupyingUnit.GetComponent<CharacterStatus>().teamNum
                    && _occupyingUnit.GetComponent<CharacterStatus>().currentHealth > 0)
                {
                    //select the unit
                    mi.isSelected = true;
                    unit.SelectedUnitChanged();
                    unit._map.charSelect = true;
                }
            }
        }

    }
}
