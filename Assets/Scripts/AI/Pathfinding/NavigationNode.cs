using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationNode
{
    public float gCost; //distance from start node to this node following the path
    public float hCost; //estimate distance to goal node, generally just a straight line distance

    public int x, y, z; //the grid cells each node relates to

    public bool isOpen = false, isClosed = false; //which list is the node in

    public NavigationNode parent; //the node came from to reach this one

    public NavigationNode(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

        gCost = -1;
        hCost = -1;
    }
    
    public float fCostSet() //estimation of total cost of current path from start to end nodes
    {
        return gCost + hCost;
    }
}
