using System;
using UnityEngine;
using Utility;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "Warhead", menuName = "MissileComponent/Warhead")]
    public class WarheadComponent : MissileComponent
    {
        public float explosionRadius;
        public bool proximityFuse;
        public float proximityRadius;

        public bool CheckExplode(Vector3 position)
        {
            bool shouldExplode = proximityFuse && Physics.CheckSphere(position, proximityRadius, LayerMask.GetMask("Target"));
            
            if (shouldExplode)
                Explode(position);

            return shouldExplode;
        }

        public void Explode(Vector3 position)
        {
            Instantiate(EffectController.Instance.explosionEffect, position, Quaternion.identity);
            Collider[] exploded = new Collider[20];

            int numberBlown = Physics.OverlapSphereNonAlloc(position, explosionRadius, exploded);

            for (int i = 0; i < numberBlown; i++)
            {
                if (!exploded[i].TryGetComponent(out Rigidbody rb)) continue;
                
                Vector3 displacement = rb.transform.position - position;
                rb.AddForceAtPosition(displacement.normalized / (0.0001f * (0.5f + displacement.magnitude)), position, ForceMode.Impulse);
                Debug.Log($"added a force to object {rb.name} {displacement.normalized / (0.0001f * (0.5f + displacement.magnitude))}");
            }

        }
    }
}