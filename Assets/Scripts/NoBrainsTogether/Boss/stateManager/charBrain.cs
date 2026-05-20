using UnityEngine;

public class charBrain : MonoBehaviour
{
    public enum CharacterType
    {
        Boss,
    }

    [Header("Character Type")]
    [SerializeField] private CharacterType characterType;

    [Header("Components")]
    public Movement Movement;
    public Health Health;
    public BossWeakPointComponent WeakPoints;

    [Header("Players")]
    public Transform Player1;
    public Transform Player2;

    // Current State
    private IState currentState;

    // States
    public bossMoveState MoveState { get; private set; }

    public bossShootState ShootState { get; private set; }

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
        StartingState();

        Health.OnDeath += HandleDeath;
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();

        Debug.Log($"Boss changed state to {newState.GetType().Name}");
    }

    private void InitializeStates()
    {
        switch (characterType)
        {
            case CharacterType.Boss:
                MoveState = new bossMoveState(this);
                ShootState = new bossShootState(this);
                SlamState = new SlamState(this);
                ChaseState = new ChaseState(this);
                Down1State = new Down1State(this);
                Down2State = new Down2State(this);
                Down3State = new Down3State(this);
                DeadState = new DeadState(this);
                break;
        }
    }
    private void StartingState()
    {
        switch (characterType)
        {
            case CharacterType.Boss:
                ChangeState(MoveState);
                break;
        }
    }

    private void HandleDeath()
    {
        ChangeState(DeadState);
    }
}