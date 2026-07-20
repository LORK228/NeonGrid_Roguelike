using UnityEngine;
using UnityEngine.UI;
using TMPro; // Используем современный TextMeshPro

namespace UI
{
    public class HUDView : MonoBehaviour
    {
        [SerializeField] private Slider _playerHealthSlider;
        [SerializeField] private TextMeshProUGUI _notificationText;

        public void UpdateHealthBar(float current, float max)
        {
            if (_playerHealthSlider != null)
            {
                _playerHealthSlider.maxValue = max;
                _playerHealthSlider.value = current;
            }
        }

        public void SetNotificationText(string text)
        {
            if (_notificationText != null)
            {
                _notificationText.text = text;
            }
        }
    }
}