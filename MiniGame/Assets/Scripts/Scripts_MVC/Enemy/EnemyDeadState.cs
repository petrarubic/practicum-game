public class EnemyDeadState : StateMachineState
{
    EnemyStateMachine parent;

    public override void SetupTransitions()
    {
        parent = (EnemyStateMachine)parentMachine;

        var toIdle = new StateMachineTransition(typeof(EnemyIdleState), () => parent.health > 0);
        AddTransition(toIdle);
    }
}
