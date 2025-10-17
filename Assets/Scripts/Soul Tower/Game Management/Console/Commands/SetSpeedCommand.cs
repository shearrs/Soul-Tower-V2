using System;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public readonly struct SetSpeedCommand : IConsoleCommand
    {
        public readonly string Command => "set_speed";
        public readonly string Description => "Sets game timescale.";

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
                consoleError($"Could not parse command '{input}'. Did you mean {Command}?");
                return;
            }

            try
            {
                int offset = cmdLength + 1;
                float speed = float.Parse(input[offset..]);

                if (speed > 100.0f)
                    consoleError($"{Command} can only be used with values less than or equal to 100.0.");
                else
                    Time.timeScale = speed;
            }
            catch (ArgumentOutOfRangeException)
            {
                consoleError($"{Command} requires a number value to be set.");
            }
            catch (FormatException)
            {
                consoleError($"Could not parse float value from {input[8..]}.");
            }
        }
    }
}
