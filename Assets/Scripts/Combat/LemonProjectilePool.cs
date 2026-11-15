using System.Collections.Generic;
using UnityEngine;

namespace platform.Combat
{
    public class LemonProjectilePool : MonoBehaviour
    {
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] int initialPoolSize = 10;
        
        List<GameObject> pool = new List<GameObject>();

        void Start()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                AddProjectileToPool();
            }
        }

        GameObject AddProjectileToPool()
        {
            //Sin transform, la posicion de los limonos no dependen del player
            GameObject obj = Instantiate(projectilePrefab);

            obj.SetActive(false);
            pool.Add(obj);
            return obj;
        }

        public GameObject RequestProjectile()
        {
            // Buscar el primero que esté apagado
            foreach (GameObject obj in pool)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // Si todos están ocupados, se crea uno nuevo 
            GameObject newObj = AddProjectileToPool();
            newObj.SetActive(true);
            return newObj;
        }
    }
}