using Shears.Signals;
using TMPro;
using UnityEngine;

namespace SoulTower.Currency.UI
{
    public class SoulsManagerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;

        private void OnEnable()
        {
            SignalShuttle.Register<SoulsChangedSignal>(OnSoulsChanged);
        }

        private void OnDisable()
        {
            SignalShuttle.Deregister<SoulsChangedSignal>(OnSoulsChanged);
        }

        private void Start()
        {
            textMesh.text = SoulsManager.SoulPoints.ToString();
        }

        private void OnSoulsChanged(SoulsChangedSignal signal)
        {
            textMesh.text = signal.Souls.ToString();
        }
    }
}
