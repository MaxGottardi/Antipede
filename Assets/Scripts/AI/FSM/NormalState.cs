using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
/// <summary>
/// The normal movement around the map
/// </summary>
public class MovementState : State
{
    Node topNode;

    GenericAnt owner;
    public MovementState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
        GetNextNode getNextNode = new GetNextNode(owner);
        MoveTowards moveTowards = new MoveTowards(owner, true);

        topNode = new Sequence(new List<Node> { getNextNode, moveTowards });
    }
    public void enter()
    {
        GameObject closestNode = owner.nodesList[0]; //find the next location to move towards
        foreach (GameObject node in owner.nodesList)
        {
            if (Vector3.Distance(node.transform.position, owner.transform.position) < Vector3.Distance(closestNode.transform.position, owner.transform.position))
                closestNode = node;
        }
        owner.nextPosTransform = closestNode.transform;
    }

    public void execute()
    {
        Profiler.BeginSample("AI movement");
        if (owner.DetectPlayer() || owner.canInvestigate)
            owner.stateMachine.changeState(owner.stateMachine.Shock);
        else
            topNode.execute();

        Profiler.EndSample();
    }

    public void exit()
    {
        topNode.interupt();

        if (owner.canInvestigate)
            owner.canInvestigate = false;
    }

    public void loadData(GenericAntData saveableData)
    {
        topNode.loadData(saveableData);
    }

    public void saveData(GenericAntData saveableData)
    { 
        topNode.saveData(saveableData); 
    }
}
/// <summary>
/// Called when AI first sees player and it showcases warning above head
/// </summary>
public class ShockState : State
{
    float shockTime = 1.17f;
    GenericAnt owner;

    public ShockState(GenericAnt owner)
    {
        this.owner = owner;
    }
    public void enter()
    {
        shockTime = 1.17f;
        owner.callBackupWait = 0; //reset the ability to be able to call for backup
        owner.shockBar.SetActive(true);
    }

    public void execute()
    {
        shockTime -= Time.deltaTime;
        if (shockTime <= 0)
            owner.stateMachine.changeState(owner.stateMachine.Investigate);

    }

    public void exit()
    {
        shockTime = 1.17f;
        owner.shockBar.SetActive(false);
    }

    public void loadData(GenericAntData saveableData)
    {
        shockTime = saveableData.shockTimeLeft;
        owner.shockBar.SetActive(saveableData.bShockBarActiveState);

        if (saveableData.bShockBarActiveState)//if the shockbar was visible when saved, reset it to where it left from
            owner.shockBar.GetComponent<Animator>().Play("Show", 0, saveableData.shockBarCurrAntimNormTime);
    }

    public void saveData(GenericAntData saveableData)
    {
        saveableData.shockTimeLeft = shockTime;
        saveableData.bShockBarActiveState = owner.shockBar.activeSelf;
        saveableData.shockBarCurrAntimNormTime = saveableData.bShockBarActiveState ? owner.shockBar.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime : 0;
    }
}
/// <summary>
/// When seen player and moving towards it
/// </summary>
public class InvestigateState : State
{
    protected GenericAnt owner;
    protected float lostPlayerTime = 10.0f;

    protected Node topNode;
    public InvestigateState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;

        DetermineAttackSeg determineAttackSeg = new DetermineAttackSeg(owner);

        PathToSegment pathToSegment = new PathToSegment(owner);
        MoveTowards moveTowards = new MoveTowards(owner, false);

        CanCallBackup canCallBackup = new CanCallBackup(owner);
        CallBackup callBackup = new CallBackup(owner);
        Sequence callBackupSeq = new Sequence(new List<Node> { canCallBackup, callBackup });
        Succeeder backupSucceeder = new Succeeder(callBackupSeq);

        Sequence moveSequence = new Sequence(new List<Node> { pathToSegment, moveTowards, backupSucceeder });
        RepeatUntilFail repeatUntilFail = new RepeatUntilFail(moveSequence);

