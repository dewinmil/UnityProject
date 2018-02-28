using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Security.Cryptography;
using System.Text;


public class TileMap : MonoBehaviour
{
    //2-D array of tiles
    private int[,] _tiles;
    private HashAlgorithm _hashAlgorithm;
    private Dictionary<string, GameObject> _tileObjects;
    private int _mapSizeX = 20;
    private int _mapSizeZ = 20;
    public TileType[] _tileTypes;
    public GameObject _selectedUnit;
    private Node[,] _graph;
    public Ray ray;
    private const float TILE_OFFSET = 1.80f;
    private const float TILE_Y_POS = -.5f;
    private bool wasCasting;
    private Node[] _currentPath;
    private readonly Color CURRENT_PATH_TILE_COLOR = Color.yellow;
    private readonly Color WALKABLE_TILE_COLOR = new Color(0.49f, 1.0f, 0.47f);
    private readonly Color UNWALKABLE_TILE_COLOR = new Color(1.0f, 0.47f, 0.47f);
    public GameObject _gameMasterObject;

    private void Start()
    {
        wasCasting = false;
        _hashAlgorithm = MD5.Create();
        _tileObjects = new Dictionary<string, GameObject>();
        //set up selected unit vars
        _selectedUnit.GetComponent<Unit>().tileX = (int)_selectedUnit.transform.position.x;
        _selectedUnit.GetComponent<Unit>().tileZ = (int)_selectedUnit.transform.position.z;
        _selectedUnit.GetComponent<Unit>()._map = this;

        //Generate the data for the map 
        GenerateMapData();

        //Generate graph for pathing
        GeneratePathGraph();

        //Spawn the prefabs
        GenerateMapObjects();
    }

