using UnityEngine;

namespace Utility
{

public class OrbitCamera : MonoBehaviour
{
    public GameObject center;
    [SerializeField] private float scrollSensitivity = 8;
    [SerializeField] private float minDistance = 10;
    [SerializeField] private float maxDistance = 50;

    public float sensitivity = 2;

    private Transform _transform;
    private Vector3 _offset;

    private void Start()
    {
        _transform = transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _offset = _transform.position - center.transform.position;
    }

    private void LateUpdate()
    {
        Transform centerTransform = center.transform;
        
        // Rotate the camera around the center object based on mouse input
        Quaternion rotationX = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * sensitivity, _transform.right);
        Quaternion rotationY = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity, centerTransform.up);

        // Apply rotations to the offset vector
        _offset = rotationY * rotationX * _offset;

        // Calculate the new distance after zooming in/out
        _offset = _offset.normalized * Mathf.Clamp(_offset.magnitude - Input.mouseScrollDelta.y * scrollSensitivity, minDistance, maxDistance);

        // Recalculate the camera position based on the center object's new position
        _transform.position = centerTransform.position + _offset;

        // Calculate the direction from the camera to the center object and set the rotation manually
        _transform.rotation = Quaternion.LookRotation(centerTransform.position - _transform.position);
    }
}
}