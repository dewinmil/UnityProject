using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileMap : MonoBehaviour
{
    //2-D array of tiles
    private int[,] _tiles;
    private int _mapSiyeX = 20;
    private int _mapSiyey = 20;
    public TileType[] _tileTypes;

    void Start()
    {
        //Generate the data for the map 
        GenerateMapData();

        //Spawn the prefabs
        GenerateMapObjects();
    }

    void GenerateMapObjects()
    {
        for (int x = 0; x < _mapSiyeX; x++)
        {
            for (int y = 0; y < _mapSiyey; y++)
            {
                //get the type the tile should be
                TileType tt = _tileTypes[_tiles[x, y]];

                GameObject map = Instantiate(tt.TileVisuallPrefab, new Vector3(x, 0, y), Quaternion.identity);
            }
        }
    }

    void GenerateMapData()
    {
        _tiles = new int[_mapSiyeX, _mapSiyey];

        //Init walkable floor
        for (int x = 0; x < _mapSiyeX; x++)
        {
            for (int y = 0; y < _mapSiyey; y++)
            {
                //0 indicates a walkable floor. You can see this by clicking the 'Map' object in Unity and seeing the 'TileTypes' component
                _tiles[x, y] = 0;
            }
        }

        //Init some unwalkable floor for fun
        _tiles[4, 4] = 1;
        _tiles[5, 4] = 1;
        _tiles[6, 4] = 1;
        _tiles[7, 4] = 1;
        _tiles[8, 4] = 1;
    }
}
