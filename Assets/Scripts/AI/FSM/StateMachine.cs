using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface State
{
    public void enter(); //runs when first entre the state
    public void execute(); //update code for when running the state
    public void exit(); //runs when leaving the state

}
public class StateMachine
{
    public State currState;
    //contain a list of all possible states
    public State Movement, Investigate, Attack;
    GenericAnt owner;

    public StateMachine(GenericAnt owner)
    {
        this.owner = owner;
        Movement = new MovementState(owner);
        Investigate = new InvestigateState(owner);
        Attack = new AttackState(owner);
    }
    public void changeState(State newState)
    {
        if (currState != null)
            currState.exit();
        currState = newState;
        currState.enter();
    }

    public void Update()
    {
        if (currState != null) 
            currState.execute();
    }
}
