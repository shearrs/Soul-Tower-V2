using Shears;
using Shears.Tweens;
using System.Collections;
using Shears.Signals;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SoulTower.Towers;

namespace SoulTower.Enemies
{    
    public class SPManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI SPtext;
        public int soulPointTotal;

        void OnEnable()
        {
            SignalShuttle.Register<SoulCollectedSignal>(OnSoulCollected);
        }

        void OnDisable()
        {
            SignalShuttle.Deregister<SoulCollectedSignal>(OnSoulCollected);
        }

        private void Start()
        {
            UpdateSPCount(0);
        }

        private void OnSoulCollected(SoulCollectedSignal signal)
        {
            UpdateSPCount(signal.DroppedSoul.GetComponent<DroppedSoul>().Value);
        }

        public void UpdateSPCount(int SPtoAdd)
        {
            soulPointTotal += SPtoAdd;
            if(soulPointTotal < 0)
            {
                soulPointTotal = 0;
            }
            SPtext.text = $"SP: {soulPointTotal}";
            SignalShuttle.Emit(new SPCountChangedSignal(soulPointTotal));
        }
    }
}
