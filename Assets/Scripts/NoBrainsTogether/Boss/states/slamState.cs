using UnityEngine;

public class SlamState : BaseBossState
{
    private float slamTimer = 2f;

    public SlamState(charBrain brain) : base(brain)
    {

    }

    public override void Enter()
    {
        Debug.Log("Boss slam started");
    }

    public override void Update()
    {
        slamTimer -= Time.deltaTime;

        // SLAM LOGIC HERE

        if (slamTimer <= 0)
        {
            brain.ChangeState(brain.MoveState);
        }
    }
    public override void Exit()
    {
        slamTimer = 2f;
        Debug.Log("Boss slam ended");
    }
}