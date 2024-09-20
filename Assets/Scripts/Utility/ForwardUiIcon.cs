using System;
using UnityEngine;

namespace Utility
{
    public class ForwardUiIcon : MonoBehaviour
    {
        public RectTransform icon;
        [SerializeField] private Camera cam;
        public float distance = 150;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }

        private void Awake()
        {
            if (cam == null)
                cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }


        private void Update()
        {
            Vector3 screenPoint = cam.WorldToScreenPoint(_transform.position + _transform.forward * distance);
            
            icon.position = screenPoint;
        }
    }
}