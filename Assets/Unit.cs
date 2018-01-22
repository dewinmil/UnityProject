using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{

    public int tileX;
    public int tileZ;
    public TileMap map;

    public List<Node> currentPath = null;

    void Update()
    {
        if (currentPath != null)
        {
            int currNode = 0;

            while (currNode < currentPath.Count - 1)
            {
                Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].z);
                Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].z); 
                Debug.DrawLine(start, end, Color.red);

                currNode++;
            }
        }
    }

    public void MoveToNextTile()
    {
        if (currentPath == null)
            return;

        //remove the old current/first node from the path
        currentPath.RemoveAt(0);
        
        //grab the new first node and move to that position
        this.transform.position = map.TileCoordToWorldCoord(currentPath[0].x, currentPath[0].z);

        //if we get in this, we know we are at our destination
        if (currentPath.Count == 1)
            currentPath = null;
    }
}
