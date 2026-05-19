//using UnityEngine;
//
//[RequireComponent(typeof(BossAttackSystem))]
//public class ChaseSystem : BaseBossState
//{
//    private Transform target;
//
//    public ChaseSystem(BossBrain brain) : base(brain) { }
//
//    public override void Enter()
//    {
//        target = Random.value > 0.5f ? player1 : player2;
//
//        brain.GetComponent<BossAttackSystem>().StartChase(target);
//    }
//
//    public override void Update()
//    {
//        brain.GetComponent<BossAttackSystem>().UpdateChase();
//
//        if (weakPoints.AllWeakPointsDestroyed)
//        {
//            brain.ChangeState(brain.Down1State);
//        }
//    }
//
//    public override void Exit()
//    {
//        brain.GetComponent<BossAttackSystem>().StopChase();
//    }
//}
