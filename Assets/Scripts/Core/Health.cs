using UnityEngine;
using UnityEngine.Events;

namespace platform.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float invulnerabilityDuration = 1.5f;

        float currentHealth;
        float lastDamageTime;

        public UnityEvent onDamageTaken;
        public UnityEvent onDie;

        void Awake()
        {
            currentHealth = maxHealth;
            lastDamageTime = -invulnerabilityDuration;
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;
            if (Time.time < lastDamageTime + invulnerabilityDuration) return;

            currentHealth = Mathf.Max(currentHealth - damage, 0f);
            lastDamageTime = Time.time;

            onDamageTaken?.Invoke();
            print($"{name} - Current Health = {currentHealth}");
            
            if (currentHealth == 0)
            {
                onDie?.Invoke();
            }
        }

        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }

        public bool IsDead()
        {
            return currentHealth <= 0;
        }
    }
}