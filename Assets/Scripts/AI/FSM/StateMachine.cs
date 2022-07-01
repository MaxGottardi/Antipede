using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface State
{
    public void enter(); //runs when first entre the state
    public void execute(); //update code for when running the state
    public void exit(); //runs when leaving the state

    public void saveData(GenericAntData saveableData);
    public void loadData(GenericAntData saveableData);

}
[System.Serializable]
public class StateMachine
{
    [SerializeField] public State currState;
    //contain a list of all possible states
    public State Movement, Shock, Investigate, Attack, Damage, Dead, SpawnIn;
    GenericAnt owner;

    public StateMachine(GenericAnt owner)
    {
        this.owner = owner;
        Movement = new MovementState(owner);
        Shock = new ShockState(owner);
        Investigate = new InvestigateState(owner);
        Attack = new AttackState(owner);
        Damage = new DamageState(owner);
        Dead = new DeadState(owner);
        SpawnIn = new SpawnInState(owner);
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

    //save the nessesary data for each state
    public void saveData(GenericAntData saveableData)
    {
        Movement.saveData(saveableData);
        Shock.saveData(saveableData);
        Investigate.saveData(saveableData);
        Attack.saveData(saveableData);
        Damage.saveData(saveableData);
        Dead.saveData(saveableData);
        SpawnIn.saveData(saveableData);
    }

    //load in the data from a file and set it to the appropriate states
    public void loadData(GenericAntData saveableData)
    {
        Movement.loadData(saveableData);
        Shock.loadData(saveableData);
        Investigate.loadData(saveableData);
        Attack.loadData(saveableData);
        Damage.loadData(saveableData);
        Dead.loadData(saveableData);
        SpawnIn.loadData(saveableData);
    }
}
