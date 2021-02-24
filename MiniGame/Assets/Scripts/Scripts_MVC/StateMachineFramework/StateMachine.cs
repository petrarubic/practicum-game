using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TODO: Separate classes in their files

public class StateMachine : IStateMachineState
{
    public Dictionary<Type, IStateMachineState> states = new Dictionary<Type, IStateMachineState>();
    public IStateMachineState currentState;
    public IStateMachineState anyState;

    public List<StateMachineTransition> transitions = new List<StateMachineTransition>();
    public StateMachine parentMachine;


    public void InitializeMachine(Type initialState)
    {
        currentState = states[initialState];
        currentState.OnStateEnter();
    }

    public virtual void Update()
    {
        if (anyState != null)
        {
            ProcessTrasitionsForState(anyState);
        }


        ProcessTrasitionsForState(currentState);

        currentState.Update();
    }

    private void ProcessTrasitionsForState(IStateMachineState state)
    {
        foreach (var transtion in state.GetTransitions())
        {
            if (transtion.Evaluate())
            {
                ChangeState(states[transtion.TargetState]);
                return;
            }
        }
    }

    private void ChangeState(IStateMachineState state)
    {
        if (state == currentState) return;

        currentState.OnStateExit();
        state.OnStateEnter();
        currentState = state;
    }

    public void AddState(IStateMachineState state)
    {
        states[state.GetType()] = state;
        state.SetParent(this);
        state.SetupTransitions();
    }

    public void AddAnyState(IStateMachineState state)
    {
        anyState = state;
        anyState.SetParent(this);
        anyState.SetupTransitions();
    }

    //IStateMachineState interface
    public void SetParent(StateMachine parent)
    {
        parentMachine = parent;
    }

    public List<StateMachineTransition> GetTransitions()
    {
        return transitions;
    }

    public virtual void OnStateEnter()
    {
        currentState.OnStateEnter();
    }

    public virtual void OnStateExit()
    {
        currentState.OnStateExit();
    }

    public virtual void SetupTransitions()
    {
        
    }

    public void AddTransition(StateMachineTransition transition) 
    {
        transitions.Add(transition);
    }
}

public interface IStateMachineState
{
    void SetParent(StateMachine parent);
    List<StateMachineTransition> GetTransitions();
    void Update();
    void OnStateEnter();
    void OnStateExit();
    void SetupTransitions();
}

public abstract class StateMachineState : IStateMachineState
{
    public StateMachine parentMachine;

    public List<StateMachineTransition> transitions = new List<StateMachineTransition>();

    public void AddTransition(StateMachineTransition transition)
    {
        transitions.Add(transition);
    }

    public virtual void Update()
    {

    }
    public virtual void OnStateEnter()
    {

    }

    public virtual void OnStateExit()
    {

    }

    public abstract void SetupTransitions();

    public void SetParent(StateMachine parent)
    {
        this.parentMachine = parent;
    }

    public List<StateMachineTransition> GetTransitions()
    {
        return transitions;
    }
}

public class StateMachineTransition
{
    private Type targetState;
    public Type TargetState => targetState;

    private Func<bool> condition;

    public StateMachineTransition(Type target, Func<bool> condition)
    {
        this.targetState = target;
        this.condition = condition;
    }

    public bool Evaluate()
    {
        return condition();
    }
}