    private void Update()
    {
        if (_selectedUnit.GetComponent<MoveInput>().castingSpell)
        {
            wasCasting = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.tag == "Unit")
                    {
                        if (_selectedUnit.GetComponent<Unit>().moveToggle == false)
                        {
                            if (wasCasting == false)
                            {
                                _selectedUnit = hit.collider.gameObject;
                            }
                            else
                            {
                                wasCasting = false;
                            }
                        }
                    }
                }
            }
        }
    }

    //method should be called when selected unit is changed
    //ideally this would be an event
    public void SelectedUnitChanged(GameObject selectedUnit)
    {
        _selectedUnit = selectedUnit;
    }

    private void GenerateMapObjects()
    {
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int z = 0; z < _mapSizeZ; z++)
            {
                //get the type the tile should be
                TileType tt = _tileTypes[_tiles[x, z]];

                GameObject tile;
                //add the tile to the map
                if ((z % 2) == 0)
                    tile = Instantiate(tt.TileVisuallPrefab, new Vector3(x * TILE_OFFSET, TILE_Y_POS, z * (TILE_OFFSET-.15f)), Quaternion.Euler(90, 0, 0));
                else
                    tile = Instantiate(tt.TileVisuallPrefab, new Vector3((x * TILE_OFFSET) + (TILE_OFFSET/2), TILE_Y_POS, z * (TILE_OFFSET - .15f)), Quaternion.Euler(90, 0, 0));

                //make the map clickable
                ClickableTile ct = tile.GetComponent<ClickableTile>();
                ct.tileX = x;
                ct.tileZ = z;
                ct.map = this;

                //give each tile its own hash as its identifier. To get the hash, you need to hash the string of the x coordinate plus the z coordinate
                string hash = GetHashString(x, z);
                if(!_tileObjects.ContainsKey(hash))
                    _tileObjects.Add(hash, tile);
            }
        }
        //enable the game master after the tiles have been created
        _gameMasterObject.SetActive(true);
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
                CalculateNeighbors(x, z);
            }
        }
    }

    public void CalculateNeighbors(int x, int z)
    {
        //Find all neighbors in a hex direction

        //left neighbors
        if (x > 0)
        {
            //left neighbor
            _graph[x, z].neighbours.Add(_graph[x - 1, z]);

            //bottom left
            //if it's an even z row, the neighbors will be different due to the offset of the hex tiles
            if (z > 0 && z % 2 == 0)
                _graph[x, z].neighbours.Add(_graph[x - 1, z - 1]);
            else if (z > 0)
                _graph[x, z].neighbours.Add(_graph[x, z - 1]);

            //upper left
            //if it's an even z row, the neighbors will be different due to the offset of the hex tiles
            if (z < _mapSizeZ - 1 && z % 2 == 0)
                _graph[x, z].neighbours.Add(_graph[x - 1, z + 1]);
            else if (z < _mapSizeZ - 1)
                _graph[x, z].neighbours.Add(_graph[x, z + 1]);
        }

        //right neighbors
        if (x < _mapSizeX - 1)
        {
            //right neighbor
            _graph[x, z].neighbours.Add(_graph[x + 1, z]);

            //bottom right
            //if it's an even z row, the neighbors will be different due to the offset of the hex tiles
            if (z > 0 && z % 2 == 0)
                _graph[x, z].neighbours.Add(_graph[x, z - 1]);
            else if (z > 0)
                _graph[x, z].neighbours.Add(_graph[x + 1, z - 1]);

            //upper right
            //if it's an even z row, the neighbors will be different due to the offset of the hex tiles
            if (z < _mapSizeZ - 1 && z % 2 == 0)
                _graph[x, z].neighbours.Add(_graph[x, z + 1]);
            else if (z < _mapSizeZ - 1)
                _graph[x, z].neighbours.Add(_graph[x + 1, z + 1]);
        }
    }

    //This is an implementation of Dijkstra's algorithm
    public void GeneratePathTo(int x, int z)
    {
        //remove old path on selected unit
        _selectedUnit.GetComponent<Unit>()._currentPath = null;

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

            if (current != null)
                HighlightSelectedTile(current.x, current.z, CURRENT_PATH_TILE_COLOR);
        }

        //right now the currentPath has a route from our target to our source
        //needs to be inverted so that we can traverse the path
        currentPath.Reverse();
        //copy the path to the global so that we can remember what it was
        _currentPath = new Node[currentPath.Count];
        currentPath.CopyTo(_currentPath);

        _selectedUnit.GetComponent<Unit>()._currentPath = currentPath;
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
        string hash = GetHashString(x, z);
        GameObject targetTile = _tileObjects[hash];

        return targetTile.transform.localPosition;
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

    public static byte[] GetHash(string inputString)
    {
        HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(int x, int z)
    {
        string inputString = String.Format("{0}:{1}", x.ToString(), z.ToString());
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    public void UnhighlightTilesInCurrentPath()
    {
        if(_currentPath == null)
            return;

        int count = 0;
        foreach (Node tile in _currentPath)
        {
            Color color;
            if (count == _currentPath.Length-1)
                color = UNWALKABLE_TILE_COLOR;
            else
                color = WALKABLE_TILE_COLOR;

            HighlightSelectedTile(tile.x, tile.z, color);
            count++;
        }
    }

    public void HighlightSelectedTile(int x, int z, Color color)
    {
        string hash = GetHashString(x, z);
        MeshRenderer mesh = _tileObjects[hash].GetComponent<MeshRenderer>();
        if (mesh == null)
            return;
        mesh.material.color = color;
    }

    public void SetTileWalkable(int x, int z, bool isWalkable)
    {
        //if we pass in true, make the tile walkable (0)
        //if we pass in false, make the tile unwalkable (1)
        _tiles[x, z] = isWalkable ? 0 : 1;

        Color color = isWalkable ? WALKABLE_TILE_COLOR : UNWALKABLE_TILE_COLOR;
        HighlightSelectedTile(x, z, color);
    }

    //NOTE the bool highlight here determines if we are highlighting the tiles around the unit, or unhighlighting them
    public int[,] HighlightWalkableTiles(int maxDistance, bool highight)
    {
        Unit unit = _selectedUnit.GetComponent<Unit>();
        Color color = highight ? CURRENT_PATH_TILE_COLOR : WALKABLE_TILE_COLOR;
        //int[,]
        for (int x = unit.tileX; x < (maxDistance + unit.tileX); x++)
        {
            for (int z = unit.tileZ; z < (maxDistance + unit.tileZ); z++)
            {
                for (int i = 0; i < maxDistance; i++)
                {
                    if(x-i > 0 && z - i > 0)
                    HighlightSelectedTile(x - i, z - i, color);

                    if(x + i < _mapSizeX && z + i < _mapSizeZ)
                        HighlightSelectedTile(x + i, z + i, color);

                    if (x + i < _mapSizeX && z - i > 0)
                        HighlightSelectedTile(x + i, z - i, color);

                    if (x - i > 0 && z + i < _mapSizeZ)
                        HighlightSelectedTile(x - i, z + i, color);
                }
            }
        }
        return new int[1,1];
    }

}
