using Shears.Detection;
using Shears.Logging;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyCatalystState : EnemyState
    {
        private const float MAX_SQR_DISTANCE = 5.0f * 5.0f;
        private const float RANDOM_POSITION_OFFSET = 0.5f;

        private readonly Enemy enemy;
        private readonly SpeedAnimation animWalk;
        private readonly SpeedAnimation animCatalyst;
        private readonly EnemyState returnState;
        private Catalyst catalyst;
        private Vector3 targetPosition;

        public EnemyCatalystState(Enemy enemy, SpeedAnimation animWalk, SpeedAnimation animCatalyst, EnemyState returnState)
        {
            Name = "Catalyst State";

            this.enemy = enemy;
            this.animWalk = animWalk;
            this.animCatalyst = animCatalyst;
            this.returnState = returnState;
        }

        protected override void OnEnter()
        {
            if (!CurrentRoom.HasCatalyst)
            {
                Log("Current room does not have the catalyst!", SHLogLevels.Error);
                EnterState(returnState);

                return;
            }

            catalyst = CurrentRoom.Catalyst;
            var attackPoint = enemy.TargetAttackPoint;

            if (attackPoint == null)
            {
                attackPoint = catalyst.GetEmptiestAttackPoint();
                attackPoint.RegisterEntity(enemy);
                enemy.TargetAttackPoint = attackPoint;
            }

            targetPosition = attackPoint.Position;
            Vector2 randomOffset = Random.insideUnitCircle * RANDOM_POSITION_OFFSET;

            targetPosition.x += randomOffset.x;
            targetPosition.z += randomOffset.y;

            SetAnimationSpeed(animWalk.Speed);
            CrossFade(animWalk, 0.1f);
        }

        protected override void OnExit()
        {
            enemy.TargetAttackPoint?.DeregisterEntity(enemy);
            enemy.TargetAttackPoint = null;
        }

        protected override void OnUpdate()
        {
            float sqrDistance = (catalyst.transform.position - enemy.transform.position).sqrMagnitude;

            if (sqrDistance > MAX_SQR_DISTANCE)
            {
                EnterState(returnState);
                return;
            }

            bool inPosition = true;

            if (enemy.transform.position != targetPosition)
            {
                inPosition = false;
                StandardMoveAndRotate(targetPosition);
            }
            else if (enemy.transform.rotation != enemy.TargetAttackPoint.Rotation)
                StandardRotate(enemy.TargetAttackPoint.Rotation, enemy.Model.RotationSpeed * 2.0f);

            if (!inPosition)
                return;

            if (!IsInAnimation(animCatalyst))
            {
                SetAnimationSpeed(animCatalyst.Speed);
                CrossFade(animCatalyst, 0.1f);
            }    
        }
    }
}
