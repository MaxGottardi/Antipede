using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    this is just a generic function showcasing the base things each leaf node is required to contain to operate properly
    can simply copy this code into any leaf node and modify as needed
 */
public class GenericLeaf : Node
{
    public GenericLeaf(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }

    public override void init()
    {
        base.init();
    }
    public override NodeState evaluate()
    {
        doInit();

        return NodeState.Running; //can be whatever retrun is nessesary
    }

    public override void end()
    {
        base.end();
    }
}