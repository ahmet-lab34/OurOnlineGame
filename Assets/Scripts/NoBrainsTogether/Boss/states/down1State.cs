using UnityEngine;

public class Down1State : BaseBossState
{
    private float timer = 5f;

    public Down1State(BossBrain brain) : base(brain)
    {

    }

    public override void Enter()
    {
        Debug.Log("Boss DOWN 1");

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