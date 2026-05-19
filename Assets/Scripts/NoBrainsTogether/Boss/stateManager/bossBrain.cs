using UnityEngine;

public class BossBrain : MonoBehaviour
{
    [Header("Components")]
    public BossMovement Movement;
    public BossHealth Health;
    public BossWeakPointComponent WeakPoints;

    [Header("Players")]
    public Transform Player1;
    public Transform Player2;

    // Current State
    private IBossState currentState;

    // States
    public MoveState MoveState { get; private set; }

    public ShootState ShootState { get; private set; }

    public SlamState SlamState { get; private set; }

    public ChaseState ChaseState { get; private set; }

    public Down1State Down1State { get; private set; }

    public Down2State Down2State { get; private set; }

    public Down3State Down3State { get; private set; }

    public DeadState DeadState { get; private set; }

    private void Awake()
    {
        InitializeStates();
    }

    private void Start()
    {
        ChangeState(MoveState);

        Health.OnDeath += HandleDeath;
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(IBossState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();

        Debug.Log($"Boss changed state to {newState.GetType().Name}");
    }

    private void InitializeStates()
    {
        MoveState = new MoveState(this);

        ShootState = new ShootState(this);

        SlamState = new SlamState(this);

        ChaseState = new ChaseState(this);

        Down1State = new Down1State(this);

        Down2State = new Down2State(this);

        Down3State = new Down3State(this);

        DeadState = new DeadState(this);
    }

    private void HandleDeath()
    {
        ChangeState(DeadState);
    }
}