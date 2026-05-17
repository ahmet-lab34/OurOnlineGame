//using UnityEngine;
//
//[RequireComponent(typeof(BossAttackSystem))]
//public class SlamSystem : BaseBossState
//{
//    private Vector2 wallTarget;
//
//    public SlamSystem(BossBrain brain) : base(brain) { }
//
//    public override void Enter()
//    {
//        wallTarget = new Vector2(10f, brain.transform.position.y);
//
//        brain.GetComponent<BossAttackSystem>().StartSlam(wallTarget);
//    }
//
//    public override void Update()
//    {
//        if (brain.GetComponent<BossAttackSystem>().IsAtSlamTarget())
//        {
//            brain.GetComponent<BossAttackSystem>().ExecuteSlamEffect();
//
//            brain.ChangeState(brain.MoveState);
//        }
//    }
//
//    public override void Exit()
//    {
//        brain.GetComponent<BossAttackSystem>().StopSlam();
//    }
//}