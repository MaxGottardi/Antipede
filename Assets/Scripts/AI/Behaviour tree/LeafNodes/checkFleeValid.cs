using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFleeValid : Node
{
    public CheckFleeValid(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        //mainly if some random flee range has been met
        //flee chance is calculated based on 

        //fleeChance = blackboard.minFleeChance + blackboard.health / blackboard.maxHealth;
        if(Random.value < blackboard.minFleeChance && blackboard.health < blackboard.maxHealth / 2 && GameManager1.mCentipedeBody.Segments.Count > 10)
        {
            blackboard.pathToNextPos.Clear(); //as valid remove any current path have
            //Debug.Log("Begun to Flee");
            return NodeState.Success;
        }

        return NodeState.Failure;//as failed to flee play the damage animation
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
        //nothing to load
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
        //nothing to save
    }
}