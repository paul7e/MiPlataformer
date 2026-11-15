using UnityEngine;
using platform.Core;

namespace platform.Controller
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 3f;
        [SerializeField] float chaseRange = 8f;

        Transform player;
        Rigidbody2D rb;
        Health health;

        void Awake()
        {
            player = GameObject.FindWithTag("Player").transform;
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (health.IsDead())
            {
                DieBehavior();
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= chaseRange)
            {
                ChasePlayer();
            }
            else
            {
                Idle();
            }
        }

        void ChasePlayer()
        {
            float directionX = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);
            UpdateFacing(directionX);
        }

        void Idle()
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        void UpdateFacing(float directionX)
        {
            transform.localScale = new Vector3(Mathf.Sign(directionX), 1f, 1f);
        }

        void DieBehavior()
        {
            rb.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.deepSkyBlue;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }
}