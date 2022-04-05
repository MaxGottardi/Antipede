using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//behaviour tree used to actually determine what occurs at each state e.g for movement 2 nodes would exists one for determine next position and the other for moving to that pos(if nessesary)
public abstract class Node
{
    public GenericAnt blackboard;
   // public Node(roo blackboard)
   // {
   //     this.blackboard = blackboard;
   // }

    public NodeState nodeState;
    public enum NodeState { Success, Failure, Running };

    public abstract NodeState evaluate(); //determine the state the node is in upon completion
}
