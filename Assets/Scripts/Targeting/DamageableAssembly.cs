using System;
using UnityEngine;

namespace Targeting
{
    public class DamageableAssembly : MonoBehaviour
    {
        [SerializeField] private Damageable[] vitalComponents;

        private void Start()
        {
            foreach (Damageable d in vitalComponents)
            {
                d.onDestroy += OnVitalComponentDestroyed;
            }
        }

        private void OnVitalComponentDestroyed()
        {
            Destroy(gameObject);
        }
        
    }
}