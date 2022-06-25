using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface State
{
    public void enter(); //runs when first entre the state
    public void execute(); //update code for when running the state
    public void exit(); //runs when leaving the state

    public void saveData(ref GenericAntData saveableData);
    public void loadData(ref GenericAntData saveableData);

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
    public void saveData(ref GenericAntData saveableData)
    {
        Movement.saveData(ref saveableData);
        Shock.saveData(ref saveableData);
        Investigate.saveData(ref saveableData);
        Attack.saveData(ref saveableData);
        Damage.saveData(ref saveableData);
        Dead.saveData(ref saveableData);
        SpawnIn.saveData(ref saveableData);
    }

    //load in the data from a file and set it to the appropriate states
    public void loadData(ref GenericAntData saveableData)
    {
        Movement.loadData(ref saveableData);
        Shock.loadData(ref saveableData);
        Investigate.loadData(ref saveableData);
        Attack.loadData(ref saveableData);
        Damage.loadData(ref saveableData);
        Dead.loadData(ref saveableData);
        SpawnIn.loadData(ref saveableData);
    }
}
