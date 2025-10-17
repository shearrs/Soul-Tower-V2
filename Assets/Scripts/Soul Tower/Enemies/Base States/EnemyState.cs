using Shears;
using Shears.Detection;
using Shears.Logging;
using Shears.Pathfinding;
using Shears.StateMachineGraphs;
using SoulTower.Towers;
using SoulTower.Traps;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SoulTower.Enemies
{
    public abstract class EnemyState : State
    {
        private static readonly int BLEND_PARAMETER = Animator.StringToHash("blend");

        private Enemy enemy;
        private StateMachine stateMachine;
        private EnemyModel model;

        private Animator Animator => model.Animator;
        protected Room CurrentRoom => enemy.CurrentRoom;

        public void Initialize(Enemy enemy, StateMachine stateMachine)
        {
            this.enemy = enemy;
            model = enemy.Model;
            this.stateMachine = stateMachine;
        }

        #region State Control
        protected void EnterState(EnemyState state) => stateMachine.EnterState(state);

        protected void EnterStateOfType<T>() where T : EnemyState => stateMachine.EnterStateOfType<T>();

        protected bool IsInState(State state) => stateMachine.IsInState(state);

        protected bool IsInStateOfType<T>() where T : EnemyState => stateMachine.IsInStateOfType<T>();
        #endregion

        #region Positioning
        protected bool IsAtCatalyst()
        {
            return enemy.CurrentRoom != null && enemy.CurrentRoom.HasCatalyst && enemy.TargetAttackPoint != null && enemy.transform.position == enemy.TargetAttackPoint.Position;
        }

        protected bool IsAtExitDoor()
        {
            return enemy.CurrentRoom != null && enemy.CurrentRoom.HasExitDoor && enemy.IsAtNodePosition(enemy.CurrentRoom.ExitDoorPosition);
        }
        
        protected void StandardPathUpdate(EnemyPathfinder pathfinder, List<PathNode> path, List<TowerNodeData> registeredNodes)
        {
            if (CurrentRoom == null)
                return;

            foreach (var nodeData in registeredNodes)
                nodeData?.DeregisterEntity(enemy);

            registeredNodes.Clear();

            Vector3 targetPosition;

            if (CurrentRoom.HasExitDoor)
                targetPosition = CurrentRoom.ExitDoorPosition;
            else if (CurrentRoom.HasCatalyst)
            {
                if (enemy.TargetAttackPoint == null)
                {
                    enemy.TargetAttackPoint = CurrentRoom.Catalyst.GetEmptiestAttackPoint();
                    enemy.TargetAttackPoint.RegisterEntity(enemy);
                }

                targetPosition = enemy.TargetAttackPoint.Position;
            }
            else
            {
                Log("Room has no exit door or catalyst!", SHLogLevels.Error);

                path.Clear();
                return;
            }

            pathfinder.GetPath(enemy.transform.position, targetPosition, path);

            foreach (var node in path)
            {
                if (node.TryGetData(out TowerNodeData nodeData))
                {
                    nodeData.RegisterEntity(enemy);
                    registeredNodes.Add(nodeData);
                }
            }
        }
        
        protected void StandardPathFollow(List<PathNode> path)
        {
            if (!StandardMovementValidation(path))
                return;

            var node = path[0];
            Vector3 targetPosition = enemy.GetNodePosition(node.WorldPosition);

            StandardMoveAndRotate(targetPosition);

            if (enemy.transform.position == targetPosition)
            {
                enemy.transform.position = targetPosition;
                path.RemoveAt(0);
            }
        }

        protected void StandardMoveAndRotate(Vector3 targetPosition, float? moveSpeed = null)
        {
            float speed;

            if (moveSpeed != null)
                speed = moveSpeed.Value;
            else
                speed = enemy.MoveSpeed;

            Vector3 lookDirection = (targetPosition - enemy.transform.position).normalized.XZ();
            Quaternion rotation = enemy.transform.rotation;

            if (lookDirection != Vector3.zero)
            {
                rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                rotation = Quaternion.RotateTowards(enemy.transform.rotation, rotation, enemy.RotationSpeed);
            }

            enemy.transform.SetPositionAndRotation
            (
                Vector3.MoveTowards(enemy.transform.position, targetPosition, speed * Time.deltaTime), 
                rotation
            );
        }

        protected void StandardMove(Vector3 targetPosition, float? moveSpeed = null)
        {
            float speed = moveSpeed != null ? moveSpeed.Value : enemy.MoveSpeed;

            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, speed * Time.deltaTime);
        }

        protected void StandardRotateToMovement(Vector3 targetPosition)
        {
            Vector3 lookDirection = (targetPosition - enemy.transform.position).normalized.XZ();
            Quaternion rotation = enemy.transform.rotation;

            if (lookDirection != Vector3.zero)
            {
                rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                rotation = Quaternion.RotateTowards(enemy.transform.rotation, rotation, enemy.RotationSpeed);
            }

            enemy.transform.rotation = rotation;
        }

        protected void StandardRotate(Quaternion targetRotation, float? rotationSpeed = null)
        {
            float speed = rotationSpeed != null ? rotationSpeed.Value : enemy.RotationSpeed;

            Quaternion rotation = enemy.transform.rotation;
            rotation = Quaternion.RotateTowards(rotation, targetRotation, speed);

            enemy.transform.rotation = rotation;
        }

        protected bool StandardMovementValidation(List<PathNode> path)
        {
            if (CurrentRoom != null)
            {
                if (IsAtCatalyst())
                {
                    EnterStateOfType<EnemyCatalystState>();
                    return false;
                }
                else if (IsAtExitDoor())
                {
                    EnterStateOfType<EnemyStairsState>();
                    return false;
                }
            }

            if (path.Count == 0)
            {
                Log($"Enemy {enemy.name} cannot find path!", SHLogLevels.Warning, context: enemy);
                return false;
            }

            return true;
        }
        #endregion

        #region Animation
        protected bool IsInAnimation(SpeedAnimation anim)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == anim.ID && !Animator.IsInTransition(0))
                return true;
            else
                return false;
        }

        protected void CrossFade(SpeedAnimation anim, float fadeDuration)
        {
            if (IsInAnimation(anim))
                return;
            
            Animator.CrossFade(anim.ID, fadeDuration);
        }

        protected void SetAnimationSpeed(float speed)
        {
            Animator.speed = speed;
        }
        
        protected void SetBlend(float value)
        {
            Animator.SetFloat(BLEND_PARAMETER, value);
        }
        #endregion

        protected static bool DetectThreats(AreaDetector3D frontDetector, AreaDetector3D bodyDetector)
        {
            bool threat = false;

            if (frontDetector.Detect())
            {
                bodyDetector.Detect();

                if (frontDetector.TryGetDetection(out TrapThreatArea threatArea) && !bodyDetector.TryGetDetection(out TrapThreatArea _))
                    threat = threatArea.IsActive;
            }

            return threat;
        }
    }
}
