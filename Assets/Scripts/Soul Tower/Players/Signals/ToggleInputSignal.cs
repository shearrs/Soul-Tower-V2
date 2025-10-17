using Shears.Signals;
using UnityEngine;

namespace SoulTower.Players
{
    public readonly struct ToggleInputSignal : ISignal
    {
        private readonly bool enableInput;

        public readonly bool EnableInput => enableInput;

        public ToggleInputSignal(bool enableInput)
        {
            this.enableInput = enableInput;
        }
    }
}