        topNode = new Sequence(new List<Node> { determineAttackSeg, repeatUntilFail });
    }

    public virtual void enter()
    {   
        lostPlayerTime = 10.0f;

        owner.callingBackup = false;

        topNode.execute();
    }
    public void execute()
    {
        if (owner.callBackupWait > 0) //update wating time between each call for beckup
            owner.callBackupWait -= Time.deltaTime;

        if (!owner.DetectPlayer())
            lostPlayerTime -= Time.deltaTime; //update amount of time not seen player for
        else if(lostPlayerTime < 10.0f) //if have seen the player, reset amount of time
            lostPlayerTime = 10.0f;

        if (!owner.callingBackup && lostPlayerTime <= 0) //when haven't seen player for 3 seconds or segment already destroyed
        {
            //if (owner.nextPosTransform && owner.nextPosTransform.gameObject.GetComponent<MSegment>())
            //    owner.nextPosTransform.gameObject.GetComponent<MSegment>().beingAttacked = false;
            owner.loseAttackInterest();
            owner.stateMachine.changeState(owner.stateMachine.Movement);
        }
        else if(owner.nextPosTransform == null || !owner.nextPosTransform.gameObject.CompareTag("PlayerSegment"))
        {
            owner.stateMachine.changeState(owner.stateMachine.Investigate); //restart the investigation as segment already destroyed
        }
        else if (checkAttack()) //within attack range of chosen player segment
        {
            owner.stateMachine.changeState(owner.stateMachine.Attack);
        }
        else //as no change of state occured, can run this one
        {
#if UNITY_EDITOR
            Profiler.BeginSample("AI investigataion");
#endif
            topNode.execute();
#if UNITY_EDITOR
            Profiler.EndSample();
#endif
        }
        
    }

    public virtual bool checkAttack()
    {
        float dot = Vector3.Dot(owner.transform.forward, owner.nextPosTransform.forward);
        return owner.NearSegment(owner.transform.position) && !owner.callingBackup && dot > -0.75f && dot < 0.75f;
    }

    public virtual void exit()
    {
        topNode.interupt();
        owner.shockBar.SetActive(false);
        lostPlayerTime = 0;
    }

    public virtual void saveData(GenericAntData saveableData)
    {
        saveableData.investigateStateLostPlayerTime = lostPlayerTime;
        topNode.saveData(saveableData);
    }

    public virtual void loadData(GenericAntData saveableData)
    {
        lostPlayerTime = saveableData.investigateStateLostPlayerTime;
        topNode.loadData(saveableData);
    }
}
/// <summary>
/// when within certain distance of player can begin attacking
/// </summary>
public class AttackState : State
{
    float attackTime = 2;
    GenericAnt owner;
    protected bool attackDone = false;
    public AttackState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
    }
    public virtual void enter()
    {
        owner.sfxManager.AntAttack();
        attackTime = 2;
        //headNormRote = owner.headTransform.rotation;
        ////        owner.headTransform.LookAt(GameManager1.playerObj.transform);
        if (!owner.anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.AntAttack"))
        {
            owner.anim.SetTrigger("Attack");
            
        }
        attackDone = false;
    }

    public virtual void execute()
    {
        attackTime -= Time.deltaTime;
        if (attackTime <= owner.attackAnimLength)//when finished attacking add any damage to the appropriate segment
        {
            if (owner.NearSegment(owner.transform.position) && !attackDone && owner.nextPosTransform != null)
            {
                GameManager1.mCentipedeBody.RemoveSegment(100, owner.nextPosTransform.position);

                attackDone = true;
            }

            if (attackTime <= 0)
                owner.stateMachine.changeState(owner.stateMachine.Investigate);

        }
    }

    public virtual void exit()
    {
        attackTime = 2;
        //owner.headTransform.rotation = headNormRote;
    }

    public virtual void saveData(GenericAntData saveableData)
    {
        saveableData.bAttackDone = attackDone;
        saveableData.currAttackTime = attackTime;
    }

    public virtual void loadData(GenericAntData saveableData)
    {
        attackDone = saveableData.bAttackDone;
        attackTime = saveableData.currAttackTime;
    }
}

/// <summary>
/// called when lose all health, but not die
/// </summary>
public class DamageState : State
{
    float damageTime = 1.7f;
    GenericAnt owner;

    Node topNode;
    public DamageState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;

        CheckFleeValid checkFleeValid = new CheckFleeValid(owner);

