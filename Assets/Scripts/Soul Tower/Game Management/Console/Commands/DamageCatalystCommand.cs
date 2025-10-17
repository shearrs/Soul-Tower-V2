using SoulTower.Towers;
using System;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public readonly struct DamageCatalystCommand : IConsoleCommand
    {
        public readonly string Command => "damage_catalyst";
        public readonly string Description => "Damages the catalyst by a passed integer amount.";

        public void TryExecuteCommand(string input, Action<string> consoleMessage, Action<string> consoleError)
        {
            int cmdLength = Command.Length;

            if (input.Length > cmdLength)
            {
                consoleError($"Could not parse ${input}, did you mean '{Command}'?");
                return;
            }

            if (GameConsole.Catalyst)
            {
                consoleError("Could not find Catalyst in scene");
                return;
            }
            else if (GameConsole.Catalyst.Health == 0)
            {
                consoleError("Catalyst health is already 0");
                return;
            }

            GameConsole.Catalyst.Damage();
        }
    }
}
