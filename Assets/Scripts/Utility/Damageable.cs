using System;
using UnityEngine;

namespace Utility
{
    public class Damageable : MonoBehaviour
    {

        [SerializeField] private bool willExplode;
        
        public float maxHealth = 100;
        public float Health { get; private set; }

        private void Start()
        {
            Health = maxHealth;
        }
        
        public void Damage(float amount)
        {
            Health -= amount;

            if (!(Health <= 0)) return;
            
            if (willExplode)
                Instantiate(EffectController.Instance.explosionEffect, transform.position, Quaternion.identity);
                    
            Health = 0;
            Destroy(gameObject);
        }

    }
}