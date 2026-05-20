using UnityEngine;

public class DeadState : BaseBossState
{
    public DeadState(BossBrain brain) : base(brain)
    {

    }

    public override void Enter()
    {
        Debug.Log("Boss Dead");

        movement.StopMoving();

        weakPoints.ClearWeakPoints();

        // death animation here
    }
}