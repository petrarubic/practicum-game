public class EnemyIdleState : StateMachineState
{
    public override void SetupTransitions()
    {
        var enemyStateMachine = (EnemyStateMachine)parentMachine;

        var toChase = new StateMachineTransition(typeof(EnemyChaseState), () => { return enemyStateMachine.distanceFromTarget < 8; });
        AddTransition(toChase);
    }
}
