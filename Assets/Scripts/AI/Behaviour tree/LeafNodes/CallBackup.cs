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
        blackboard.anim.SetTrigger("Backup");
        blackboard.callingBackup = true;
        //start call for backup
    }
    public override NodeState evaluate()
    {
        //within a certain range of the AI cast out and see who responds
        if (runTime <= 0)
        {
            int numAnts = 0;
            Collider[] hitColliders = Physics.OverlapSphere(blackboard.transform.position, blackboard.backupCallDist, blackboard.EnemyLayer);

            foreach (Collider obj in hitColliders) //get all nearby ants to call for backup
            {
                GenericAnt antObj = obj.gameObject.transform.parent.GetComponent<GenericAnt>();
                if (obj.gameObject.CompareTag("Enemy") && antObj && antObj.stateMachine.currState == antObj.stateMachine.Movement)
                {
                    antObj.canInvestigate = true;
                    numAnts++;
                }
            }
            runTime = 2;
            SpawnHelpers(numAnts);
            return NodeState.Success;
        }
        else
        {
            runTime -= Time.deltaTime;
            return NodeState.Running;
        }
    }

    void SpawnHelpers(int currAmount)
    {
        int numAntsSpawn = Mathf.Clamp(Mathf.FloorToInt(blackboard.chanceSpawnHelpers * GameManager1.mCentipedeBody.Segments.Count - currAmount), 0, 5);
        List<Vector3> spawnPositions;

        if (numAntsSpawn > 0)
        {
            spawnPositions = PossionDiskSampling.CreatePoints(3, 30, blackboard.backupCallDist, blackboard.transform.position.x, blackboard.transform.position.z);

            if (spawnPositions.Count > 0)
            {
                for (int i = 0; i < numAntsSpawn; i++)
                {
                    if (spawnPositions.Count > 0)
                    {
                        int randIndex = Random.Range(0, spawnPositions.Count);
                        Vector3 spawnPos = spawnPositions[randIndex];
                        spawnPos.y = blackboard.transform.position.y;
                        spawnPositions.RemoveAt(randIndex);
                        while (spawnPositions.Count > 0 && blackboard.NearSegment(spawnPos, true) && Vector3.Distance(spawnPos, blackboard.transform.position) < 1)
                        {
                            randIndex = Random.Range(0, spawnPositions.Count);
                            spawnPos = spawnPositions[randIndex];
                            spawnPos.y = blackboard.transform.position.y;
                            spawnPositions.RemoveAt(randIndex);
                        }
                        GameObject ant = GameObject.Instantiate(blackboard.spawnedHelpBag.getNext(), spawnPos, Quaternion.identity);
                        GenericAnt genericAnt = ant.GetComponent<GenericAnt>();
                        genericAnt.isHelper = true;
                    }
                }
            }
        }
    }

    public override void end()
    {
        base.end();

        blackboard.callingBackup = false;
        blackboard.backupRing.SetActive(false);
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
        if (!blackboard.isHelper && Vector3.Distance(blackboard.transform.position, blackboard.nextPosTransform.position) < blackboard.maxBackupDist && blackboard.callBackupWait <= 0 &&
            (blackboard.health < 20 || GameManager1.playerObj.GetComponent<MCentipedeBody>().Segments.Count > 5))
        {
            blackboard.callBackupWait = 20;
            return NodeState.Success; //can call for backup
        }
        else
            return NodeState.Failure; //cannot call for backup
    }
}
