using Unity.VisualScripting;
using UnityEngine;

public class bossMoveState : BaseBossState
{
    private float waitTimer;


    public bossMoveState(charBrain brain) : base(brain)
    {

    }

    public override void Enter()
    {
        movement.MoveTo(RandomPoints());

        waitTimer = 1.5f;
    }

    public override void Update()
    {
        if (movement.HasReachedTarget())
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                ChooseRandomAttack();
            }
        }
    }

    private void ChooseRandomAttack()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                brain.ChangeState(brain.ShootState);
                break;

            case 1:
                brain.ChangeState(brain.SlamState);
                break;

            case 2:
                brain.ChangeState(brain.ChaseState);
                break;
        }
    }

    public Vector2 RandomPoints()
    {
        float x = Random.Range(-5f, 5f);
        float y = Random.Range(-3f, 3f);
        return new Vector2(x, y);
    }

    public override void Exit()
    {
        movement.StopMoving();
    }
}