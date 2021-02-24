using UnityEngine;

public class EnemyChaseState : StateMachineState
{
    EnemyStateMachine parent;

    public override void SetupTransitions()
    {
        parent = (EnemyStateMachine)parentMachine;

        var toIdle = new StateMachineTransition(typeof(EnemyIdleState), () => { return parent.distanceFromTarget > 8; });
        AddTransition(toIdle);

        var toAttack = new StateMachineTransition(typeof(EnemyAttackState), () => { return parent.distanceFromTarget < 4; });
        AddTransition(toAttack);
    }

    public override void Update()
    {
        parent.controller.transform.LookAt(parent.controller.target);

        var direction = parent.controller.target.position - parent.controller.transform.position;
        var movementVector = direction.normalized * Time.deltaTime * parent.controller.Speed;

        parent.controller.transform.position += movementVector;
    }
}
