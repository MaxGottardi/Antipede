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
    public void enter() //omly important when other states also added in
    {
        GameObject closestNode = owner.nodesList[0]; //find the next location to move towards
        foreach (GameObject node in owner.nodesList)
        {
            if (Vector3.Distance(node.transform.position, owner.transform.position) < Vector3.Distance(closestNode.transform.position, owner.transform.position))
                closestNode = node;
        }
        owner.nextPosTransform = closestNode.transform;

        //owner.shockBar.SetActive(false);
        //find the nearest node to this object as it would have been lost
        //stuff do when enter the state
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
}
/// <summary>
/// Called when AI first sees player and it showcases warning above head
/// </summary>
public class ShockState : State
{
    float shockTime = 0.45f;
    GenericAnt owner;
    public ShockState(GenericAnt owner)
    {
        this.owner = owner;
    }
    public void enter()
    {
        shockTime = 0.45f;
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
        shockTime = 0.45f;
        owner.shockBar.SetActive(false);
    }
}
/// <summary>
/// When seen player and moving towards it
/// </summary>
public class InvestigateState : State
{
    protected GenericAnt owner;
    float lostPlayerTime = 6.0f;

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
        lostPlayerTime = 6.0f;

        owner.callingBackup = false;

        topNode.execute();
    }
    public void execute()
    {
        if (owner.callBackupWait > 0) //update wating time between each call for beckup
            owner.callBackupWait -= Time.deltaTime;

        if (!owner.DetectPlayer())
            lostPlayerTime -= Time.deltaTime; //update amount of time not seen player for
        else if(lostPlayerTime < 6.0f)
            lostPlayerTime = 6.0f;

        if (!owner.callingBackup && lostPlayerTime <= 0) //when haven't seen player for 3 seconds or segment already destroyed
        {
            //if (owner.nextPosTransform && owner.nextPosTransform.gameObject.GetComponent<MSegment>())
            //    owner.nextPosTransform.gameObject.GetComponent<MSegment>().beingAttacked = false;
            owner.stateMachine.changeState(owner.stateMachine.Movement);
        }
        else if (checkAttack()) //within attack range of chosen player segment
        {
            owner.stateMachine.changeState(owner.stateMachine.Attack);
        }
        else //as no change of state occured, can run this one
        {
            Profiler.BeginSample("AI investigataion");
            topNode.execute();
            Profiler.EndSample();
        }
        
    }

    public virtual bool checkAttack()
    {
        return owner.NearSegment(owner.transform.position) && !owner.callingBackup;
    }

    public virtual void exit()
    {
        topNode.interupt();

        owner.shockBar.SetActive(false);
        lostPlayerTime = 0;
    }
}
/// <summary>
/// when within certain distance of player can begin attacking
/// </summary>
public class AttackState : State
{
    float attackTime = 2;
    GenericAnt owner;
    Quaternion headNormRote;

    protected bool attackDone = false;
    public AttackState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
    }
    public virtual void enter()
    {
        attackTime = 2;
        headNormRote = owner.headTransform.rotation;
        owner.headTransform.LookAt(owner.nextPosTransform);
        owner.anim.SetTrigger("Attack");
        attackDone = false;
    }

    public virtual void execute()
    {
        attackTime -= Time.deltaTime;
        if (attackTime <= owner.attackAnimLength)//when finished attacking add any damage to the appropriate segment
        {
            if (owner.NearSegment(owner.transform.position) && !attackDone)
            {
                GameManager1.mCentipedeBody.RemoveSegment(100);
                attackDone = true;
            }

            if (attackTime <= 0)
                owner.stateMachine.changeState(owner.stateMachine.Investigate);

        }
    }

    public virtual void exit()
    {
        attackTime = 2;
        owner.headTransform.rotation = headNormRote;
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
        owner.anim.SetTrigger("Dead");
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
}


/// <summary>
/// The attack state of the hunter ant, where it uses a weapon to do its attacking
/// </summary>
public class HunterAttack : AttackState
{
    HunterAnt owner;
    float shootDelay = 0.5f;
    public HunterAttack(GenericAnt owner) : base(owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner.gameObject.GetComponent<HunterAnt>();
    }
    public override void enter()
    {
        owner = owner.gameObject.GetComponent<HunterAnt>();
        shootDelay = 0.25f;
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

/// <summary>
/// guards attack state, basically just remove more health, and at a different speed
/// </summary>
public class GuardAttack : AttackState
{
    float attackTime = 1.25f;
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
                GameManager1.mCentipedeBody.RemoveSegment(100);
                GameManager1.mCentipedeBody.RemoveSegment(100);
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
        dashOwner.tempSpeed = owner.Speed;
        dashOwner.tempRoteSpeed = owner.rotSpeed;
        dashOwner.tempAnimSpeed = owner.animMultiplier;


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

        Vector3 oldLocalPos = owner.transform.localPosition;
        Quaternion oldLocalRot = owner.transform.localRotation;
        Vector3 oldLocalScale = owner.transform.localScale;
        owner.transform.parent = owner.nextPosTransform.GetChild(0);//, true);

        //owner.transform.rotation = oldLocalRot;
        //owner.transform.position = oldLocalPos;
        //owner.transform.localScale = oldLocalScale;

        owner.transform.GetChild(0).GetComponent<Collider>().enabled = false;

        owner.anim.SetTrigger("Core"); //play the explode animation
    }

    public override void execute()
    {
        timeTilExplode -= Time.deltaTime;
        //owner.transform.localScale += new Vector3(4-timeTilExplode, 4-timeTilExplode, 4-timeTilExplode);
        if (timeTilExplode <= 0)//when finished explode
        {
            GameManager1.mCentipedeBody.RemoveSegment(100);
            GameManager1.mCentipedeBody.RemoveSegment(100);
            GameManager1.mCentipedeBody.RemoveSegment(100);
            GameManager1.mCentipedeBody.RemoveSegment(100);

            owner.transform.parent = null;
            owner.stateMachine.changeState(owner.stateMachine.Dead);
        }
    }

    public override void exit()
    {
        base.exit();
    }
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
        waitTime = 0.45f;
    }
}