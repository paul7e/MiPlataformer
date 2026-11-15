using UnityEngine;
using platform.Core;

namespace platform.Combat
{
    public class ContactDamage : MonoBehaviour
    {
        [SerializeField] float damageAmount = 15f;
        [SerializeField] string targetTag = "Player";

        void OnCollisionEnter2D(Collision2D collision)
        {
            DealDamage(collision.gameObject);
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            DealDamage(collision.gameObject);
        }

        void DealDamage(GameObject target)
        {
            if (target.CompareTag(targetTag))
            {
                if (target.TryGetComponent(out Health targetHealth))
                {
                    targetHealth.TakeDamage(damageAmount);
                }
            }
        }
    }
}