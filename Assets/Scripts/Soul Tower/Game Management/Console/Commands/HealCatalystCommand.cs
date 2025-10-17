using SoulTower.Towers;
using System;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public readonly struct HealCatalystCommand : IConsoleCommand
    {
        public readonly string Command => "heal_catalyst";
        public readonly string Description => "Heals the catalyst by a passed integer amount.";

        public void TryExecuteCommand(string input, Action<string> consoleMessage, Action<string> consoleError)
        {
            int cmdLength = Command.Length;

            if (input.Length > cmdLength)
            {
                consoleError($"Could not parse ${input}, did you mean '{Command}'?");
                return;
            }

            if (GameConsole.Catalyst == null)
            {
                consoleError("Could not find Catalyst in scene");
                return;
            }
            else if (GameConsole.Catalyst.Health >= GameConsole.Catalyst.MaxHealth)
            {
                consoleError("Catalyst is already at max health");
                return;
            }

            GameConsole.Catalyst.Heal();
        }
    }
}