        GetNextNode getNextNode = new GetNextNode(owner, true); //here also need node for determining the path
        MoveBackwards moveBackwards = new MoveBackwards(owner);//using the flee backwards code, actually move forwards
        Sequence doMovement = new Sequence(new List<Node>{ getNextNode, moveBackwards});

        Sequence doFlee = new Sequence(new List<Node> { checkFleeValid, doMovement }); //picks a point so far enough away from the player, and then moves towards it with some speed

        ////Sequence doStay = new Sequence(); //stay still, play the damage animation and continue

        Selector fleeOrStay = new Selector(new List<Node> { doFlee });

        topNode = fleeOrStay;
    }
    public void enter()
    {
        damageTime = 1.7f;
        owner.anim.SetTrigger("Dazed");
    }

    public void execute()
    {
        topNode.execute();
        damageTime -= Time.deltaTime;
        if (damageTime <= 0)//when finished attacking add any damage to the appropriate segment
        {
            owner.stateMachine.changeState(owner.stateMachine.Investigate);
        }
    }

    public void exit()
    {
        damageTime = 1.7f;
    }

    public void loadData(GenericAntData saveableData)
    {
        damageTime = saveableData.currDamageTime;
        topNode.loadData(saveableData);
    }

    public void saveData(GenericAntData saveableData)
    {
        saveableData.currDamageTime = damageTime;
        topNode.saveData(saveableData);
    }
}

/// <summary>
/// called when lost 100% of the health
/// </summary>
public class DeadState : State
{
    float deadTime = 3;
    GenericAnt owner;
    public DeadState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
    }
    public void enter()
    {
        deadTime = 3;
        if (!owner.anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.AntDead"))
            owner.anim.SetTrigger("Dead");
        Debug.Log("Guard is now dead");
    }

    public virtual void execute()
    {
        deadTime -= Time.deltaTime;
        if (deadTime <= 0)//when finished attacking add any damage to the appropriate segment
        {
            dropWeapon();
            MonoBehaviour.Destroy(owner.gameObject);
        }
    }

    public virtual void dropWeapon()
    {

    }

    public void exit()
    {
        deadTime = 3;
    }

    public void saveData(GenericAntData saveableData)
    {
        saveableData.currDeadTime = deadTime;
    }

    public void loadData(GenericAntData saveableData)
    {
        deadTime = saveableData.currDeadTime;
    }
}


/// <summary>
/// The attack state of the hunter ant, where it uses a weapon to do its attacking
/// </summary>
public class HunterAttack : AttackState
{
    HunterAnt owner;
    float shootDelay = 0.25f;
    public HunterAttack(GenericAnt owner) : base(owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner.gameObject.GetComponent<HunterAnt>();
    }
    public override void enter()
    {
        owner = owner.gameObject.GetComponent<HunterAnt>();
        shootDelay = 0.5f;
        owner.anim.SetTrigger("Idle");
    }

    public override void execute()
    {
        shootDelay -= Time.deltaTime;
        if (owner.nextPosTransform == null || Vector3.Distance(owner.transform.position, GameManager1.mCentipedeBody.Head.position) < 7|| Vector3.Distance(owner.transform.position, owner.nextPosTransform.position) > 18)
            owner.stateMachine.changeState(owner.stateMachine.Investigate);
        else if (shootDelay <= 0)
        {
            shootDelay = 0.5f;
//            Debug.Log(owner.nextPosTransform.gameObject.name);
            owner.weaponClass.LookAt(owner.nextPosTransform.position);
            owner.weaponClass.Fire(owner.nextPosTransform.position); //fire at the players segment
        }
    }

