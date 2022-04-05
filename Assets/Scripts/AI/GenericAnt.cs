using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAnt : MonoBehaviour
{
    public GameObject newNode;
    public float Speed = 1.5f, attachDist = 2.0f;


    [HideInInspector] public StateMachine stateMachine;
    void Start()
    {
        stateMachine = new StateMachine(this);
        stateMachine.changeState(stateMachine.Movement);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public bool DetectPlayer()
    {
        //player pos is the global position of a player
        //if found set the found pos to the player position
        return false;
    }
}
