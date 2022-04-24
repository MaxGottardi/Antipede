using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    NavigationNode[,,] grid;
    public int width, height, depth, tileSize;
    public Vector3 offset;
    public LayerMask groundLayer;
    public bool bShowGrid = false;

    List<NavigationNode> OpenSet = new List<NavigationNode>(); //the list of nodes which have been seen, but not yet evaluated
    List<NavigationNode> ClosedSet = new List<NavigationNode>(); //the list of nodes which have been seen, and evaluated
    // Start is called before the first frame update
    void Start()
    {
        grid = new NavigationNode[width, height, depth];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < depth; k++)
                {
                    if (Physics.CheckSphere(PointToWorld(i, j, k), tileSize * 0.5f, groundLayer))
                        grid[i, j, k] = new NavigationNode(i, j, k);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (bShowGrid)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < depth; k++)
                    {
                        Gizmos.color = Color.red;
                        if (grid.Length > 0 && grid[i, j, k] != null)
                            Gizmos.DrawCube(PointToWorld(i, j, k), new Vector3(1, 1, 1));

                    }
                }
            }
        }
    }

    Vector3 PointToWorld(int x, int y, int z)
    {
        return new Vector3(x * tileSize, y * tileSize, z * tileSize) + offset;
    }

    public void WorldToPoint(Vector3 worldPoint)
    {
        worldPoint -= offset;
        int x = Mathf.CeilToInt(worldPoint.x / tileSize);
        int y = Mathf.CeilToInt(worldPoint.y / tileSize);
        int z = Mathf.CeilToInt(worldPoint.z / tileSize);
    }

    /// <summary>
    /// find a valid node on the grid for the position, checking neighbours if the initially choosen one is not valid
    /// </summary>
    /// <param name="position">the position checking, either being the paths start or end</param>
    /// <returns>A position on the grid</returns>
    (int, int, int) FindValidCell(Vector3 position)
    {
        position -= offset;
        int gridX = Mathf.Clamp(Mathf.RoundToInt(position.x / tileSize), 0, width - 1);
        int gridY = Mathf.Clamp(Mathf.RoundToInt(position.y / tileSize), 0, height - 1);
        int gridZ = Mathf.Clamp(Mathf.RoundToInt(position.z / tileSize), 0, depth - 1);

        if(grid[gridX, gridY, gridZ] == null)
        {
            for (int i = -1; i <= 1; i++) //for every possible neighbour see if it would be valid instead
            {
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        NavigationNode neighbour;
                        if (gridX + i >= 0 && gridX + i < width && gridY + j >= 0 && gridY + j < height
                            && gridZ + k >= 0 && gridZ + k < depth)
                        {
                            neighbour = grid[gridX + i, gridY + j, gridZ + k];
                            if (neighbour != null) //as long as this is a valid node, return it
                            {
                                return (gridX + i, gridY + j, gridZ + k);
                            }
                        }
                    }
                }
            }
        }

        return (gridX, gridY, gridZ);
    }

    public List<Vector3> APathfinding(Vector3 startPosition, Vector3 targetPosition)
    {
        ResetPathFinding();
        //convert the world positions to the tiles on the grid
        (int targetGridX, int targetGridY, int targetGridZ) = FindValidCell(targetPosition);
        (int gridX, int gridY, int gridZ) = FindValidCell(startPosition);

//        Debug.Log(gridX + " " + gridY + " " + gridZ);
        if (grid[targetGridX, targetGridY, targetGridZ] != null && grid[gridX, gridY, gridZ] != null)
        {
            grid[gridX, gridY, gridZ].gCost = 0;
            grid[gridX, gridY, gridZ].hCost = DistToTarget(new Vector3(gridX, gridY, gridZ), new Vector3(targetGridX, targetGridY, targetGridZ));
            OpenSet.Add(grid[gridX, gridY, gridZ]); //add the start node to the open set

            while (OpenSet.Count > 0)
            {
                //find the node with the lowest fscore as its the one which is likly closest to the goal
                NavigationNode currentNode = LowestFCost(); //the current node at in the grid
                OpenSet.Remove(currentNode);
                ClosedSet.Add(currentNode);
                currentNode.isClosed = true;
                currentNode.isOpen = false;

                if (currentNode.x == targetGridX && currentNode.y == targetGridY && currentNode.z == targetGridZ)
                    return RecalculatePath(targetGridX, targetGridY, targetGridZ, gridX, gridY, gridZ); //break out as have reached the goal

                List<NavigationNode> neighbourNodes = new List<NavigationNode>();//every valid neighbour of the current node
                for (int i = -1; i <= 1; i++) //for every possible neighbour add it
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int k = -1; k <= 1; k++)
                        {
                            NavigationNode neighbour;
                            if (currentNode.x + i >= 0 && currentNode.x + i < width && currentNode.y + j >= 0 && currentNode.y + j < height
                                && currentNode.z + k >= 0 && currentNode.z + k < depth)//ensure checked node is inside the grid
                                                                                       //&& currentNode.x + i != currentNode.x && currentNode.y + j != currentNode.y && currentNode.z + k != currentNode.z) //ensure that node checking is also not the current node itself
                            {
                                neighbour = grid[currentNode.x + i, currentNode.y + j, currentNode.z + k];
                                if (neighbour != null && !neighbour.isClosed) //as long as valid to actually move to that node
                                {
                                    neighbourNodes.Add(neighbour);
                                }
                            }
                        }
                    }
                }
                if (neighbourNodes.Count <= 0) //also return when no new neighbours found
                {
                    //Debug.Log("found no valid neighbours");
                }
                //as now have a valid list of neighbour nodes can loop through them
                foreach (NavigationNode neighbour in neighbourNodes)
                {
                    float tentativeGCost = currentNode.gCost + DistToTarget(new Vector3(currentNode.x, currentNode.y, currentNode.z), new Vector3(neighbour.x, neighbour.y, neighbour.z));//the new gCost if this node is used
                    if (!neighbour.isOpen || tentativeGCost < currentNode.gCost) //if it the new node is closer or this neighbour not yet seen
                    {
                        neighbour.gCost = tentativeGCost; //update nodes values
                        neighbour.hCost = DistToTarget(new Vector3(neighbour.x, neighbour.y, neighbour.z), new Vector3(targetGridX, targetGridY, targetGridZ));
                        neighbour.parent = currentNode;
                        if (!neighbour.isOpen) //add it to the openset if it isn't already
                        {
                            OpenSet.Add(neighbour);
                            neighbour.isOpen = true;
                        }
                    }

                }
            }
        }
        Debug.Log("No Valid Path found");
        return new List<Vector3>();
    }

    NavigationNode LowestFCost()
    {
        NavigationNode current = OpenSet[0];
        foreach (NavigationNode node in OpenSet) //loop through all current nodes visited to find one with shortest path
        {
            if (node.fCostSet() < current.fCostSet())
                current = node;
        }

        return current; //the node with the current shortest path
    }

    List<Vector3> RecalculatePath(int targetX, int targetY, int targetZ, int startX, int startY, int startZ)
    {
        List<Vector3> worldPoints = new List<Vector3>();
        NavigationNode current = grid[targetX, targetY, targetZ];
        worldPoints.Add(PointToWorld(current.x, current.y, current.z));

        //Debug.Log(targetX + "  " + startX + "  " + targetY + "  " + startY + "  " + targetZ + "  " + startZ);
        while (current.x != startX || current.y != startY || current.z != startZ) //while not at the start position yet
        {
            worldPoints.Add(PointToWorld(current.x, current.y, current.z));
            current = current.parent;
            //Debug.Log("generating the Path!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
        return worldPoints;
    }

    float DistToTarget(Vector3 currPos, Vector3 targetPos)
    {
        return Vector3.Distance(currPos, targetPos);
    }

    void ResetPathFinding()
    {
        OpenSet.Clear();
        ClosedSet.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < depth; k++)
                {
                    Gizmos.color = Color.red;
                    if (grid.Length > 0 && grid[i, j, k] != null)
                    {
                        grid[i, j, k].isClosed = false;
                        grid[i, j, k].isOpen = false;
                        grid[i, j, k].hCost = -1;
                        grid[i, j, k].gCost = -1;
                    }
                }
            }
        }
    }
}
