using UnityEngine;

namespace Targeting
{
    public class CollisionDamageable : MonoBehaviour
    {
        // should be attached to the rigidbody parent of any object that should be damaged in a collision
        [SerializeField] private float collisionDamageScale = 0.1f; // perchance should be per damageable
        private void OnCollisionEnter(Collision other)
        {
            Collider hitCollider = other.GetContact(0).thisCollider;

            if (hitCollider.TryGetComponent(out Damageable d))
                d.Damage(collisionDamageScale * other.relativeVelocity.magnitude);

        }
    }
}