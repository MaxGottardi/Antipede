using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackup : Node
{
    float runTime = 2;
    public CallBackup(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override void init()
    {
        base.init();

        runTime = 2;
        blackboard.GetComponent<Animator>().SetTrigger("backupCall");
        blackboard.callingBackup = true;
        //start call for backup
    }
    public override NodeState evaluate()
    {
        //within a certain range of the AI cast out and see who responds
        if (runTime <= 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(blackboard.transform.position, blackboard.backupCallDist, blackboard.EnemyLayer);

            foreach (Collider obj in hitColliders) //get all nearby ants to call for backup
            {
                GenericAnt antObj = obj.gameObject.GetComponent<GenericAnt>();
                if (obj.gameObject.CompareTag("Enemy") && antObj.stateMachine.currState == antObj.stateMachine.Movement)
                    antObj.canInvestigate = true;
            }
            runTime = 2;
            return NodeState.Success;
        }
        else
        {
            runTime -= Time.deltaTime;
            return NodeState.Running;
        }
    }
    public override void end()
    {
        base.end();

        blackboard.callingBackup = false;
    }
}

public class CanCallBackup : Node
{
    public CanCallBackup(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        //within a certain range of the AI cast out and see who responds
        if (Vector3.Distance(blackboard.transform.position, blackboard.nextPosTransform.position) < blackboard.maxBackupDist && blackboard.callBackupWait <= 0 &&
            (blackboard.health < 20 || GameManager1.playerObj.GetComponent<MCentipedeBody>().Segments.Count > 5))
        {
            blackboard.callBackupWait = 20;
            return NodeState.Success; //can call for backup
        }
        else
            return NodeState.Failure; //cannot call for backup
    }
}
