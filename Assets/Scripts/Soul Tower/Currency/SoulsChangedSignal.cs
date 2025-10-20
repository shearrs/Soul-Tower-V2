using Shears.Signals;
using UnityEngine;

namespace SoulTower.Currency
{
    public readonly struct SoulsChangedSignal : ISignal
    {
        private readonly int souls;

        public readonly int Souls => souls;

        public SoulsChangedSignal(int souls)
        {
            this.souls = souls;
        }
    }
}
