using UnityEngine;
using platform.Core;

namespace platform.Combat
{
    public class LemonProjectile : MonoBehaviour
    {
        [SerializeField] float speedLemon = 15f;
        [SerializeField] float damageLemon = 10f;
        [SerializeField] string targetTag = "Enemy";
        [SerializeField] string wallLayerName = "Ground";
        
        Rigidbody2D rb;
    
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Fire(float directionX)
        {
            rb.linearVelocity = new Vector2(directionX * speedLemon, 0f);
        }

        void OnBecameInvisible()
        {
            gameObject.SetActive(false); 
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(targetTag))
            {
                if (other.TryGetComponent(out Health health))
                {
                    health.TakeDamage(damageLemon);
                    gameObject.SetActive(false);
                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer(wallLayerName))
            {
                gameObject.SetActive(false);
            }
        }
    }
}