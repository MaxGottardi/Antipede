using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succeeder : Node
{
    protected Node child;
    public Succeeder(Node child)
    {
        this.child = child;//when this class initialized set the list to be the one created upon initilization
    }

    public override NodeState evaluate()
    {

        switch (child.execute())
        {
            case NodeState.Running:
                nodeState = NodeState.Running;
                return nodeState;
            case NodeState.Success:
                child.end();
                break;
            case NodeState.Failure:
                child.end();
                break;
        }
        nodeState = NodeState.Success; //no matter node state return success to parent
        end();
        return nodeState;
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
        child.loadData(saveableData);
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
        child.saveData(saveableData);
    }
}

