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

        protected Room CurrentRoom => enemy.CurrentRoom;

        public void Initialize(Enemy enemy, StateMachine stateMachine)
        {
            this.enemy = enemy;
            this.stateMachine = stateMachine;
        }

        #region State Control
        protected void EnterState(EnemyState state) => stateMachine.EnterState(state);

        protected void EnterStateOfType<T>() where T : EnemyState => stateMachine.EnterStateOfType<T>();

        protected bool IsInStateOfType<T>() where T : EnemyState => stateMachine.IsInStateOfType<T>();
        #endregion

        #region Positioning
        protected bool IsAtCatalyst()
        {
            return enemy.CurrentRoom != null && enemy.CurrentRoom.HasCatalyst && enemy.transform.position == enemy.CurrentRoom.CatalystPosition + enemy.HeightOffset;
        }

        protected bool IsAtExitDoor()
        {
            return enemy.CurrentRoom != null && enemy.CurrentRoom.HasExitDoor && enemy.transform.position == enemy.CurrentRoom.ExitDoorPosition + enemy.HeightOffset;
        }
        
        protected void StandardPathUpdate(EnemyPathfinder pathfinder, List<PathNode> path, List<TowerNodeData> registeredNodes)
        {
            if (CurrentRoom == null)
                return;

            foreach (var nodeData in registeredNodes)
                nodeData.DeregisterEntity(enemy);

            registeredNodes.Clear();

            Vector3 targetPosition = Vector3.zero;

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
        
        protected void StandardMove(List<PathNode> path)
        {
            if (!StandardMovementValidation(path))
                return;

            var node = path[0];
            Vector3 targetPosition = node.WorldPosition + enemy.HeightOffset;
            Vector3 heading = targetPosition - enemy.transform.position;
            float magnitude = heading.magnitude;
            Vector3 direction = heading / magnitude;
            float movement = enemy.MoveSpeed * Time.deltaTime;

            if (magnitude < movement)
                movement = magnitude;

            if (magnitude > 0.001f)
                enemy.transform.position += movement * direction;
            else
            {
                enemy.transform.position = targetPosition;
                path.RemoveAt(0);
            }
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
