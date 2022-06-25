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

    public override void loadData(ref GenericAntData saveableData)
    {
        child.loadData(ref saveableData);
    }

    public override void saveData(ref GenericAntData saveableData)
    {
        child.saveData(ref saveableData);
    }
}
