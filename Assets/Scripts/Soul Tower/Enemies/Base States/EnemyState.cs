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
            return enemy.CurrentRoom != null && enemy.CurrentRoom.HasCatalyst && enemy.IsAtNodePosition(enemy.CurrentRoom.CatalystPosition);
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
                targetPosition = CurrentRoom.CatalystPosition;
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
                rotation = Quaternion.RotateTowards(enemy.transform.rotation, rotation, model.RotationSpeed);
            }

            enemy.transform.SetPositionAndRotation
            (
                Vector3.MoveTowards(enemy.transform.position, targetPosition, speed * Time.deltaTime), 
                rotation
            );
        }

        protected void StandardMove(Vector3 targetPosition, float? moveSpeed = null)
        {
            float speed;

            if (moveSpeed != null)
                speed = moveSpeed.Value;
            else
                speed = enemy.MoveSpeed;

            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, speed * Time.deltaTime);
        }

        // TODO: smooth out rotations
        protected void StandardRotateToMovement(Vector3 targetPosition)
        {
            Vector3 lookDirection = (targetPosition - enemy.transform.position).normalized.XZ();
            Quaternion rotation = enemy.transform.rotation;

            if (lookDirection != Vector3.zero)
            {
                rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                rotation = Quaternion.RotateTowards(enemy.transform.rotation, rotation, model.RotationSpeed);
            }

            enemy.transform.rotation = rotation;
        }

        protected bool StandardMovementValidation(List<PathNode> path)
        {
            if (CurrentRoom == null)
            {
                Log("Current room is null!", SHLogLevels.Error);
                return false;
            }

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

            if (path.Count == 0)
            {
                Log($"Enemy {enemy.name} cannot find path!", SHLogLevels.Warning, context: enemy);
                return false;
            }

            return true;
        }
        #endregion

        #region Animation
        protected void CrossFade(SpeedAnimation anim, float fadeDuration)
        {
            Animator.speed = anim.Speed;
            Animator.CrossFade(anim.ID, fadeDuration);
        }

        protected void SetAnimationSpeed(float speed)
        {
            Animator.speed = speed;
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
