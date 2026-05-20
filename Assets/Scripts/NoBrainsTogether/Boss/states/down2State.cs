using UnityEngine;

public class Down2State : BaseBossState
{
    private float timer = 5f;

    public Down2State(charBrain brain) : base(brain)
    {

    }

    public override void Enter()
    {
        Debug.Log("Boss DOWN 2");

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