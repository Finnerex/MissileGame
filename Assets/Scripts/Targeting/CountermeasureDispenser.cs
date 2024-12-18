using System;
using System.Collections;
using UnityEngine;

namespace Targeting
{
    public class CountermeasureDispenser : MonoBehaviour
    {
        [SerializeField] private Rigidbody parentRigidbody;
        
        [SerializeField] private Flare flarePrefab;
        [SerializeField] private float flareDeployForce = 10;
        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }
        
        public void DeployFlare()
        {
            Flare createdFlare = Instantiate(flarePrefab, _transform.position, Quaternion.identity);
            createdFlare.flareRigidbody.velocity = parentRigidbody.velocity;
            createdFlare.flareRigidbody.AddForce(_transform.forward * flareDeployForce);
            Destroy(createdFlare.gameObject, 5);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}