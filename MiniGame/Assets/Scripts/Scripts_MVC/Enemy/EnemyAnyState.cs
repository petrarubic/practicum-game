public class EnemyAnyState : StateMachineState
{
    public override void SetupTransitions()
    {
        var parent = (EnemyStateMachine)parentMachine;

        var toDead = new StateMachineTransition(typeof(EnemyDeadState), () => parent.health <= 0);
        AddTransition(toDead);

        var toEnrage = new StateMachineTransition(typeof(EnemyEnrageState), () => parent.health > 0 && parent.health <= 3);
        AddTransition(toEnrage);

        var toFrozen = new StateMachineTransition(typeof(EnemyFrozenStateMachine), () => parent.isFrozen);
        AddTransition(toFrozen);
    }
}
