public class EnemyEnrageState : StateMachineState
{
    EnemyStateMachine parent;

    public override void SetupTransitions()
    {
        parent = (EnemyStateMachine)parentMachine;

        var toIdle = new StateMachineTransition(typeof(EnemyIdleState), () => parent.health > 3);
        AddTransition(toIdle);
    }

    public override void Update()
    {
        parent.controller.transform.LookAt(parent.controller.target);
        parent.controller.Shoot();
    }

    public override void OnStateEnter()
    {
        parent.controller.SetFirerate(5);
    }

    public override void OnStateExit()
    {
        parent.controller.SetFirerate(parent.controller.initialFirerate);
    }
}
