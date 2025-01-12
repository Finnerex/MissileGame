using UnityEngine;

namespace Utility
{
    public class BillboardObject : MonoBehaviour
    {
        private Transform _transform;
        private Camera _mainCamera;
        
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