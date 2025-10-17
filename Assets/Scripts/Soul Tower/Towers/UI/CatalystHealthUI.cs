using Shears;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulTower.Towers.UI
{
    public class CatalystHealthUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Catalyst catalyst;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image healthFillImage;

        [Header("Settings")]
        [SerializeField] private Range<float> healthFillRange;

        private bool isEnabled = false;

        private void Start()
        {
            Enable();
        }

        private void OnDisable()
        {
            catalyst.HealthChanged -= UpdateHealthSlider;
        }

        public void Enable()
        {
            if (isEnabled)
                return;

            catalyst.HealthChanged += UpdateHealthSlider;
            UpdateHealthSlider(catalyst.Health);
            canvas.enabled = true;

            isEnabled = true;
        }

        public void Disable()
        {
            if (!isEnabled)
                return;

            catalyst.HealthChanged -= UpdateHealthSlider;
            canvas.enabled = false;

            isEnabled = false;
        }

        private void UpdateHealthSlider(int health)
        {
            healthText.text = health.ToString();

            Vector2 position = healthFillImage.rectTransform.anchoredPosition;
            position.y = healthFillRange.Lerp((float)health / catalyst.MaxHealth);

            healthFillImage.rectTransform.anchoredPosition = position;
        }
    }
}
