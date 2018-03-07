using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{

    public int tileX;
    public int tileZ;
    public int unitId;
    public TileMap _map;
    private bool _isMoving;
    public MoveInput _characterMoveInput;
    public bool moveToggle;
    public Animator anim;
    public int abil;
    public bool react;
    public Rigidbody _rigidbody;
    private Vector3 _nextTile;
    private const float MOVEMENT_SPEED = 100f;
    private bool first = true;

    public List<Node> _currentPath = null;

    private void Start()
    {
        this.unitId = -1;
        anim = GetComponentInChildren<Animator>();
        abil = 0;
        react = false;
        
        
        
    }

    void quickUpdate() {
        foreach (KeyValuePair<string, GameObject> entry in _map._tileObjects)
        {
            if (((int) entry.Value.transform.position.x == (int)this.transform.position.x) && ((int)entry.Value.transform.position.z == (int)this.transform.position.z))
            {
                tileX = (int)entry.Value.transform.position.x;
                tileZ = (int)entry.Value.transform.position.z;
                _nextTile = entry.Value.transform.localPosition;
            }
        }
    }

    void Update()
    {
        if ((first == true) && (_map.genDone == true)) {
            quickUpdate();
            first = false;
        }

        anim.SetBool("Moving", _isMoving);
        anim.SetInteger("Ability", abil);
        abil = 0;
        anim.SetBool("React", react);
        react = false;
    }

    void FixedUpdate()
    {
        if (_currentPath != null && _isMoving)
        {
            //if we are pretty damn close to the center of the next tile, move to the next one
            float currDistance = Vector3.Distance(this.transform.position, _nextTile);
            if (currDistance > 0 && currDistance < 0.2)
            {
                MoveToNextTile();  
            }
            //else keep moving towards the current targeted tile
            else
            {
                _rigidbody.AddRelativeForce(Vector3.forward * MOVEMENT_SPEED, ForceMode.Force);
            }
        }
    }

    public void setUnitId(int id)
    {
        this.unitId = id;
    }

    public int getUnitId()
    {
        return this.unitId;
    }

    private void MoveToNextTile()
    {
        if (_currentPath == null)
            return;

        _rigidbody.velocity = Vector3.zero;
        this.transform.position = _nextTile;

        //if we get in this, we know we are at our destination
        if (_currentPath.Count == 1)
        {
            //update the units X/Y
            this.tileX = _currentPath[0].x;
            this.tileZ = _currentPath[0].z;
            //remove the path highlight
            _map.UnhighlightTilesInCurrentPath();
            //set the tile to be unwalkable since the unit is on top of it
            _map.SetTileWalkable(this.tileX, this.tileZ, false);
            _currentPath = null;
            _isMoving = false;
            moveToggle = false;
        }
        else
        {
            //remove the tile we are standing on
            _currentPath.RemoveAt(0);
            //global coordinates for the next tile
            _nextTile = _map.TileCoordToWorldCoord(_currentPath[0].x, _currentPath[0].z);
            this.transform.LookAt(_nextTile);
            //update the units X/Y
            this.tileX = _currentPath[0].x;
            this.tileZ = _currentPath[0].z; 
        }

    }

    public void BeginMovement()
    {
        if (moveToggle)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                if (_currentPath == null)
                    return;
                _map.SetTileWalkable(this.tileX, this.tileZ, true);
                MoveToNextTile();
                _isMoving = true;
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
        _map.SelectedUnitChanged(this.gameObject);
    }
}
