using Shears.Signals;
using SoulTower.Enemies;
using System;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public readonly struct SetDefenderShieldsCommand : IConsoleCommand
    {
        public string Command => "set_defender_shields";

        public string Description => "Sets all defender's shields to a direction.";

        public void TryExecuteCommand(string input, Action<string> consoleMessage, Action<string> consoleError)
        {
            int cmdLength = Command.Length;

            if (input.Length == cmdLength)
            {
                consoleError($"{Command} requires a direction value to be set.");
                return;
            }
            else if (input[cmdLength] != ' ')
            {
                consoleError($"Could not parse command '{input}'.");
                return;
            }

            try
            {
                int offset = cmdLength + 1;
                DefenderShield.Direction direction = (DefenderShield.Direction)Enum.Parse(typeof(DefenderShield.Direction), input[offset..], true);

                SignalShuttle.Emit(new SetDefenderShieldsSignal(direction));
            }
            catch (ArgumentException)
            {
                int offset = cmdLength + 1;

                consoleError($"Could not parse direction value from '{input[offset..]}'. Valid directions are: {{ right, up, left }}");
            }
        }
    }
}
