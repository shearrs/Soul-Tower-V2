using Shears;
using Shears.Logging;
using Shears.Signals;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public class GameManager : ProtectedSingleton<GameManager>
    {
        private void OnEnable()
        {
            SignalShuttle.Register<CatalystHealthChangedSignal>(OnCatalystHealthChanged);
        }

        private void OnDisable()
        {
            SignalShuttle.Deregister<CatalystHealthChangedSignal>(OnCatalystHealthChanged);
        }

        private void OnCatalystHealthChanged(CatalystHealthChangedSignal signal)
        {
            if (signal.Health > 0)
                return;

            SHLogger.Log("Game lost!", color: Color.darkRed);
        }
    }
}
