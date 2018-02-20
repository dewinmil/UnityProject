using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{

    public int tileX;
    public int tileZ;
    public int unitId;
    public TileMap map;
    private bool isMoving;
    public MoveInput _characterMoveInput;
    public bool moveToggle;
    public Animator anim;
    public int abil;
    public bool react;

    //variable to make the unit walk slower
    private int _waitCount = 0;

    public List<Node> currentPath = null;

    private void Start()
    {
        this.unitId = -1;
        anim = GetComponent<Animator>();
        abil = 0;
        react = false;
    }

    void Update()
    {
        _waitCount++;
        //if the character is set to move, move it
        //unit will only 'walk' every 15 frames
        //this probably isn't a good way to do it, since framerate will depend on the computer 
        //best way would be to use Time.DeltaTime I believe, but that can be implemented later
        if (currentPath != null && isMoving && (_waitCount % 60 == 0))
        {
            MoveUnitToTarget();
        }

        //Character animations for moving, ability use, and hit reaction.
        anim.SetBool("Moving", isMoving);
        anim.SetInteger("Ability", abil);
        abil = 0;
        anim.SetBool("React", react);
        react = false;
        
    }

    public void setUnitId(int id)
    {
        this.unitId = id;
    }

    public int getUnitId()
    {
        return this.unitId;
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
                //why are there two variables for when the unit is moving?
                map.UnhighlightTilesInCurrentPath();
                currentPath = null;
                isMoving = false;
                _waitCount = 0;
                moveToggle = false;
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

    public void SelectedUnitChanged()
    {
        map.SelectedUnitChanged(this.gameObject);
    }
}
