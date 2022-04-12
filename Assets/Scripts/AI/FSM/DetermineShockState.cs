using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetermineShockState
{
    //Node topNode;
    public float smallShockDist = 6.0f, medShockDist = 4f, largeShockDist = 2f;
    int state = 0;
    float backupRadius = 7.5f;

    GenericAnt owner;
    ExpressShock shockWait;
    bool expressingShock = false;
    public DetermineShockState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;

        shockWait = new ExpressShock(owner);
    }
    public void enter()
    {
        Debug.Log("Running shocked player");

        //stuff do when enter the state
        if (Vector3.Distance(owner.transform.position, owner.nextPosTransform.transform.position) < largeShockDist)
        {
            owner.shockBars[0].SetActive(true);
            owner.shockBars[1].SetActive(true);
            owner.shockBars[2].SetActive(true);
            state = 3;
            expressingShock = true;
        }
        else if (Vector3.Distance(owner.transform.position, owner.nextPosTransform.transform.position) < medShockDist)
        {
            owner.shockBars[0].SetActive(true);
            owner.shockBars[1].SetActive(true);
            state = 2;
            expressingShock = true;

        }
        else
        {
            owner.shockBars[0].SetActive(true);
            state = 1;
            expressingShock = true;
        }
    }

    public void execute()
    {
        if (expressingShock)
        {
            Node.NodeState state = shockWait.evaluate();
            if (state == Node.NodeState.Success)
                expressingShock = false;
        }
        else
        {
            Movement();
            AdjustShock();
        }
    }

    void Movement()
    {
        owner.transform.position += owner.transform.forward * Time.deltaTime * owner.Speed;
        Vector3 lookPos = owner.nextPosTransform.transform.position;
        lookPos.y = owner.transform.position.y;

        owner.transform.LookAt(lookPos);
    }

    void AdjustShock()
    {
        if (state == 1 && Vector3.Distance(owner.transform.position, owner.nextPosTransform.transform.position) < medShockDist)
        {
            state = 2;
            owner.shockBars[1].SetActive(true);
            expressingShock = true;
            CallReinforcements();
        }
        if (state == 2 && Vector3.Distance(owner.transform.position, owner.nextPosTransform.transform.position) < largeShockDist)
        {
            owner.shockBars[2].SetActive(true);
            state = 3;
            expressingShock = true;
            //call for backup
        }
        if (state == 3 && Vector3.Distance(owner.transform.position, owner.nextPosTransform.transform.position) < owner.attachDist)
        {
            //switch to the attacking state here

        }
    }

    void CallReinforcements()
    { 
        Collider[] hitColliders = Physics.OverlapSphere(owner.transform.position, backupRadius);

        foreach (Collider obj in hitColliders)
        {
            if(obj.gameObject.CompareTag("Enemy") && obj.gameObject.GetComponent<GenericAnt>().stateMachine.currState == obj.gameObject.GetComponent<GenericAnt>().stateMachine.Movement)
            {
                obj.gameObject.GetComponent<GenericAnt>().isRienforcement = true;
                obj.gameObject.GetComponent<GenericAnt>().nextPosTransform = owner.nextPosTransform;
            }
        }
    }

    public void exit()
    {
        //deactivate all showing warning symbols
        //throw new System.NotImplementedException();
    }
}