using System;
using UnityEngine;

namespace Targeting
{
    public class Flare : MonoBehaviour
    {

        private Transform _transform;
        private Camera _mainCamera;
        public Rigidbody flareRigidbody;
        
        private void Start()
        {
            _transform = transform;
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            _transform.rotation = _mainCamera.transform.rotation;
        }
    }
}