    public override void exit()
    {
        shootDelay = 0.5f;
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
        ////HunterAntData hunterAntData = saveableData as HunterAntData;
        ////shootDelay = hunterAntData.currShootDelay;
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
      ////  HunterAntData hunterAntData = saveableData as HunterAntData;
      ////  hunterAntData.currShootDelay = shootDelay;
        //not 100% sure this will work correctly though
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}

public class HunterInvestigate : InvestigateState
{
    new HunterAnt owner;
    public HunterInvestigate(GenericAnt owner):base(owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner.gameObject.GetComponent<HunterAnt>();

        DetermineAttackSeg determineAttackSeg = new DetermineAttackSeg(owner);

        PathToSegment pathToSegment = new PathToSegment(owner);
        MoveTowards moveTowards = new MoveTowards(owner, false);

        CanCallBackup canCallBackup = new CanCallBackup(owner);
        CallBackup callBackup = new CallBackup(owner);
        Sequence callBackupSeq = new Sequence(new List<Node> { canCallBackup, callBackup });
        Succeeder backupSucceeder = new Succeeder(callBackupSeq);

        Sequence headToPlayer = new Sequence(new List<Node> { pathToSegment, moveTowards });

        MoveBackwards moveBackwards = new MoveBackwards(owner);
        PlayerTooClose playerTooClose = new PlayerTooClose(owner);
        Sequence headAwayFromPlayer = new Sequence(new List<Node> { playerTooClose, moveBackwards }); //if player too close move backwards for X seconds

        Selector chooseMovement = new Selector(new List<Node> {headAwayFromPlayer , headToPlayer}); //do you head towards or move away from the player

        Sequence moveSequence = new Sequence(new List<Node> { chooseMovement, backupSucceeder });
        RepeatUntilFail repeatUntilFail = new RepeatUntilFail(moveSequence);

        topNode = new Sequence(new List<Node> { determineAttackSeg, repeatUntilFail });
    }

    public override bool checkAttack()
    {
        return base.checkAttack() && !owner.isFleeing;
    }

    public override void saveData(GenericAntData saveableData)
    {
        saveableData.investigateStateLostPlayerTime = lostPlayerTime;
        topNode.saveData(saveableData);
    }

    public override void loadData(GenericAntData saveableData)
    {
        lostPlayerTime = saveableData.investigateStateLostPlayerTime;
        topNode.loadData(saveableData);
    }
}

/// <summary>
/// the dead state of the huner, should drop its weapon here
/// </summary>
public class HunterDead : DeadState
{
    HunterAnt owner;
    public HunterDead(GenericAnt owner) : base(owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner.gameObject.GetComponent<HunterAnt>();
    }

    public override void dropWeapon()
    {
        owner.DropWeapon();
    }
}

public class GuardDead : DeadState
{
    GuardAnt owner;
    public GuardDead(GenericAnt owner) : base(owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner.gameObject.GetComponent<GuardAnt>();
    }

    public override void dropWeapon()
    {
        owner.DropParentSeg();
    }
}

/// <summary>
/// guards attack state, basically just remove more health, and at a different speed
/// </summary>
public class GuardAttack : AttackState
{
    float attackTime = 2.5f;
    GenericAnt owner;
    public GuardAttack(GenericAnt owner) : base(owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
    }
    public override void enter()
    {
        base.enter();
        attackTime = 2.5f;
    }

    public override void execute()
    {
        attackTime -= Time.deltaTime;
        if (attackTime <= owner.attackAnimLength)//when finished attacking add any damage to the appropriate segment
        {
            if (owner.NearSegment(owner.transform.position) && !attackDone)
            {
                if (owner.nextPosTransform != null)
                    GameManager1.mCentipedeBody.RemoveSegment(100, owner.nextPosTransform.position);
                if (owner.nextPosTransform != null)
                    GameManager1.mCentipedeBody.RemoveSegment(100, owner.nextPosTransform.position);
                attackDone = true;
            }
            if (attackTime <= 0)
                owner.stateMachine.changeState(owner.stateMachine.Investigate);
        }
    }

    public override void exit()
    {
        base.exit();
        attackTime = 2.5f;
    }

    public override void saveData(GenericAntData saveableData)
    {
        saveableData.currAttackTime = attackTime;
        saveableData.bAttackDone = attackDone;
    }

    public override void loadData(GenericAntData saveableData)
    {
        attackTime = saveableData.currAttackTime;
        attackDone = saveableData.bAttackDone;
    }
}

/// <summary>
/// guard investigate, same as normal but with no call for backup
/// </summary>
public class GuardInvestigate : InvestigateState
{
    public GuardInvestigate(GenericAnt owner) : base(owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;

        DetermineAttackSeg determineAttackSeg = new DetermineAttackSeg(owner);

        PathToSegment pathToSegment = new PathToSegment(owner);
        MoveTowards moveTowards = new MoveTowards(owner, false);

        Sequence moveSequence = new Sequence(new List<Node> { pathToSegment, moveTowards });
        RepeatUntilFail repeatUntilFail = new RepeatUntilFail(moveSequence);

        topNode = new Sequence(new List<Node> { determineAttackSeg, repeatUntilFail });
    }

