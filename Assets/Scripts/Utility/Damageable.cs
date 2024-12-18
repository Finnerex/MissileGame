using System;
using UnityEngine;

namespace Utility
{
    public class Damageable : MonoBehaviour
    {

        [SerializeField] private bool willExplode;
        [SerializeField] private bool producesSmoke;
        
        
        public float maxHealth = 100;
        public float Health { get; private set; }

        private void Start()
        {
            Health = maxHealth;
        }


        public void Damage(float amount)
        {
            Health -= amount;

            if (producesSmoke)
                Destroy(Instantiate(EffectController.Instance.smokeEffect, transform.position, Quaternion.identity), amount * amount);
            
            if (Health > 0) return;
            
            if (willExplode)
                Instantiate(EffectController.Instance.explosionEffect, transform.position, Quaternion.identity);
                    
            Health = 0;
            Destroy(gameObject);
        }
        
    }
}