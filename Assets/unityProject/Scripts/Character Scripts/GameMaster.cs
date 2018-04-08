using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.NetworkSystem;

public class GameMaster : NetworkManager
{
    public GameObject Warrior1;
    public GameObject Warrior2;
    public GameObject Knight1;
    public GameObject Knight2;
    public GameObject Wizard1;
    public GameObject Wizard2;
    public GameObject Spearman1;
    public GameObject Spearman2;
    public GameObject Leader1;
    public GameObject Leader2;
    public short _playerID;
    public TileMap _map;
    public GameObject _mapObject;
    private const int NUM_UNITS_PER_TEAM = 5;
    public List<Unit> _units;
    private int _prevX;
    public int turn;
<<<<<<< HEAD
    public CharacterStatus _currentStatus;
=======
    public GameObject winScreen;
    public GameObject loseScreen;
    public int connections;
>>>>>>> merge

    // Use this for initialization
    public void Start()
    {
        if (IsClientConnected())
        {
            _playerID = 0;
        }
        else
        {
            _playerID = 0;
        }
        connections = 0;
        turn = 1;
        _units = new List<Unit>();
        _prevX = 0;
        winScreen = GameObject.FindWithTag("winScreen");
        loseScreen = GameObject.FindWithTag("loseScreen");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player;
        Unit unit;
        //if this is the host client, spawn them on the other side of the map
        if (playerControllerId < NUM_UNITS_PER_TEAM)
        {
            if (playerControllerId == 0)
            {
                player = Instantiate(Warrior2, _map.TileCoordToWorldCoord(_prevX, 0), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, 0);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
            }
            else if (playerControllerId == 1)
            {
                player = Instantiate(Wizard2, _map.TileCoordToWorldCoord(_prevX, 0), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, 0);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
            }
            else if (playerControllerId == 2)
            {
                player = Instantiate(Leader2, _map.TileCoordToWorldCoord(_prevX, 0), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, 0);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
            }
            else if (playerControllerId == 3)
            {
                player = Instantiate(Knight2, _map.TileCoordToWorldCoord(_prevX, 0), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, 0);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
            }
            else
            {
                player = Instantiate(Spearman2, _map.TileCoordToWorldCoord(_prevX, 0), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, 0);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
            }
        }
        else
        {
            if (playerControllerId == 5)
            {
                player = Instantiate(Warrior1, _map.TileCoordToWorldCoord(_prevX, _map._mapSizeZ - 1), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, _map._mapSizeZ - 1);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
            }
            else if (playerControllerId == 6)
            {
                player = Instantiate(Wizard1, _map.TileCoordToWorldCoord(_prevX, _map._mapSizeZ - 1), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, _map._mapSizeZ - 1);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
            }
            else if (playerControllerId == 7)
            {
                player = Instantiate(Leader1, _map.TileCoordToWorldCoord(_prevX, _map._mapSizeZ - 1), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, _map._mapSizeZ - 1);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
            }
            else if (playerControllerId == 8)
            {
                player = Instantiate(Knight1, _map.TileCoordToWorldCoord(_prevX, _map._mapSizeZ - 1), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, _map._mapSizeZ - 1);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
            }
            else
            {
                player = Instantiate(Spearman1, _map.TileCoordToWorldCoord(_prevX, _map._mapSizeZ - 1), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), _prevX, _map._mapSizeZ - 1);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
            }
        }
        
        _units.Add(unit);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        if (_playerID > 0)
            _prevX = 0;

        AddPlayers(conn, NUM_UNITS_PER_TEAM);
        FindObjectOfType<ToggleActive>().playerConnected();
    }

    
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (_playerID > 0)
            _prevX = 0;

        AddPlayers(conn, NUM_UNITS_PER_TEAM);
    }
    
    private void AddPlayers(NetworkConnection conn, int numPlayers)
    {
        connections += 1;
        for (int i = 0; i < numPlayers; i++)
        {
            ClientScene.AddPlayer(conn, _playerID);
            _playerID++;
        }
        if (_playerID == NUM_UNITS_PER_TEAM * connections)
        {
            winScreen = GameObject.FindWithTag("winScreen");
            loseScreen = GameObject.FindWithTag("loseScreen");
            winScreen.GetComponent<Canvas>().enabled = false;
            loseScreen.GetComponent<Canvas>().enabled = false;
        }
    }

    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        FindObjectOfType<ToggleActive>().playerDisconnected();
        NetworkManager.Shutdown();
        SceneManager.LoadScene(0);
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
<<<<<<< HEAD
    
    public void endTurn()
    {
        //CharacterStatus _currentStatus = GetComponent("CharacterStatus") as CharacterStatus;
        //if (_currentStatus.getTeamNum() == turn)
        //{
            if (turn == 1)
            {
                turn = 2;
            }
            else
            {
                turn = 1;
            }
        //}
=======

    private void UpdateCharacterStatus(CharacterStatus status, int teamNum)
    {
        status.teamNum = teamNum;
>>>>>>> merge
    }

}