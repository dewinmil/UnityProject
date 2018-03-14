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

        //if this is the host client, spawn them on the other side of the map
        if (_playerID < NUM_UNITS_PER_TEAM)
            player = Instantiate(Unit1, _map.TileCoordToWorldCoord(_prevX, 0), Quaternion.identity) as GameObject;
        else
            player = Instantiate(Unit2, _map.TileCoordToWorldCoord(_prevX, _map._mapSizeZ - 1), Quaternion.Euler(0, 180, 0)) as GameObject;

        Unit unit = CreateUnit(player.GetComponent<Unit>());
        _units.Add(unit);
        _map.SetTileWalkable(unit.tileX, unit.tileZ, false);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
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
    private Unit CreateUnit(Unit unit)
    {
        unit.tileX = _prevX;
        unit.tileZ = 0;
        unit._map = _map;

        _prevX++;
        return unit;
    }

    public void endTurn()
    {
        if (turn == 1)
        {
            turn = 2;
        }
        else
        {
            turn = 1;
        }
    }
}