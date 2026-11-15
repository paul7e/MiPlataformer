using UnityEngine;
using platform.Core;

namespace platform.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground;
        [SerializeField] Health targetHealth;

        void Start()
        {
            UpdateHealthBar();
        }

        void OnEnable()
        {
            if (targetHealth != null)
            {
                targetHealth.onDamageTaken.AddListener(UpdateHealthBar);
            }
        }

        void OnDisable()
        {
            if (targetHealth != null)
            {
                targetHealth.onDamageTaken.RemoveListener(UpdateHealthBar);
            }
        }

        void UpdateHealthBar()
        {
            foreground.localScale = new Vector3(targetHealth.GetHealthPercentage(), 1f, 1f);
        }
    }
}