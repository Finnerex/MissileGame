using UnityEngine;

namespace Player
{
    public class ForwardUiIcon : MonoBehaviour
    {
        public RectTransform icon;
        [SerializeField] private Camera cam;
        public float distance = 500;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }

        private void Awake()
        {
            if (cam == null)
                cam = Camera.main;/*GameObject.FindWithTag("MainCamera").GetComponent<Camera>();*/
        }


        private void Update()
        {
            Vector3 screenPoint = cam.WorldToScreenPoint(_transform.position + _transform.forward * distance);

            if (screenPoint.z >= 0) 
                icon.position = screenPoint;

        }

    }
}