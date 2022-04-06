using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetermineShockState
{
    //Node topNode;
    public float smallShockDist = 5.0f, medShockDist = 3f, largeShockDist = 1f;
    int state = 0;
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
        if (Vector3.Distance(owner.transform.position, owner.newNode.transform.position) < largeShockDist)
        {
            owner.shockBars[0].SetActive(true);
            owner.shockBars[1].SetActive(true);
            owner.shockBars[2].SetActive(true);
            state = 3;
            expressingShock = true;
        }
        else if (Vector3.Distance(owner.transform.position, owner.newNode.transform.position) < medShockDist)
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
            Debug.Log("Running shocked player");
        }
    }

    void Movement()
    {
        owner.transform.position += owner.transform.forward * Time.deltaTime * owner.Speed;
        Vector3 lookPos = owner.newNode.transform.position;
        lookPos.y = owner.transform.position.y;

        owner.transform.LookAt(lookPos);
    }

    void AdjustShock()
    {
        if (state == 1 && Vector3.Distance(owner.transform.position, owner.newNode.transform.position) < medShockDist)
        {
            state = 2;
            owner.shockBars[1].SetActive(true);
            expressingShock = true;
        }
        if (state == 2 && Vector3.Distance(owner.transform.position, owner.newNode.transform.position) < largeShockDist)
        {
            owner.shockBars[2].SetActive(true);
            state = 3;
            expressingShock = true;
            //call for backup
        }
        if (state == 3 && Vector3.Distance(owner.transform.position, owner.newNode.transform.position) < owner.attachDist)
        {
            //switch to the attacking state here

        }

    }

    public void exit()
    {
        //deactivate all showing warning symbols
        //throw new System.NotImplementedException();
    }
}