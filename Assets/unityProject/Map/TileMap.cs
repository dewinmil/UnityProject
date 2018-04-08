using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine.Networking;


public class TileMap : NetworkBehaviour
{
    //2-D array of tiles
    private int[,] _tiles;
    private HashAlgorithm _hashAlgorithm;

    private Dictionary<string, GameObject> _tileObjects;
    public int _mapSizeX = 20;
    public int _mapSizeZ = 20;
    public TileType[] _tileTypes;
    public GameObject _selectedUnit;
    private Node[,] _graph;
    public Ray ray;
    private const float TILE_OFFSET = 1.80f;
    private const float TILE_Y_POS = -.5f;
    private bool wasCasting;
    private Node[] _currentPath;
    private List<GameObject> _highlightedTiles;
    private readonly Color CURRENT_PATH_TILE_COLOR = Color.yellow;
    private readonly Color WALKABLE_TILE_COLOR = new Color(0.49f, 1.0f, 0.47f);
    private readonly Color UNWALKABLE_TILE_COLOR = new Color(1.0f, 0.47f, 0.47f);
    public bool genDone = false;
    public bool charSelect = false;
    public List<UnitSpawn> _team1SpawnLocations;
    public List<UnitSpawn> _team2SpawnLocations;
    public List<UnitSpawn> _initialSpawns;

