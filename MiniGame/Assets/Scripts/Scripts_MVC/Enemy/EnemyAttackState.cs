public class EnemyAttackState : StateMachineState
{
    EnemyStateMachine parent;

    public override void SetupTransitions()
    {
        parent = (EnemyStateMachine)parentMachine;

        var toChase = new StateMachineTransition(typeof(EnemyChaseState), () => { return parent.distanceFromTarget > 4; });
        AddTransition(toChase);
    }

    public override void Update()
    {
        parent.controller.transform.LookAt(parent.controller.target);
        parent.controller.Shoot();
    }
}
