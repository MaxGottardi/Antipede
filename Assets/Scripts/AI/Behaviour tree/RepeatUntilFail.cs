using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatUntilFail : Node
{
    protected Node child;
    public RepeatUntilFail(Node child)
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
                nodeState = NodeState.Running;
                return nodeState;
            case NodeState.Failure:
                child.end();
                nodeState = NodeState.Failure;
                end(); //only stop executing the single node if it fails
                return nodeState;
        }
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
