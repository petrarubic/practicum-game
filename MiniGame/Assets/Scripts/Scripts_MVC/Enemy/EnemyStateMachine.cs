//TODO: Remove EnemyController3 and create IEnemyController interface to decouple the code

using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    public float distanceFromTarget;
    public float health;
    public EnemyController3 controller;
    public bool isFrozen;

    private EnemyFrozenStateMachine frozenStateMachine;

    public EnemyStateMachine()
    {
        var idleState = new EnemyIdleState();
        AddState(idleState);

        var chaseState = new EnemyChaseState();
        AddState(chaseState);

        var attackState = new EnemyAttackState();
        AddState(attackState);

        var deadState = new EnemyDeadState();
        AddState(deadState);

        var enrageState = new EnemyEnrageState();
        AddState(enrageState);

        var anyState = new EnemyAnyState();
        AddAnyState(anyState);

        //sub-state machine
        frozenStateMachine = new EnemyFrozenStateMachine();
        AddState(frozenStateMachine);

        frozenStateMachine.controller = controller;
        frozenStateMachine.distanceFromTarget = distanceFromTarget;

        InitializeMachine(typeof(EnemyIdleState));
    }

    public override void Update()
    {
        base.Update();
        frozenStateMachine.controller = controller;
        frozenStateMachine.distanceFromTarget = distanceFromTarget;
    }
}

public class EnemyFrozenStateMachine : StateMachine
{
    public EnemyController3 controller;
    public float distanceFromTarget;

    public EnemyFrozenStateMachine()
    {
        var idleState = new EnemyFrozenIdleState();
        AddState(idleState);

        var attackState = new EnemyFrozenAttackState();
        AddState(attackState);

        InitializeMachine(typeof(EnemyFrozenIdleState));
    }

    public override void SetupTransitions()
    {
        var parent = (EnemyStateMachine)parentMachine;

        var toIdle = new StateMachineTransition(typeof(EnemyIdleState), () => !parent.isFrozen);
        AddTransition(toIdle);
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        Debug.Log("Entered FROZEN");
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        Debug.Log("Exit FROZEN");
    }
}

public class EnemyFrozenIdleState : StateMachineState
{
    public override void SetupTransitions()
    {
        var parent = (EnemyFrozenStateMachine)parentMachine;

        var toAttack = new StateMachineTransition(typeof(EnemyFrozenAttackState), () => parent.distanceFromTarget < 5);
        AddTransition(toAttack);
    }
}

public class EnemyFrozenAttackState : StateMachineState
{
    EnemyFrozenStateMachine parent;

    public override void SetupTransitions()
    {
        parent = (EnemyFrozenStateMachine)parentMachine;

        var toIdle = new StateMachineTransition(typeof(EnemyFrozenIdleState), () => parent.distanceFromTarget > 5);
        AddTransition(toIdle);
    }

    public override void Update()
    {
        parent.controller.transform.LookAt(parent.controller.target);
        parent.controller.Shoot();
    }

    public override void OnStateEnter()
    {
        parent.controller.SetFirerate(10);
    }

    public override void OnStateExit()
    {
        parent.controller.SetFirerate(parent.controller.initialFirerate);
    }
}
