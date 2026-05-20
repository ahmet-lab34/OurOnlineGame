public abstract class BaseBossState : IState
{
    protected charBrain brain;

    protected Movement movement;

    protected Health health;

    protected BossWeakPointComponent weakPoints;

    protected UnityEngine.Transform player1;

    protected UnityEngine.Transform player2;

    public BaseBossState(charBrain brain)
    {
        this.brain = brain;

        movement = brain.Movement;

        health = brain.Health;

        weakPoints = brain.WeakPoints;

        player1 = brain.Player1;

        player2 = brain.Player2;
    }

    public virtual void Enter()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {

    }
}