    public override void saveData(GenericAntData saveableData)
    {
        topNode.saveData(saveableData);
    }

    public override void loadData(GenericAntData saveableData)
    {
        topNode.loadData(saveableData);
    }
}

/// <summary>
/// when see player run fast towards them
/// </summary>
public class DasherInvestigate : InvestigateState
{
    DasherAnt dashOwner;
    public DasherInvestigate(DasherAnt owner) : base(owner)
    {
        this.owner = owner;
        dashOwner = owner.gameObject.GetComponent<DasherAnt>();
    }

    public override void enter()
    {
        owner.Speed = dashOwner.dashSpeed;
        owner.rotSpeed = dashOwner.dashRoteSpeed;
        owner.animMultiplier = dashOwner.dashAnimSpeed;
        base.enter();
    }

    public override void exit()
    {
        owner.Speed = dashOwner.tempSpeed;
        owner.rotSpeed = dashOwner.tempRoteSpeed;
        owner.animMultiplier = dashOwner.tempAnimSpeed;
        base.exit();
    }
    ///////for dasher ant not sure actually need to save these values here, probs better in the default dasher ant class instead
}

/// <summary>
/// for the bomb and wait few seconds and then explode
/// </summary>
public class BombAttack : AttackState
{
    float timeTilExplode = 4f;
    GenericAnt owner;
    public BombAttack(GenericAnt owner) : base(owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
    }
    public override void enter()
    {
        timeTilExplode = 4;
        //attach to the players segment
        owner.transform.parent = owner.nextPosTransform.GetChild(0);
        owner.transform.GetChild(0).GetComponent<Collider>().enabled = false;

        owner.anim.SetTrigger("Core"); //play the explode animation
        Debug.Log("Set the bomb up to explode...");
    }

    public override void execute()
    {
        timeTilExplode -= Time.deltaTime;
        //owner.transform.localScale += new Vector3(4-timeTilExplode, 4-timeTilExplode, 4-timeTilExplode);
        if (timeTilExplode <= 0 && owner.transform.parent.parent.CompareTag("PlayerSegment"))//when finished explode, need to add a check to ensure its near the player
        {
            if (owner.nextPosTransform != null)
                GameManager1.mCentipedeBody.RemoveSegment(100, owner.nextPosTransform.position);
            if (owner.nextPosTransform != null)
                GameManager1.mCentipedeBody.RemoveSegment(100, owner.nextPosTransform.position);
            if (owner.nextPosTransform != null)
                GameManager1.mCentipedeBody.RemoveSegment(100, owner.nextPosTransform.position);
            if (owner.nextPosTransform != null)
                GameManager1.mCentipedeBody.RemoveSegment(100, owner.nextPosTransform.position);

            owner.transform.parent = null;
            owner.stateMachine.changeState(owner.stateMachine.Dead);
        }
    }

    public override void exit()
    {
        base.exit();
    }


    /////////when get to it add in the data for saving the bombs state stuff
}


// <summary>
/// Called when AI first sees player and it showcases warning above head
/// </summary>
public class SpawnInState : State
{
    float waitTime = 1;
    GenericAnt owner;
    public SpawnInState(GenericAnt owner)
    {
        this.owner = owner;
    }
    public void enter()
    {
        waitTime = 1;
        owner.anim.SetTrigger("SpawnIn");
    }

    public void execute()
    {
        //if (owner.backupRing.activeSelf)
        owner.backupRing.SetActive(false);
        waitTime -= Time.deltaTime;
        if (waitTime <= 0)
            owner.stateMachine.changeState(owner.stateMachine.Investigate);

    }

    public void exit()
    {
        waitTime = 1;
    }

    public void loadData(GenericAntData saveableData)
    {
        waitTime = saveableData.spawnInWaitTime;
    }

    public void saveData(GenericAntData saveableData)
    {
        saveableData.spawnInWaitTime = waitTime;
    }
}