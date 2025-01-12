using System;
using Targeting;
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

        [NonSerialized] private static readonly Collider[] Exploded = new Collider[20];

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

            int numberBlown = Physics.OverlapSphereNonAlloc(position, explosionRadius, Exploded);

            for (int i = 0; i < numberBlown; i++)
            {
                Vector3 displacement = Exploded[i].transform.position - position;

                if (Exploded[i].TryGetComponent(out Damageable d))
                {
                    Debug.Log("bro was damageable and was hit with " + (500 + explosionRadius) / (displacement.magnitude + 1) + " damage");
                    d.Damage((500 + explosionRadius) / (displacement.magnitude + 1));
                }

                if (Exploded[i].TryGetComponent(out Rigidbody rb))
                    rb.AddExplosionForce(5000, position, explosionRadius);
                    // rb.AddForceAtPosition(displacement.normalized / (0.0001f * displacement.magnitude + 1), position, ForceMode.Impulse);
                    
            }

        }

        public override string ToString()
        {
            string fuseYesNo = proximityFuse ? "yes" : "no";
            
            return $"{base.ToString()}\nExplosionSize: {explosionRadius}\nProximity Fuse: {fuseYesNo}";
        }
    }
}