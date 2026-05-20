//using System;
//using UnityEngine;
//
//[RequireComponent(typeof(BossAttackSystem))]
//public class ShootSystem : BaseBossState
//{
//    public ShootSystem(BossBrain brain) : base(brain) { }
//
//    public override void Enter()
//    {
//        brain.GetComponent<BossAttackSystem>().StartShoot();
//    }
//
//    public override void Update()
//    {
//        brain.GetComponent<BossAttackSystem>().Shoot();
//
//        if (/* timer or condition */ false)
//        {
//            brain.ChangeState(brain.MoveState);
//        }
//    }
//
//    public override void Exit()
//    {
//        brain.GetComponent<BossAttackSystem>().StopShoot();
//    }
//}
//
//internal class RequireFieldAttribute : Attribute
//{
//}