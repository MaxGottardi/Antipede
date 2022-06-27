using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
public class DetermineAttackSeg : Node
{
    public DetermineAttackSeg(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }

    public override NodeState evaluate()
    {
#if UNITY_EDITOR
        Profiler.BeginSample("AI Determine Seg Attack");
#endif
        if (GameManager1.mCentipedeBody.Segments.Count > 0)
        {
            MSegment randSegment = null;
            float distToSegmnet = -1;
            int numAttackingSegment = int.MaxValue;

            foreach (MSegment segment in GameManager1.playerObj.GetComponent<MCentipedeBody>().Segments) //find the closest player segment
            {
                float dist = Vector3.Distance(blackboard.transform.position, segment.gameObject.transform.position);
                if ((randSegment == null || dist < distToSegmnet) && segment.numAttacking <= numAttackingSegment) //also if segment does not have an ant currently attacking it
                {
                    randSegment = segment;
                    distToSegmnet = dist;
                    numAttackingSegment = segment.numAttacking;
                }
            }

            //if (randSegment == null)
            {
                //as no segment found without an ant attacking it, just pick a random one
                int randIndex = Random.Range(0, GameManager1.mCentipedeBody.Segments.Count); //pick a random segment to attack
                randSegment = GameManager1.mCentipedeBody.Segments[randIndex];
            }
            
            ////float distToSegmnet = -1;

            if (randSegment != null)
            {
                blackboard.nextPosTransform = randSegment.gameObject.transform;
                randSegment.numAttacking++;
                blackboard.pathToNextPos = GameManager1.generateGrid.APathfinding(blackboard.transform.position, blackboard.nextPosTransform.position);//generate the new path

                if (blackboard.pathToNextPos.Count > 0)
                {
                    blackboard.nextPosVector = blackboard.pathToNextPos[blackboard.pathToNextPos.Count - 1];
                    blackboard.pathToNextPos.RemoveAt(blackboard.pathToNextPos.Count - 1);
                    if (blackboard.pathToNextPos.Count <= 0) //as no new tiles to move towards can safely say move towards the final goal
                        blackboard.nextPosVector = blackboard.nextPosTransform.position;
                    //currSegment.beingAttacked = true;
                    return NodeState.Success; //assigned segment
                }
            }
        }
#if UNITY_EDITOR
        Profiler.EndSample();
#endif
        ////blackboard.stateMachine.changeState(blackboard.stateMachine.Movement);
        return NodeState.Failure; //no segments found
        //////This for some reason causes an error to appear
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
        //do nothing as no new data to save
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
        //do nothing as no new data to save
    }
}

public class PathToSegment : Node
{
    public PathToSegment(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }

    public override NodeState evaluate()
    {

        if (blackboard.nextPosTransform != null)
        {
            if (blackboard.pathToNextPos.Count > 0)
            {
                GenerateNewPath(blackboard.transform.position, blackboard.nextPosTransform.position);
               
                if (blackboard.pathToNextPos.Count > 0)
                {
                    blackboard.nextPosVector = blackboard.pathToNextPos[blackboard.pathToNextPos.Count - 1]; //get the next node to move towards
                    blackboard.pathToNextPos.RemoveAt(blackboard.pathToNextPos.Count - 1);
                }
            }
            if (blackboard.pathToNextPos.Count <= 0) //as no new tiles to move towards can safely say move towards the final goal
                blackboard.nextPosVector = blackboard.nextPosTransform.position;

            return NodeState.Success; //assigned segment
        }
        return NodeState.Failure;
    }

    void GenerateNewPath(Vector3 currPos, Vector3 goalPos)
    {
        if (blackboard.pathToNextPos.Count <= 0 || Time.frameCount % 20 == 0 || Time.deltaTime > 0.25f)
        {
            blackboard.pathToNextPos = GameManager1.generateGrid.APathfinding(currPos, goalPos);//generate the new path
        }
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
        //do nothing as no new data to save
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
        //do nothing as no new data to save
    }
}