    private void Awake()
    {
        //genDone = false;
        wasCasting = false;
        _hashAlgorithm = MD5.Create();
        _tileObjects = new Dictionary<string, GameObject>();
        _highlightedTiles = new List<GameObject>();
        //set up selected unit vars
        //_selectedUnit.GetComponent<Unit>().tileX = (int)_selectedUnit.transform.position.x;
        //_selectedUnit.GetComponent<Unit>().tileZ = (int)_selectedUnit.transform.position.z;
        _selectedUnit.GetComponent<Unit>()._map = this;

        //set up spawns
        _team1SpawnLocations = InitTeam1Spawns();
        _team2SpawnLocations = InitTeam2Spawns();
        _initialSpawns = new List<UnitSpawn>();
        _initialSpawns.AddRange(_team1SpawnLocations);
        _initialSpawns.AddRange(_team2SpawnLocations);

        //Generate the data for the map 
        GenerateMapData();

        //Generate graph for pathing
        GeneratePathGraph();

        //Spawn the prefabs
        GenerateMapObjects();
        genDone = true;
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
                    tile = Instantiate(tt.TileVisuallPrefab, new Vector3(x * TILE_OFFSET, TILE_Y_POS, z * (TILE_OFFSET - .15f)), Quaternion.Euler(90, 0, 0));
                else
                    tile = Instantiate(tt.TileVisuallPrefab, new Vector3((x * TILE_OFFSET) + (TILE_OFFSET / 2), TILE_Y_POS, z * (TILE_OFFSET - .15f)), Quaternion.Euler(90, 0, 0));

                //make the map clickable
                ClickableTile ct = tile.GetComponent<ClickableTile>();
                ct.tileX = x;
                ct.tileZ = z;
                ct.map = this;
                //DEBUG ONLY: DISPLAY THE TILE COORDINATE ON TOP OF THE TILE
                //tile.GetComponentInChildren<TextMesh>().text = String.Format("({0} , {1})", x, z);

                //give each tile its own hash as its identifier. To get the hash, you need to hash the string of the x coordinate plus the z coordinate
                string hash = GetHashString(x, z);
                if (!_tileObjects.ContainsKey(hash))
                    _tileObjects.Add(hash, tile);
            }
        }

        //highlight initial spawn locations for units
        foreach (UnitSpawn spawn in _initialSpawns)
            SetTileWalkable(spawn._x, spawn._z, false);
    }

    private List<UnitSpawn> InitTeam1Spawns()
    {
        List<UnitSpawn> spawns = new List<UnitSpawn>();
        //warrior locations: HARDCODED TO 4 UNITS
        UnitSpawn warr1 = new UnitSpawn(2, _mapSizeZ - 2, "warrior");
        spawns.Add(warr1);
        UnitSpawn warr2 = new UnitSpawn(6, _mapSizeZ - 2, "warrior");
        spawns.Add(warr2);
        UnitSpawn warr3 = new UnitSpawn(12, _mapSizeZ - 2, "warrior");
        spawns.Add(warr3);
        UnitSpawn warr4 = new UnitSpawn(17, _mapSizeZ - 2, "warrior");
        spawns.Add(warr4);

        //knight locations: HARDCODED TO 2 UNITS
        UnitSpawn knight1 = new UnitSpawn(0, _mapSizeZ - 1, "knight");
        UnitSpawn knight2 = new UnitSpawn(_mapSizeX - 1, _mapSizeZ - 1, "knight");
        spawns.Add(knight1);
        spawns.Add(knight2);

        //Spearman locations: HARDCODED TO 2 UNITS
        UnitSpawn spear1 = new UnitSpawn(5, _mapSizeZ - 1, "spear");
        UnitSpawn spear2 = new UnitSpawn(14, _mapSizeZ - 1, "spear");
        spawns.Add(spear1);
        spawns.Add(spear2);

        //Wizard location
        UnitSpawn wizard = new UnitSpawn(9, _mapSizeZ - 1, "wizard");
        spawns.Add(wizard);

        //Leader Location
        UnitSpawn leader = new UnitSpawn(10, _mapSizeZ - 1, "leader");
        spawns.Add(leader);

        return spawns;
    }

    //TODO: These spawns are all hardcoded
    private List<UnitSpawn> InitTeam2Spawns()
    {
        List<UnitSpawn> spawns = new List<UnitSpawn>();
        //warrior locations: HARDCODED TO 4 UNITS
        //warrior locations: HARDCODED TO 4 UNITS
        UnitSpawn warr1 = new UnitSpawn(2, 1, "warrior");
        spawns.Add(warr1);
        UnitSpawn warr2 = new UnitSpawn(6, 1, "warrior");
        spawns.Add(warr2);
        UnitSpawn warr3 = new UnitSpawn(12, 1, "warrior");
        spawns.Add(warr3);
        UnitSpawn warr4 = new UnitSpawn(17, 1, "warrior");
        spawns.Add(warr4);

        //knight locations: HARDCODED TO 2 UNITS
        UnitSpawn knight1 = new UnitSpawn(0, 0, "knight");
        UnitSpawn knight2 = new UnitSpawn(_mapSizeX - 1, 0, "knight");
        spawns.Add(knight1);
        spawns.Add(knight2);

        //Spearman locations: HARDCODED TO 2 UNITS
        UnitSpawn spear1 = new UnitSpawn(5, 0, "spear");
        UnitSpawn spear2 = new UnitSpawn(14, 0, "spear");
        spawns.Add(spear1);
        spawns.Add(spear2);

        //Wizard location
        UnitSpawn wizard = new UnitSpawn(9, 0, "wizard");
        spawns.Add(wizard);

        //Leader Location
        UnitSpawn leader = new UnitSpawn(10, 0, "leader");
        spawns.Add(leader);

        return spawns;
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
            {
                //change the tile colors
                string hash = GetHashString(current.x, current.z);
                MeshRenderer mesh = _tileObjects[hash].GetComponent<MeshRenderer>();
                mesh.material.color = CURRENT_PATH_TILE_COLOR;
            }
        }

        //right now the currentPath has a route from our target to our source
        //needs to be inverted so that we can traverse the path
        currentPath.Reverse();
        //copy the path to the global so that we can remember what it was
        _currentPath = new Node[currentPath.Count];
        currentPath.CopyTo(_currentPath);

        //set destination to be occupied
        SetTileWalkable(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].z, false);

        //set origin to be walkable
        SetTileWalkable(currentPath[0].x, currentPath[0].z, true);

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
    }

    public Vector3 TileCoordToWorldCoord(int x, int z)
    {
        string hash = GetHashString(x, z);
        GameObject targetTile = _tileObjects[hash];

        return targetTile.transform.localPosition;
    }

    private float CostToEnterTile(int targetX, int targetZ, int sourceX, int sourceZ)
    {
        float movementCost;
        try
        {

            TileType tt = _tileTypes[_tiles[targetX, targetZ]];

            //if the tile is walkable, the unit can move through it
            if (tt.IsWalkable)
                movementCost = 1;
            
            //else set the cost to be infinity so that the algorithm will avoid it
            else
                movementCost = Mathf.Infinity;
        }
        catch (IndexOutOfRangeException ex)
        {
            movementCost = -1;
        }
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
        if (_currentPath == null)
            return;

        int count = 0;
        foreach (Node tile in _currentPath)
        {
            string hash = GetHashString(tile.x, tile.z);
            MeshRenderer mesh = _tileObjects[hash].GetComponent<MeshRenderer>();
            if (count == _currentPath.Length - 1)
                mesh.material.color = UNWALKABLE_TILE_COLOR;
            else
                mesh.material.color = WALKABLE_TILE_COLOR;

            count++;
        }
    }

    private void SetTileWalkable(int x, int z, bool isWalkable)
    {
        //if we pass in true, make the tile walkable (0)
        //if we pass in false, make the tile unwalkable (1)
        _tiles[x, z] = isWalkable ? 0 : 1;

        string hash = GetHashString(x, z);
        MeshRenderer mesh = _tileObjects[hash].GetComponent<MeshRenderer>();
        mesh.material.color = isWalkable ? WALKABLE_TILE_COLOR : UNWALKABLE_TILE_COLOR;
    }

    //Params are current units x/z coords and the number of tiles the unit can move
    public List<Node> HighlightWalkableTiles(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = null;
        if (playerZ % 2 == 0)
            neighbors = BuildQuadrantsEven(playerX, playerZ, numMoves);

        else
            neighbors = BuildQuadrantsOdd(playerX, playerZ, numMoves);

        foreach (Node node in neighbors)
        {
            string hash = GetHashString(node.x, node.z);
            MeshRenderer mesh = _tileObjects[hash].GetComponent<MeshRenderer>();
            mesh.material.color = CURRENT_PATH_TILE_COLOR;
            _highlightedTiles.Add(_tileObjects[hash]);
        }

        return neighbors;
    }

    #region BuildQuadrantsEven
    private List<Node> BuildQuadrantsEven(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        //first upper right quadrant
        neighbors.AddRange(BuildUpperRightQuadrantEven(playerX, playerZ, numMoves));
        //next upper left
        neighbors.AddRange(BuildUpperLeftQuadrantEven(playerX, playerZ, numMoves));
        //next bottom right
        neighbors.AddRange(BuildLowerRightQuadrantEven(playerX, playerZ, numMoves));
        //finally bottom left
        neighbors.AddRange(BuildLowerLeftQuadrantEven(playerX, playerZ, numMoves));

        return neighbors;
    }
    private List<Node> BuildUpperRightQuadrantEven(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        int xMax = playerX + numMoves;
        int zMax = playerZ + numMoves;
        //int to hold the halfway point the unit can move
        int xHalfWay = (int)Math.Floor((double)playerX + ((double)numMoves / 2));

        //int to hold the previous z value used when we begin sloping downwards
        int prevZ = zMax - 2;
        //begin creating the upper right quadrant area
        for (int x = playerX; x <= xMax; x++)
        {
            //TILES LESS THAN HALF
            //if this tile is less than or equal to the halfway point, we want to add all tiles in its column up until the max
            if (x <= xHalfWay)
            {
                for (int z = playerZ; z <= zMax; z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
            //TILES MORE THAN HALF BUT NOT LAST
            //here we calculate how many tiles in this column we should add
            else if (x != xMax && x > xHalfWay)
            {
                int xDistFromPlayer = x - playerX;
                int xMovesRemaining = prevZ;
                //if we have an odd number of moves, we need to add one more because of the shift
                if (numMoves % 2 != 0)
                    xMovesRemaining++;

                for (int z = playerZ; z <= xMovesRemaining; z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
                prevZ -= 2;
            }
            //if we get here we are at our max
            else
            {
                float cost = CostToEnterTile(x, playerZ, playerX, playerZ);
                if (cost > -1f && cost < Mathf.Infinity)
                {
                    Node node = new Node(x, playerZ);
                    neighbors.Add(node);
                }
            }
        }
        return neighbors;
    }
    private List<Node> BuildUpperLeftQuadrantEven(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        int xMin = playerX - numMoves;
        int zMax = playerZ + numMoves;

        //int to hold the halfway point the unit can move
        int xHalfWay = (int)Math.Floor((double)playerX - ((double)numMoves / 2));

        //int to hold the previous z value used when we begin sloping downwards
        int prevZ = zMax - 1;
        //begin creating the upper left quadrant area
        for (int x = playerX; x >= xMin; x--)
        {
            //TILES LESS THAN HALF
            //if this tile is less than or equal to the halfway point, we want to add all tiles in its column up until the max
            if (x >= xHalfWay)
            {
                for (int z = playerZ; z <= zMax; z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
            //TILES MORE THAN HALF BUT NOT LAST
            //here we calculate how many tiles in this column we should add
            else if (x != xMin && x < xHalfWay)
            {
                int xDistFromPlayer = playerX - x;
                int xMovesRemaining = prevZ;
                //if we have an odd number of moves, we need to subtract one more because of the shift
                if (numMoves % 2 != 0)
                    xMovesRemaining--;

                for (int z = playerZ; z <= xMovesRemaining; z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
                prevZ -= 2;
            }
            //if we get here we are at our max
            //we need to add the last tile and the one above it in this scenario
            else
            {
                for (int z = playerZ; z <= (playerZ + 1); z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
        }
        return neighbors;
    }
    private List<Node> BuildLowerRightQuadrantEven(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        int xMax = playerX + numMoves;

        int zMin = playerZ - numMoves;
        //int to hold the halfway point the unit can move
        int xHalfWay = (int)Math.Floor((double)playerX + ((double)numMoves / 2));

        //int to hold the previous z value used when we begin sloping downwards
        int prevZ = zMin + 2;
        //begin creating the lower right quadrant area
        for (int x = playerX; x <= xMax; x++)
        {
            //TILES LESS THAN HALF
            //if this tile is less than or equal to the halfway point, we want to add all tiles in its column up until the max
            if (x <= xHalfWay)
            {
                for (int z = playerZ; z >= zMin; z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
            //TILES MORE THAN HALF BUT NOT LAST
            //here we calculate how many tiles in this column we should add
            else if (x != xMax && x > xHalfWay)
            {
                int xDistFromPlayer = x - playerX;
                int xMovesRemaining = prevZ;
                //if we have an odd number of moves, we need to add one more because of the shift
                if (numMoves % 2 != 0)
                    xMovesRemaining--;

                for (int z = playerZ; z >= xMovesRemaining; z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
                prevZ += 2;
            }
            //if we get here we are at our max
            else
            {
                float cost = CostToEnterTile(x, playerZ, playerX, playerZ);
                if (cost > -1f && cost < Mathf.Infinity)
                {
                    Node node = new Node(x, playerZ);
                    neighbors.Add(node);
                }
            }
        }
        return neighbors;
    }
    private List<Node> BuildLowerLeftQuadrantEven(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        int xMax = playerX + numMoves;
        int xMin = playerX - numMoves;

        int zMin = playerZ - numMoves;
        int zMax = playerZ;
        //int to hold the halfway point the unit can move
        int xHalfWay = (int)Math.Floor((double)playerX - ((double)numMoves / 2));

        //int to hold the previous z value used when we begin sloping downwards
        int prevZ = zMin + 1;
        //begin creating the upper left quadrant area
        for (int x = playerX; x >= xMin; x--)
        {
            //TILES LESS THAN HALF
            //if this tile is less than or equal to the halfway point, we want to add all tiles in its column up until the max
            if (x >= xHalfWay)
            {
                for (int z = playerZ; z >= zMin; z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
            //TILES MORE THAN HALF BUT NOT LAST
            //here we calculate how many tiles in this column we should add
            else if (x != xMin && x < xHalfWay)
            {
                int xDistFromPlayer = playerX - x;
                int xMovesRemaining = prevZ;
                //if we have an odd number of moves, we need to subtract one more because of the shift
                if (numMoves % 2 != 0)
                    xMovesRemaining++;

                for (int z = playerZ; z >= xMovesRemaining; z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
                prevZ += 2;
            }
            //if we get here we are at our max
            //we need to add the last tile and the one above it in this scenario
            else
            {
                for (int z = playerZ; z >= (playerZ - 1); z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
        }
        return neighbors;
    }
    #endregion

    #region BuildQuadrantsOdd
    private List<Node> BuildQuadrantsOdd(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();

        //upper right quadrant
        neighbors.AddRange(BuildUpperRightQuadrantOdd(playerX, playerZ, numMoves));
        //upper left quadrat
        neighbors.AddRange(BuildUpperLeftQuadrantOdd(playerX, playerZ, numMoves));
        //lower right quadrant
        neighbors.AddRange(BuildLowerRightQuadrantOdd(playerX, playerZ, numMoves));
        //lower left quad
        neighbors.AddRange(BuildLowerLeftQuadrantOdd(playerX, playerZ, numMoves));

        return neighbors;
    }
    private List<Node> BuildUpperRightQuadrantOdd(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        int xMax = playerX + numMoves;
        int zMax = playerZ + numMoves;
        //int to hold the halfway point the unit can move
        int xHalfWay = (int)Math.Floor((double)playerX + ((double)numMoves / 2));

        //int to hold the previous z value used when we begin sloping downwards
        int prevZ = zMax - 1;
        //begin creating the upper right quadrant area
        for (int x = playerX; x <= xMax; x++)
        {
            //TILES LESS THAN HALF
            //if this tile is less than or equal to the halfway point, we want to add all tiles in its column up until the max
            if (x <= xHalfWay)
            {
                for (int z = playerZ; z <= zMax; z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
            //TILES MORE THAN HALF BUT NOT LAST
            //here we calculate how many tiles in this column we should add
            else if (x != xMax && x > xHalfWay)
            {
                int xDistFromPlayer = x - playerX;
                int xMovesRemaining = prevZ;
                //if we have an odd number of moves, we need to add one more because of the shift
                if (numMoves % 2 != 0)
                    xMovesRemaining++;

                for (int z = playerZ; z <= xMovesRemaining; z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
                prevZ -= 2;
            }
            //if we get here we are at our max
            else
            {
                for (int z = playerZ; z <= (playerZ + 1); z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
        }
        return neighbors;
    }
    private List<Node> BuildUpperLeftQuadrantOdd(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        int xMin = playerX - numMoves;
        int zMax = playerZ + numMoves;

        //int to hold the halfway point the unit can move
        int xHalfWay = (int)Math.Floor((double)playerX - ((double)numMoves / 2));

        //int to hold the previous z value used when we begin sloping downwards
        int prevZ = zMax - 1;
        //begin creating the upper left quadrant area
        for (int x = playerX; x >= xMin; x--)
        {
            //TILES LESS THAN HALF
            //if this tile is less than or equal to the halfway point, we want to add all tiles in its column up until the max
            if (x > xHalfWay)
            {
                for (int z = playerZ; z <= zMax; z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
            //TILES MORE THAN HALF BUT NOT LAST
            //here we calculate how many tiles in this column we should add
            else if (x != xMin && x <= xHalfWay)
            {
                int xDistFromPlayer = playerX - x;
                int xMovesRemaining = prevZ;
                //if we have an EVEN number of moves, we need to add one more because of the shift
                if (numMoves % 2 == 0)
                    xMovesRemaining++;

                for (int z = playerZ; z <= xMovesRemaining; z++)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
                prevZ -= 2;
            }
            //if we get here we are at our max
            //we need to add the last tile and the one above it in this scenario
            else
            {
                float cost = CostToEnterTile(x, playerZ, playerX, playerZ);
                if (cost > -1f && cost < Mathf.Infinity)
                {
                    Node node = new Node(x, playerZ);
                    neighbors.Add(node);
                }
            }
        }
        return neighbors;
    }
    private List<Node> BuildLowerRightQuadrantOdd(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        int xMax = playerX + numMoves;

        int zMin = playerZ - numMoves;
        //int to hold the halfway point the unit can move
        int xHalfWay = (int)Math.Floor((double)playerX + ((double)numMoves / 2));

        //int to hold the previous z value used when we begin sloping downwards
        int prevZ = zMin + 1;
        //begin creating the lower right quadrant area
        for (int x = playerX; x <= xMax; x++)
        {
            //TILES LESS THAN HALF
            //if this tile is less than or equal to the halfway point, we want to add all tiles in its column up until the max
            if (x <= xHalfWay)
            {
                for (int z = playerZ; z >= zMin; z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
            //TILES MORE THAN HALF BUT NOT LAST
            //here we calculate how many tiles in this column we should add
            else if (x != xMax && x > xHalfWay)
            {
                int xDistFromPlayer = x - playerX;
                int xMovesRemaining = prevZ;
                //if we have an odd number of moves, we need to subtract one more because of the shift
                if (numMoves % 2 != 0)
                    xMovesRemaining--;

                for (int z = playerZ; z >= xMovesRemaining; z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
                prevZ += 2;
            }
            //if we get here we are at our max
            else
            {
                for (int z = playerZ; z >= (playerZ - 1); z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
        }
        return neighbors;
    }
    private List<Node> BuildLowerLeftQuadrantOdd(int playerX, int playerZ, int numMoves)
    {
        List<Node> neighbors = new List<Node>();
        int xMax = playerX + numMoves;
        int xMin = playerX - numMoves;

        int zMin = playerZ - numMoves;
        int zMax = playerZ;
        //int to hold the halfway point the unit can move
        int xHalfWay = (int)Math.Floor((double)playerX - ((double)numMoves / 2));

        //int to hold the previous z value used when we begin sloping downwards
        int prevZ = zMin + 1;
        //begin creating the upper left quadrant area
        for (int x = playerX; x >= xMin; x--)
        {
            //TILES LESS THAN HALF
            //if this tile is less than or equal to the halfway point, we want to add all tiles in its column up until the max
            if (x > xHalfWay)
            {
                for (int z = playerZ; z >= zMin; z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
            }
            //TILES MORE THAN HALF BUT NOT LAST
            //here we calculate how many tiles in this column we should add
            else if (x != xMin && x <= xHalfWay)
            {
                int xDistFromPlayer = playerX - x;
                int xMovesRemaining = prevZ;
                //if we have an even number of moves, we need to subtract one more because of the shift
                if (numMoves % 2 == 0)
                    xMovesRemaining--;

                for (int z = playerZ; z >= xMovesRemaining; z--)
                {
                    float cost = CostToEnterTile(x, z, playerX, playerZ);
                    if (cost > -1f && cost < Mathf.Infinity)
                    {
                        Node node = new Node(x, z);
                        neighbors.Add(node);
                    }
                }
                prevZ += 2;
            }
            //if we get here we are at our max
            //we need to add the last tile and the one above it in this scenario
            else
            {
                float cost = CostToEnterTile(x, playerZ, playerX, playerZ);
                if (cost > -1f && cost < Mathf.Infinity)
                {
                    Node node = new Node(x, playerZ);
                    neighbors.Add(node);
                }
            }
        }
        return neighbors;
    }
    #endregion

    public void UnhighlightWalkableTiles()
    {
        foreach (var tile in _highlightedTiles)
        {
            MeshRenderer mesh = tile.GetComponent<MeshRenderer>();
            mesh.material.color = WALKABLE_TILE_COLOR;
        }
    }

    //This is called by the client when they move
    [Command]
    public void CmdSetTileWalkable(int x, int z, bool isWalkable)
    {
        SetTileWalkable(x, z, isWalkable);
        RpcUnitMoved(x, z, isWalkable);
    }

    //this sends the message to the other client about their unit moving
    [ClientRpc]
    public void RpcUnitMoved(int x, int z, bool isWalkable)
    {
        SetTileWalkable(x, z, isWalkable);
    }

    //command used to update the current clients map
    [Command]
    public void CmdUpdateMap()
    {
        GameMaster gm = FindObjectOfType<GameMaster>();
        foreach (Unit unit in gm._units)
        {
            RpcUnitMoved(unit.tileX, unit.tileZ, false);
        }
    }
}
