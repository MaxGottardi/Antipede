using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackup : Node
{
    float runTime = 2;
    bool playedAudio = false;
    Tween backupRingTween = null;

    public CallBackup(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override void init()
    {
        base.init();

        backupRingTween = new Tween(Vector3.zero, new Vector3(blackboard.backupRingScale, blackboard.backupRingScale, blackboard.backupRingScale), Quaternion.identity, 
            Quaternion.identity, Time.time, 2);
        runTime = 2;
        blackboard.anim.SetTrigger("Backup");
        blackboard.callingBackup = true;
        blackboard.backupRing.SetActive(true);

        playedAudio = false;
        //start call for backup
    }
    public override NodeState evaluate()
    {
        //within a certain range of the AI cast out and see who responds
        if (runTime <= 1 && !playedAudio)
        {
            blackboard.audioSource.Play();
            playedAudio = true;
        }
        if (runTime <= 0)
        {
            int numAnts = 0;
            Collider[] hitColliders = Physics.OverlapSphere(blackboard.transform.position, blackboard.backupCallDist, blackboard.EnemyLayer);

            foreach (Collider obj in hitColliders) //get all nearby ants to call for backup
            {
                if (obj.gameObject.CompareTag("Enemy"))
                {
                    GenericAnt antObj = obj.gameObject.transform.parent.GetComponent<GenericAnt>();

                    if (antObj != null && antObj.stateMachine != null && antObj.stateMachine.currState == antObj.stateMachine.Movement)
                    {
                        antObj.canInvestigate = true;
                        numAnts++;
                    }
                }
            }
            runTime = 2;
            if (blackboard.health < 20 || GameManager1.playerObj.GetComponent<MCentipedeBody>().Segments.Count > 5) //still play the backup animation and attract neighbours, but do not spawn in any help
                SpawnHelpers(numAnts);
            blackboard.backupRing.SetActive(false);
            return NodeState.Success;
        }
        else
        {
            blackboard.backupRing.transform.localScale = backupRingTween.UpdatePosition();

            runTime -= Time.deltaTime;
            return NodeState.Running;
        }
    }

    void SpawnHelpers(int currAmount)
    {
        int numAntsSpawn = Mathf.Clamp(Mathf.CeilToInt(blackboard.chanceSpawnHelpers * GameManager1.mCentipedeBody.Segments.Count/100 - currAmount), 0, 5);
        List<Vector3> spawnPositions;

        if (numAntsSpawn > 0)
        {
            float radius = 4;
            spawnPositions = PossionDiskSampling.CreatePoints(radius, 30, blackboard.backupCallDist, blackboard.transform.position.x - radius, blackboard.transform.position.z - radius);

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

    public override void saveData(ref GenericAntData saveableData)
    {
        saveableData.callBackupRunTime.list.Add(runTime);
        saveableData.callBackupPlayedAudio.list.Add(playedAudio);
        if (backupRingTween != null)
            saveableData.callBackupTweenStartTime.list.Add(backupRingTween.StartTime);
        else
            saveableData.callBackupTweenStartTime.list.Add(0);
    }

    public override void loadData(ref GenericAntData saveableData)
    {
        runTime = saveableData.callBackupRunTime.list[0];
        saveableData.callBackupRunTime.list.RemoveAt(0);

        playedAudio = saveableData.callBackupPlayedAudio.list[0];
        saveableData.callBackupPlayedAudio.list.RemoveAt(0);
        if (backupRingTween != null)
        {
            ////////note this will not actually work as when loading from a save time.time will be different
            backupRingTween = new Tween(Vector3.zero, new Vector3(blackboard.backupRingScale, blackboard.backupRingScale, blackboard.backupRingScale), Quaternion.identity,
            Quaternion.identity, saveableData.callBackupRunTime.list[0], 2);
        }
        saveableData.callBackupRunTime.list.RemoveAt(0);
    }
}

public class CanCallBackup : Node
{
    float maxBackupWait;
    public CanCallBackup(GenericAnt blackboard)
    {
        this.blackboard = blackboard;

        blackboard.callBackupWait = 50;
        maxBackupWait = blackboard.callBackupWait;
    }
    public override NodeState evaluate()
    {
        //within a certain range of the AI cast out and see who responds
        if (!blackboard.isHelper && Vector3.Distance(blackboard.transform.position, blackboard.nextPosTransform.position) < blackboard.maxBackupDist && blackboard.callBackupWait <= 0)
        {
            blackboard.callBackupWait = maxBackupWait;
            return NodeState.Success; //can call for backup
        }
        else
            return NodeState.Failure; //cannot call for backup
    }

    public override void loadData(ref GenericAntData saveableData)
    {
        //nothing to load in
    }

    public override void saveData(ref GenericAntData saveableData)
    {
        //nothing to save either
    }
}
