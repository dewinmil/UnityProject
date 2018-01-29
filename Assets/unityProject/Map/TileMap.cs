using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TileMap : MonoBehaviour
{
    //2-D array of tiles
    private int[,] _tiles;
    private int _mapSizeX = 20;
    private int _mapSizeZ = 20;
    public TileType[] _tileTypes;
    public GameObject _selectedUnit;
    Node[,] _graph;

    void Start()
    {
        //set up selected unit vars
        _selectedUnit.GetComponent<Unit>().tileX = (int)_selectedUnit.transform.position.x;
        _selectedUnit.GetComponent<Unit>().tileZ = (int)_selectedUnit.transform.position.z;
        _selectedUnit.GetComponent<Unit>().map = this;

        //Generate the data for the map 
        GenerateMapData();

        //Generate graph for pathing
        GeneratePathGraph();

        //Spawn the prefabs
        GenerateMapObjects();
    }

    void GenerateMapObjects()
    {
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int z = 0; z < _mapSizeZ; z++)
            {
                //get the type the tile should be
                TileType tt = _tileTypes[_tiles[x, z]];

                //add the tile to the map
                GameObject tile = Instantiate(tt.TileVisuallPrefab, new Vector3(x, -.5f, z), Quaternion.identity);

                //make the map clickable
                ClickableTile ct = tile.GetComponent<ClickableTile>();
                ct.tileX = x;
                ct.tileZ = z;
                ct.map = this;
            }
        }
    }

    private void GeneratePathGraph()
    {
        // Initialize the array
        _graph = new Node[_mapSizeX, _mapSizeZ];

        // Initialize a Node for each spot in the array
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int z = 0; z < _mapSizeZ; z++)
            {
                _graph[x, z] = new Node();
                _graph[x, z].x = x;
                _graph[x, z].z = z;
            }
        }

        // Now that all the nodes exist, calculate their neighbors
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int z = 0; z < _mapSizeZ; z++)
            {
                //Find all neighbors in a hex direction

                //left neighbors
                if (x > 0)
                {
                    //left neighbor
                    _graph[x, z].neighbours.Add(_graph[x - 1, z]);

                    //bottom left
                    if (z > 0)
                        _graph[x, z].neighbours.Add(_graph[x - 1, z - 1]);
                    
                    //upper left
                    if(z < _mapSizeZ - 1)
                        _graph[x, z].neighbours.Add(_graph[x - 1, z + 1]);
                }

                //right neighbors
                if (x < _mapSizeX - 1)
                {
                    //right neighbor
                    _graph[x, z].neighbours.Add(_graph[x + 1, z]);

                    //bottom right
                    if (z > 0)
                        _graph[x, z].neighbours.Add(_graph[x + 1, z - 1]);

                    //upper right
                    if (z < _mapSizeZ - 1)
                        _graph[x, z].neighbours.Add(_graph[x + 1, z + 1]);
                }

                //lower neighbor
                if (z > 0)
                    _graph[x, z].neighbours.Add(_graph[x, z - 1]);

                //upper neighbor
                if (z < _mapSizeZ - 1)
                    _graph[x, z].neighbours.Add(_graph[x, z + 1]);
            }
        }
    }

    //This is an implementation of Dijkstra's algorithm
    public void GeneratePathTo(int x, int z)
    {
        //remove old path on selected unit
        _selectedUnit.GetComponent<Unit>().currentPath = null;

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        //set up list of nodes we have not checked yet
        List<Node> unvisited = new List<Node>();

        int sourceXPos = _selectedUnit.GetComponent<Unit>().tileX;
        int sourceZPos = _selectedUnit.GetComponent<Unit>().tileZ;

        Node source = _graph[sourceXPos, sourceZPos];
        Node target = _graph[x, z];

        dist[source] = 0;
        prev[source] = null;

        //init everything to have infinite distance, since we don't know any better atm
        //It's possible some nodes cannot be reached from the source
        foreach (Node node in _graph)
        {
            if (node != source)
            {
                dist[node] = Mathf.Infinity;
                prev[node] = null;
            }

            unvisited.Add(node);
        }

        while (unvisited.Count > 0)
        {
            //unvisited node with smallest distance
            Node u = null;

            //find the closest unvisited node
            foreach (Node possibleUnvisited in unvisited)
            {
                if (u == null || dist[possibleUnvisited] < dist[u])
                    u = possibleUnvisited;
            }

            //if U is the closest node to our target, then that's the shortest path
            if (u == target)
                break;

            unvisited.Remove(u);

            foreach (Node node in u.neighbours)
            {

                //float alt = dist[u] + u.DistanceTo(node);
                float alt = dist[u] + CostToEnterTile(node.x, node.z, u.x, u.z);
                //if this distance is less than the current shortest distance
                if (alt < dist[node])
                {
                    dist[node] = alt;
                    prev[node] = u;
                }
            }
        }

        //if we get here, we either found the shortest route or there is NO route to the target

        //no route between target and source
        if (prev[target] == null)
            return;

        //there is a route
        List<Node> currentPath = new List<Node>();

        Node current = target;

        //step through the prev chain and add it to the path
        while (current != null)
        {
            currentPath.Add(current);
            current = prev[current];
        }

        //right now the currentPath has a route from our target to our source
        //needs to be inverted so that we can traverse the path
        currentPath.Reverse();

        _selectedUnit.GetComponent<Unit>().currentPath = currentPath;
    }

    void GenerateMapData()
    {
        _tiles = new int[_mapSizeX, _mapSizeZ];

        //Init walkable floor
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int z = 0; z < _mapSizeZ; z++)
            {
                //0 indicates a walkable floor. You can see this by clicking the 'Map' object in Unity and seeing the 'TileTypes' component
                _tiles[x, z] = 0;
            }
        }

        //Init some unwalkable floor for fun
        _tiles[4, 4] = 1;
        _tiles[5, 4] = 1;
        _tiles[6, 4] = 1;
        _tiles[7, 4] = 1;
        _tiles[8, 4] = 1;
    }

    public Vector3 TileCoordToWorldCoord(int x, int z)
    {
        return new Vector3(x, 0, z);
    }

    private float CostToEnterTile(int targetX, int targetZ, int sourceX, int sourceZ)
    {
        TileType tt = _tileTypes[_tiles[targetX, targetZ]];
        float movementCost;

        //if the tile is walkable, the unit can move through it
        if (tt.IsWalkable)
            movementCost = 1;

        //else set the cost to be infinity so that the algorithm will avoid it
        else
            movementCost = Mathf.Infinity;

        //make movement more linear, makes more sense
        if (targetX != sourceX && targetZ != sourceZ)
            movementCost += 0.001f;

        return movementCost;
    }
}
