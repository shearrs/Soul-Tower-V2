using System;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public interface IConsoleCommand
    {
        public string Command { get; }
        public string Description { get; }

        public void TryExecuteCommand(string input, Action<string> consoleMessage, Action<string> consoleError);
    }
}
