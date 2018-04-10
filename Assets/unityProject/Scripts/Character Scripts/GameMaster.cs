using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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
    private const int NUM_UNITS_PER_TEAM = 10;
    public List<Unit> _units;
    public int turn;
    public CharacterStatus _currentStatus;
    public GameObject winScreen;
    public GameObject loseScreen;
    public int connections;
    private List<UnitSpawn> _team1SpawnLocations;
    private List<UnitSpawn> _team2SpawnLocations;
    public List<UnitSpawn> _initialSpawns;

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
        winScreen = GameObject.FindWithTag("winScreen");
        loseScreen = GameObject.FindWithTag("loseScreen");
        _team1SpawnLocations = _map._team1SpawnLocations;
        _team2SpawnLocations = _map._team2SpawnLocations;
        _initialSpawns = new List<UnitSpawn>();

        //add all inital spawns to the list. Used for highlighting the initial tiles
        _initialSpawns.AddRange(_team1SpawnLocations);
        _initialSpawns.AddRange(_team2SpawnLocations);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //if these are still null set them again (race condition?)
        if (_team1SpawnLocations == null)
            _team1SpawnLocations = _map._team1SpawnLocations;

        if (_team2SpawnLocations == null)
            _team2SpawnLocations = _map._team2SpawnLocations;

        GameObject player;
        Unit unit;
        //if this is the host client, spawn them on the other side of the map
        if (playerControllerId < NUM_UNITS_PER_TEAM)
        {
            //spawn leader
            if (playerControllerId == 0)
            {
                UnitSpawn leaderSpawn = _team2SpawnLocations.FirstOrDefault(s => s._unitType.Equals("leader"));
                player = Instantiate(Leader2, _map.TileCoordToWorldCoord(leaderSpawn._x, leaderSpawn._z), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), leaderSpawn._x, leaderSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
                _team1SpawnLocations.Remove(leaderSpawn);
            }
            //spawn wizard
            else if (playerControllerId == 1)
            {
                UnitSpawn wizSpawn = _team2SpawnLocations.FirstOrDefault(s => s._unitType.Equals("wizard"));
                player = Instantiate(Wizard2, _map.TileCoordToWorldCoord(wizSpawn._x, wizSpawn._z), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), wizSpawn._x, wizSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
                _team2SpawnLocations.Remove(wizSpawn);
            }
            //spawn knights
            else if (playerControllerId <= 3)
            {
                UnitSpawn knightSpawn = _team2SpawnLocations.FirstOrDefault(s => s._unitType.Equals("knight"));
                player = Instantiate(Knight2, _map.TileCoordToWorldCoord(knightSpawn._x, knightSpawn._z), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), knightSpawn._x, knightSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
                _team2SpawnLocations.Remove(knightSpawn);
            }
            //spawn spearman
            else if (playerControllerId <= 5)
            {
                UnitSpawn spearSpawn = _team2SpawnLocations.FirstOrDefault(s => s._unitType.Equals("spear"));
                player = Instantiate(Spearman2, _map.TileCoordToWorldCoord(spearSpawn._x, spearSpawn._z), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), spearSpawn._x, spearSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
                _team2SpawnLocations.Remove(spearSpawn);
            }
            //spawn warriors
            else
            {
                UnitSpawn warrSpawn = _team2SpawnLocations.FirstOrDefault(s => s._unitType.Equals("warrior"));
                player = Instantiate(Warrior2, _map.TileCoordToWorldCoord(warrSpawn._x, warrSpawn._z), Quaternion.identity) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), warrSpawn._x, warrSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 1);
                _team2SpawnLocations.Remove(warrSpawn);
            }
        }
        else
        {
            //spawn leader
            if (playerControllerId == NUM_UNITS_PER_TEAM)
            {
                UnitSpawn leaderSpawn = _team1SpawnLocations.FirstOrDefault(s => s._unitType.Equals("leader"));
                player = Instantiate(Leader1, _map.TileCoordToWorldCoord(leaderSpawn._x, leaderSpawn._z), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), leaderSpawn._x, leaderSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
                _team1SpawnLocations.Remove(leaderSpawn);
            }
            //spawn wizard
            else if (playerControllerId == NUM_UNITS_PER_TEAM + 1)
            {
                UnitSpawn wizSpawn = _team1SpawnLocations.FirstOrDefault(s => s._unitType.Equals("wizard"));
                player = Instantiate(Wizard1, _map.TileCoordToWorldCoord(wizSpawn._x, wizSpawn._z), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), wizSpawn._x, wizSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
                _team1SpawnLocations.Remove(wizSpawn);
            }
            //spawn knights
            else if (playerControllerId <= NUM_UNITS_PER_TEAM + 3)
            {
                UnitSpawn knightSpawn = _team1SpawnLocations.FirstOrDefault(s => s._unitType.Equals("knight"));
                player = Instantiate(Knight1, _map.TileCoordToWorldCoord(knightSpawn._x, knightSpawn._z), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), knightSpawn._x, knightSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
                _team1SpawnLocations.Remove(knightSpawn);
            }
            //spawn spearman
            else if (playerControllerId <= NUM_UNITS_PER_TEAM + 5)
            {
                UnitSpawn spearSpawn = _team1SpawnLocations.FirstOrDefault(s => s._unitType.Equals("spear"));
                player = Instantiate(Spearman1, _map.TileCoordToWorldCoord(spearSpawn._x, spearSpawn._z), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), spearSpawn._x, spearSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
                _team1SpawnLocations.Remove(spearSpawn);
            }
            //spawn warriors
            else
            {
                UnitSpawn warrSpawn = _team1SpawnLocations.FirstOrDefault(s => s._unitType.Equals("warrior"));
                player = Instantiate(Warrior1, _map.TileCoordToWorldCoord(warrSpawn._x, warrSpawn._z), Quaternion.Euler(0, 180, 0)) as GameObject;
                unit = CreateUnit(player.GetComponent<Unit>(), warrSpawn._x, warrSpawn._z);
                UpdateCharacterStatus(player.GetComponent<CharacterStatus>(), 2);
                _team1SpawnLocations.Remove(warrSpawn);
            }
        }

        _units.Add(unit);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }


    public override void OnClientConnect(NetworkConnection conn)
    {

        AddPlayers(conn, NUM_UNITS_PER_TEAM);
        FindObjectOfType<ToggleActive>().playerConnected();
    }


    public override void OnServerConnect(NetworkConnection conn)
    {
        Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180.0f, Camera.main.transform.eulerAngles.z);
        Camera.main.transform.position = new Vector3(18.0f, 20.0f, 45.0f);

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
        print("why u no work");
    }

    //method used for creating the unit. Set all values here
    private Unit CreateUnit(Unit unit, int x, int z)
    {
        unit.tileX = x;
        unit.tileZ = z;
        unit._map = _map;

        return unit;
    }

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
    }

        private void UpdateCharacterStatus(CharacterStatus status, int teamNum)
        {
            status.teamNum = teamNum;
        }

}

public class UnitSpawn
{
    public int _x;
    public int _z;
    public string _unitType;
    public UnitSpawn(int x, int z, string unitType)
    {
        _x = x;
        _z = z;
        _unitType = unitType;
    }
}