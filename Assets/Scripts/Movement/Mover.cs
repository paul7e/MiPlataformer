using UnityEngine;

namespace platform.Movement
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] float maxSpeed = 8f;
        [SerializeField] float jumpForce = 12f;

        [Header("Ground Check")]
        [SerializeField] Vector2 groundBoxSize = new Vector2(0.8f, 0.2f);
        [SerializeField] float groundCastDistance = 0.5f;
        [SerializeField] LayerMask groundLayer;

        [Header("Advanced Jump Parameters")]
        [SerializeField] float fallGravityScale = 6f; // Gravedad al caer para dar sensación de peso
        [SerializeField] float lowJumpGravityScale = 5f;

        [Header("Wall Check")]
        [SerializeField] Vector2 wallBoxSize = new Vector2(0.2f, 0.8f);
        [SerializeField] float wallCastDistance = 0.5f;

        [Header("Wall Slide & Jump")]
        [SerializeField] float wallSlideSpeed = 2f; // Velocidad de caída al raspar el muro
        [SerializeField] Vector2 wallJumpForce = new Vector2(10f, 12f); // Fuerza X y Y al rebotar
        [SerializeField] float wallJumpInputBlockTime = 0.15f; // Tiempo que bloqueamos el input para que se note el rebote

        [Header("Dash")]
        [SerializeField] float dashForce = 15f;
        [SerializeField] float dashDuration = 0.25f;

        [Header("Jump Parameters")]
        [SerializeField] float jumpCutMultiplier = 0.5f;
        float currentDashTime = 0f; // Nuestra única variable de estado
        float currentWallJumpTime = 0f;
        float defaultGravity;
        bool isJumpHeld;
        Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            defaultGravity = rb.gravityScale;
        }

        void Update()
        {
            ApplyJumpPhysics();
        }

        public void MoveTo(float directionX, float speedFraction)
        {
            // 1. Actualizamos hacia dónde mira el personaje antes de cualquier validación
            UpdateFacingDirection(directionX);
            
            if (currentDashTime > 0 || currentWallJumpTime > 0) return;

            // Si el temporizador del dash está activo, ignoramos el movimiento normal

            float totSpeed = maxSpeed * Mathf.Clamp01(speedFraction);
            rb.linearVelocity = new Vector2(directionX * totSpeed, rb.linearVelocity.y);
        }

        private void UpdateFacingDirection(float directionX)
        {
            // Voltea el transform usando 1 o -1 en el eje X.
            // Si directionX es 0, no entra a las condiciones y mantiene la dirección actual.
            if (directionX > 0f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (directionX < 0f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        public void CancelJump()
        {
            // Cero estados booleanos. Evaluamos la física directamente.
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
        }

        public void Dash(float directionX)
        {
            // Evita que el jugador haga spam del dash si ya está dasheando
            if (currentDashTime > 0) return; 
            
            StartCoroutine(DashRoutine(directionX));
        }

        private System.Collections.IEnumerator DashRoutine(float directionX)
        {
            currentDashTime = dashDuration;
            
            // Guardamos la escala de gravedad y la ponemos en 0 (típico de Megaman X)
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;

            while (currentDashTime > 0)
            {
                // Sobrescribimos la velocidad directamente en X, y mantenemos Y en 0
                rb.linearVelocity = new Vector2(directionX * dashForce, 0f);
                currentDashTime -= Time.deltaTime;
                
                // Esperamos al siguiente frame de físicas
                yield return new WaitForFixedUpdate(); 
            }

            // Restauramos todo
            rb.gravityScale = defaultGravity;
            currentDashTime = 0f;
        }

        public void Jump()
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        public void SetJumpHeld(bool isHeld)
        {
            isJumpHeld = isHeld;
        }

        private void ApplyJumpPhysics()
        {
            // NUEVO: Lógica de Wall Slide
            if (IsTouchingWall() && rb.linearVelocity.y <= 0)
            {
                // Anulamos la gravedad y forzamos una velocidad de caída constante
                rb.gravityScale = 0f; 
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            }
            // Lógica que ya tenías:
            else if (rb.linearVelocity.y < 0)
            {
                rb.gravityScale = fallGravityScale;
            }
            else if (rb.linearVelocity.y > 0 && !isJumpHeld)
            {
                rb.gravityScale = lowJumpGravityScale;
            }
            else
            {
                rb.gravityScale = defaultGravity;
            }
        }

        public void WallJump()
        {
            // Determinamos hacia qué lado saltar (el contrario al que estamos mirando)
            float jumpDirection = -Mathf.Sign(transform.localScale.x);
            
            // Reseteamos la velocidad Y para que el salto siempre tenga la misma altura
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            
            // Aplicamos la fuerza del salto diagonal
            rb.linearVelocity = new Vector2(jumpDirection * wallJumpForce.x, wallJumpForce.y);

            // Volteamos al personaje visualmente para que mire hacia donde salta
            UpdateFacingDirection(jumpDirection);

            // Iniciamos la corrutina que bloquea el input brevemente
            StartCoroutine(WallJumpRoutine());
        }

        private System.Collections.IEnumerator WallJumpRoutine()
        {
            currentWallJumpTime = wallJumpInputBlockTime;
            
            while (currentWallJumpTime > 0)
            {
                currentWallJumpTime -= Time.deltaTime;
                yield return null; // Esperamos al siguiente frame
            }
        }


        public bool IsTouchingWall()
        {
            // Lanzamos el BoxCast hacia la dirección donde mira el personaje (localScale.x)
            Vector2 castDirection = new Vector2(transform.localScale.x, 0f);
            
            return Physics2D.BoxCast(
                transform.position,
                wallBoxSize,
                0f, // Ángulo
                castDirection,
                wallCastDistance,
                groundLayer
            );
        }

        

        public bool IsGrounded()
        {
            // Parámetros: (origen, tamaño de la caja, ángulo, dirección, distancia, capa)
            return Physics2D.BoxCast(
                transform.position,
                groundBoxSize,
                0f,
                Vector2.down,
                groundCastDistance,
                groundLayer
            );
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.darkOliveGreen;
            
            // Gizmo del piso 
            Vector3 groundBoxPosition = transform.position + Vector3.down * groundCastDistance;
            Gizmos.DrawWireCube(groundBoxPosition, groundBoxSize);

            // Gizmo del muro
            Vector3 wallBoxPosition = transform.position + (Vector3.right * transform.localScale.x * wallCastDistance);
            Gizmos.DrawWireCube(wallBoxPosition, wallBoxSize);
        }
    }
}