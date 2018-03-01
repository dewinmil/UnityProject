using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{ 
    //NOTE: THIS CLASS IS ACTIVATED AFTER THE TILEMAP HAS CREATED ALL OF ITS TILES, SEE TileMap.cs ln 122
    public List<GameObject> _team1Units;
    public List<GameObject> _team2Units;
    private const int NUMBER_OF_UNITS = 10; 
    public int activeTeam;
    public GameObject _baseCharacter;
    public GameObject _mapObject;
    private TileMap _map;

    // Use this for initialization
    void Start()
    {
        activeTeam = 0;
        _map = _mapObject.GetComponent<TileMap>();
        CreateUnits();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateUnits()
    {
        _team1Units = new List<GameObject>();
        _team2Units = new List<GameObject>();

        //spawn team 1's units
        for (int x = 0; x < NUMBER_OF_UNITS; x++)
        {
            //default spawn every unit on the first row
            GameObject unit = Instantiate(_baseCharacter, _map.TileCoordToWorldCoord(x, 0), new Quaternion());
            unit.GetComponent<Unit>().tileX = x;
            unit.GetComponent<Unit>().tileZ = 0;
            unit.GetComponent<CharacterStatus>().teamNum = 1;
            _team1Units.Add(unit);
            _map.SetTileWalkable(x, 0, false);
        }

        //spawn team 2's units
        for (int x = 0; x < NUMBER_OF_UNITS; x++)
        {
            //default spawn every unit on the first row
            GameObject unit = Instantiate(_baseCharacter, _map.TileCoordToWorldCoord(x, 19), Quaternion.Euler(0, 180, 0));
            unit.GetComponent<Unit>().tileX = x;
            unit.GetComponent<Unit>().tileZ = 19;
            unit.GetComponent<CharacterStatus>().teamNum = 2;
            _team2Units.Add(unit);
            _map.SetTileWalkable(x, 19, false);
        }
    }

    public void ChangeTeam()
    {
        activeTeam = (activeTeam + 1) % 2;
    }
}
