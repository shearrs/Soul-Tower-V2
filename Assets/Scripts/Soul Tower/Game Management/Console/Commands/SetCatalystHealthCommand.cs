using SoulTower.Towers;
using System;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public readonly struct SetCatalystHealthCommand : IConsoleCommand
    {
        public readonly string Command => "set_catalyst_health";
        public readonly string Description => "Sets the catalyst health to a integer value.";

        public void TryExecuteCommand(string input, Action<string> consoleMessage, Action<string> consoleError)
        {
            int cmdLength = Command.Length;

            if (input.Length == cmdLength)
            {
                consoleError($"{Command} requires a number value to be set.");
                return;
            }
            else if (input[cmdLength] != ' ')
            {
                consoleError($"Could not parse command '{input}'.");
                return;
            }

            try
            {
                if (GameConsole.Catalyst == null)
                {
                    consoleError("Could not find Catalyst in scene.");
                    return;
                }

                int offset = cmdLength + 1;
                int health = int.Parse(input[offset..]);

                if (health > GameConsole.Catalyst.MaxHealth)
                    consoleError($"{Command} can only be used with values less than or equal to 100.0.");
                else
                    GameConsole.Catalyst.SetHealth(health);
            }
            catch (ArgumentOutOfRangeException)
            {
                consoleError($"{Command} requires a number value to be set.");
            }
            catch (FormatException)
            {
                consoleError($"Could not parse integer value from {input[8..]}");
            }
        }
    }
}
