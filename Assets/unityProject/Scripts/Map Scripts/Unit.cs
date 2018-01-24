using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{

    public int tileX;
    public int tileZ;
    public TileMap map;
    private bool isMoving;
    public MoveInput _characterMoveInput;
    public bool moveToggle;

    //variable to make the unit walk slower
    private int _waitCount = 0;

    public List<Node> currentPath = null;

    void Update()
    {
        _waitCount++;
        //if the character is set to move, move it
        //unit will only 'walk' every 15 frames
        //this probably isn't a good way to do it, since framerate will depend on the computer 
        //best way would be to use Time.DeltaTime I believe, but that can be implemented later
        if (currentPath != null && isMoving && (_waitCount % 15 == 0))
        {
            MoveUnitToTarget();
        }
    }

    private void MoveUnitToTarget()
    {
        if (_characterMoveInput.isSelected)
        {
            int currNode = 0;

            while (currNode < currentPath.Count - 1)
            {
                MoveToNextTile();
                currNode++;
            }

            //if we get in this, we know we are at our destination
            if (currentPath.Count == 1)
            {
                currentPath = null;
                isMoving = false;
                _waitCount = 0;
            }
        }
    }

    private void MoveToNextTile()
    {
        if (currentPath == null)
            return;

        //remove the old current/first node from the path
        currentPath.RemoveAt(0);

        //grab the new first node and move to that position
        this.transform.position = map.TileCoordToWorldCoord(currentPath[0].x, currentPath[0].z);
        this.tileX = currentPath[0].x;
        this.tileZ = currentPath[0].z;
    }

    public void BeginMovement()
    {
        if (moveToggle)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                if (currentPath == null)
                    return;

                isMoving = true;
            }
        }
    }

    public void toggleMovement()
    {
        if(moveToggle == false)
        {
            moveToggle = true;
        }
        else
        {
            moveToggle = false;
        }
    }
}
