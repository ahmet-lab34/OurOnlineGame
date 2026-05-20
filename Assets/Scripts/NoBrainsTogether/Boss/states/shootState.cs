using UnityEngine;

public class bossShootState : BaseBossState
{
    BossAttackSystem attackSystem;
    private float attackDuration = 3f;

    public bossShootState(charBrain brain) : base(brain)
    {
        
    }

    public override void Enter()
    {
        if (attackSystem == null) {
            attackSystem = brain.GetComponent<BossAttackSystem>();
        }

        Debug.Log("Boss started shooting");
        attackSystem.StartShoot();
    }

    public override void Update()
    {
        attackDuration -= Time.deltaTime;

        // SHOOT LOGIC HERE
        attackSystem.Shoot();


        if (attackDuration <= 0)
        {
            brain.ChangeState(brain.MoveState);
        }
    }
    public  override void Exit()
    {
        attackDuration = 3f;
        attackSystem.StopShoot();
        Debug.Log("Boss stopped shooting");
    }
}