using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class GameMaster : NetworkManager
{
    public GameObject Unit1;
    public GameObject Unit2;
    public GameObject Unit3;
    public GameObject Unit4;
    public short _playerID;
    public TileMap _map;
    public GameObject _mapObject;
    private const int NUM_UNITS_PER_TEAM = 5;
    public List<Unit> _units;
    private int _prevX;
    public int turn;

    // Use this for initialization
    void Start()
    {
        turn = 1;
        _playerID = 0;
        _units = new List<Unit>();
        _prevX = 0;
        ClientScene.RegisterPrefab(Unit1);
        ClientScene.RegisterPrefab(Unit2);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {

        GameObject player;
        Unit unit;
        //if this is the host client, spawn them on the other side of the map
        //this is a shit way to do it but idgaf 
        if (_playerID < NUM_UNITS_PER_TEAM)
        {
            player = Instantiate(Unit1, _map.TileCoordToWorldCoord(_prevX, 0), Quaternion.identity) as GameObject;
            unit = CreateUnit(player.GetComponent<Unit>(), _prevX, 0);
            UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
        }
        else
        {
            player = Instantiate(Unit2, _map.TileCoordToWorldCoord(_prevX, _map._mapSizeZ - 1), Quaternion.Euler(0, 180, 0)) as GameObject;
            unit = CreateUnit(player.GetComponent<Unit>(), _prevX, _map._mapSizeZ - 1);
            UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
        }
        
        _units.Add(unit);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        if (_playerID > 0)
            _prevX = 0;

        AddPlayers(conn, NUM_UNITS_PER_TEAM);
    }

    private void AddPlayers(NetworkConnection conn, int numPlayers)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            ClientScene.AddPlayer(conn, _playerID);
            _playerID++;
        }
    }

    //method used for creating the unit. Set all values here
    private Unit CreateUnit(Unit unit, int x, int z)
    {
        unit.tileX = x;
        unit.tileZ = z;
        unit._map = _map;

        _prevX++;
        return unit;
    }

    private void UpdateCharacterStatus(CharacterStatus status, int teamNum)
    {
        status.teamNum = teamNum;
    }

}