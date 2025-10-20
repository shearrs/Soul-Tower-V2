using Shears;
using Shears.Signals;
using UnityEngine;

namespace SoulTower.Currency
{    
    public class SoulsManager : ProtectedSingleton<SoulsManager>
    {
        [SerializeField] private int soulPoints;

        public static int SoulPoints => Instance.soulPoints;

        void OnEnable()
        {
            SignalShuttle.Register<SoulCollectedSignal>(OnSoulCollected);
        }

        void OnDisable()
        {
            SignalShuttle.Deregister<SoulCollectedSignal>(OnSoulCollected);
        }

        private void OnSoulCollected(SoulCollectedSignal signal)
        {
            InstUpdateSouls(signal.DroppedSoul.Value);
        }

        public static void UpdateSouls(int changeInSouls) => Instance.InstUpdateSouls(changeInSouls);
        private void InstUpdateSouls(int changeInSouls)
        {
            soulPoints += changeInSouls;

            if(soulPoints < 0)
                soulPoints = 0;
            
            SignalShuttle.Emit(new SoulsChangedSignal(soulPoints));
        }
    }
}
