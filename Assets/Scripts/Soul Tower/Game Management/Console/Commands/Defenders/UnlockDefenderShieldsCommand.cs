using Shears;
using SoulTower.Enemies;
using System;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public readonly struct UnlockDefenderShieldsCommand : IConsoleCommand
    {
        public string Command => "unlock_shields";

        public string Description => "Allows defenders to move their shields";

        public void TryExecuteCommand(string input, Action<string> consoleMessage, Action<string> consoleError)
        {
            int cmdLength = Command.Length;

            if (input.Length > cmdLength)
            {
                consoleError($"Could not parse command '{input}'. Use 'help' to see a list of commands.");
                return;
            }

            DefenderShield.Locked = false;
        }
    }
}
