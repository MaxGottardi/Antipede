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

        return NodeState.Running; //can be whatever retrun is nessesary
    }

    public override void end()
    {
        base.end();
    }


    //functions for saving and loading the data from an ant
    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
    }
}