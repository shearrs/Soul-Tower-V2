using Shears.Signals;
using UnityEngine;

namespace SoulTower.Towers
{
    public readonly struct SPCountChangedSignal : ISignal
    {
        private readonly int sp;

        public readonly int SP => sp;

        public SPCountChangedSignal(int sp)
        {
            this.sp = sp;
        }
    }
}
