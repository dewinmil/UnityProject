using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour
{
    [SyncVar]
    public int tileX;
    [SyncVar]
    public int tileZ;
    public int unitId;
    public TileMap _map;
    [SyncVar]
    public bool _isMoving;
    public MoveInput _characterMoveInput;
    [SyncVar]
    public bool moveToggle;
    public Animator anim;
    [SyncVar]
    public int abil;
    [SyncVar]
    public bool react;
    [SyncVar]
    public bool dead;
    //number of tiles the unit can move
    /// /////////////////////////////////////
    //DO NOT CHANGE THIS VALUE IN THIS FILE
    public int _numMoves;
    /// //////////////////////////////////////

    public Rigidbody _rigidbody;
    private Vector3 _nextTile;
    private const float MOVEMENT_SPEED = 100f;
    public int _costToMove;
    private List<Node> _tilesToMove;
    public List<Node> _currentPath = null;
    public CharacterStatus _characterStatus;

    private void Start()
    {
        this.unitId = -1;
        anim = GetComponentInChildren<Animator>();
        abil = 0;
        react = false;
        dead = false;
        if (_map == null)
            _map = FindObjectOfType<TileMap>();

        _characterStatus = this.gameObject.GetComponent<CharacterStatus>();
    }


    void Update()
    {
        anim.SetBool("Moving", _isMoving);
        anim.SetInteger("Ability", abil);
        abil = 0;
        anim.SetBool("React", react);
        react = false;
        if (dead)
        {
            anim.SetBool("Dead", true);
        }
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
            //this.tileX = _currentPath[0].x;
            //this.tileZ = _currentPath[0].z;
            //remove the path highlight
            _map.UnhighlightTilesInCurrentPath();
            //set the tile to be unwalkable since the unit is on top of it
            CmdSetTileWalkable(this.tileX, this.tileZ, false);
            _currentPath = null;
            _isMoving = false;
            moveToggle = false;

            CmdSynchAnimations(abil, _isMoving, react, dead);
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

                if (_characterStatus.CanMove(_currentPath.Count - 1, _costToMove))
                {
                    _nextTile = _map.TileCoordToWorldCoord(_currentPath[0].x, _currentPath[0].z);
                    //set their origin tile to be walkable
                    CmdSetTileWalkable(this.tileX, this.tileZ, true);
                    MoveToNextTile();
                    _isMoving = true;

                    CmdSynchAnimations(abil, _isMoving, react, dead);
                }
            }
        }
    }

    public void toggleMovement()
    {
        if (moveToggle == false)
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

    public void HighlightWalkableTiles()
    {
        if (moveToggle == false)
            _map.UnhighlightWalkableTiles();

        else
            _tilesToMove = _map.HighlightWalkableTiles(this.tileX, this.tileZ, _characterStatus._numMovesRemaining);
    }
    public void UnhighlightWalkableTiles()
    {
        _map.UnhighlightWalkableTiles();
    }

    public bool InRangeOfSelectedTile(int x, int z)
    {
        if (_tilesToMove == null)
            return false;

        if (_tilesToMove.Any(n => n.x == x && n.z == z))
            return true;

        return false;
    }

    [Command]
    public void CmdSynchAnimations(int _abil, bool moving, bool _react, bool _dead)
    {
        abil = _abil;
        _isMoving = moving;
        react = _react;

        dead = _dead;
        RpcSynchAnimations(abil, _isMoving, react, dead);
    }

    [ClientRpc]
    public void RpcSynchAnimations(int _abil, bool moving, bool _react, bool _dead)
    {
        abil = _abil;
        _isMoving = moving;
        react = _react;
        dead = _dead;
    }

    public void updateMap(int x, int z, bool isWalkable)
    {
        CmdSetTileWalkable(x, z, isWalkable);
    }

    //This is called by the client when they move
    [Command]
    public void CmdSetTileWalkable(int x, int z, bool isWalkable)
    {
        this.tileX = x;
        this.tileZ = z;
        _map.SetTileWalkable(x, z, isWalkable);
        RpcUnitMoved(x, z, isWalkable);
    }

    //this sends the message to the other client about their unit moving
    [ClientRpc]
    public void RpcUnitMoved(int x, int z, bool isWalkable)
    {
        this.tileX = x;
        this.tileZ = z;
        _map.SetTileWalkable(x, z, isWalkable);
    }

    [Command]
    public void CmdLookAt(GameObject _target)
    {
        gameObject.transform.LookAt(_target.transform.position);
        _target.transform.LookAt(gameObject.transform.position);
        RpcLookAt(_target);
    }

    [ClientRpc]
    public void RpcLookAt(GameObject _target)
    {
        gameObject.transform.LookAt(_target.transform.position);
        _target.transform.LookAt(gameObject.transform.position);
    }
}