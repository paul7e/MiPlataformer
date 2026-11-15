using UnityEngine;

namespace platform.Combat
{

    [RequireComponent(typeof(LemonProjectilePool))]
    public class GunBuster : MonoBehaviour
    {
        [SerializeField] Transform firePoint; // El punto desde donde sale el limon
        LemonProjectilePool pool;

        void Awake()
        {
            pool = GetComponent<LemonProjectilePool>();
        }

        public void Shoot(float facingDirection)
        {
            // Pedir un limon al Pool
            GameObject lemon = pool.RequestProjectile();
            
            // Posicionar en el cañon
            lemon.transform.position = firePoint.position;
            
            // Pasar la direccion y diaparar
            if (lemon.TryGetComponent(out LemonProjectile projectile))
            {
                projectile.Fire(facingDirection);
            }
        }
    }
}