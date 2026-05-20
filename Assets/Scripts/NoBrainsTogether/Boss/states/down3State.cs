using UnityEngine;

public class Down3State : BaseBossState
{
    private float timer = 5f;

    public Down3State(charBrain brain) : base(brain)
    {

    }

    public override void Enter()
    {
        Debug.Log("Boss DOWN 3");

        health.SetInvulnerable(false);
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            brain.ChangeState(brain.MoveState);
        }
    }

    public override void Exit()
    {
        health.SetInvulnerable(true);
    }
}