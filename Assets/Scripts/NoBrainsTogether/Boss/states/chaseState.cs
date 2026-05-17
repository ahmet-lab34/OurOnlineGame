using System.Collections;
using UnityEngine;

public class ChaseState : BaseBossState
{
    private Transform targetPlayer;

    public ChaseState(BossBrain brain) : base(brain)
    {

    }

    public override void Enter()
    {
        int random = Random.Range(0, 2);

        targetPlayer = random == 0 ? player1 : player2;

        weakPoints.SpawnWeakPoints(1, 1);

        Debug.Log($"Boss chasing {targetPlayer.name}");

        movement.SetMoveSpeed(2f);
    }

    public override void Update()
    {
        movement.MoveTo(targetPlayer.position);

        if (weakPoints.AllWeakPointsDestroyed)
        {
            brain.ChangeState(brain.Down1State);
        }
        
        if (Vector2.Distance(movement.transform.position, targetPlayer.position) <= 0.5f)
        {
            brain.ChangeState(brain.SlamState);
        }
    }

    public override void Exit()
    {
        movement.SetMoveSpeed(5f);
        weakPoints.ClearWeakPoints();
    }
}