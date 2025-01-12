using UnityEngine;

namespace Utility
{

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private GameObject center;
    [SerializeField] private float scrollSensitivity = 8;
    [SerializeField] private float minDistance = 10;
    [SerializeField] private float maxDistance = 50;
    [SerializeField] private float zoomFOV = 30;
    [SerializeField] private float zoomTimeSeconds = 0.5f;
    
    public float sensitivity = 2;

    public bool requireRightClickDrag;
    
    private Transform _transform;
    private Vector3 _offset;
    private Transform _centerTransform;

    private Camera _camera;
    private float _defaultFOV;
    private float _currentZoomTimeSeconds;

    private void Start()
    {
        _transform = transform;

        Cursor.visible = requireRightClickDrag;
        Cursor.lockState = requireRightClickDrag ? CursorLockMode.Confined : CursorLockMode.Locked;

        _offset = _transform.position - center.transform.position;
        
        _centerTransform = center.transform;

        _camera = GetComponent<Camera>(); // omg slow get component i could never
        _defaultFOV = _camera.fieldOfView;
    }

    public void SwitchTo(Transform t)
    {
        center = t.gameObject;
        _offset = _transform.position - t.position;
        _centerTransform = t;
    }

    private void LateUpdate()
    {
        
        _offset = _offset.normalized * Mathf.Clamp(_offset.magnitude - Input.mouseScrollDelta.y * scrollSensitivity, minDistance, maxDistance);
        _transform.position = _centerTransform.position + _offset;
        _transform.rotation = Quaternion.LookRotation(_centerTransform.position - _transform.position);
        
        if (requireRightClickDrag && !Input.GetMouseButton(1)) return;
        
        Quaternion rotationX = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * sensitivity, _transform.right);
        Quaternion rotationY = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity, _centerTransform.up);

        // yeah there was some strange bits with the camera not quite following the object
        _offset = rotationY * rotationX * _offset;
        
        
        if (requireRightClickDrag) return;
        
        // zoom (stolen from FOLC!!)
        if (Input.GetMouseButton(1) && _currentZoomTimeSeconds < zoomTimeSeconds)
        {
            _currentZoomTimeSeconds += Time.deltaTime;
            _camera.fieldOfView = Mathf.Lerp(_defaultFOV, zoomFOV, _currentZoomTimeSeconds / zoomTimeSeconds);
        }
        else if (!Input.GetMouseButton(1) && _currentZoomTimeSeconds > 0)
        {
            _currentZoomTimeSeconds -= Time.deltaTime;
            // me when i repeat myself
            _camera.fieldOfView = Mathf.Lerp(_defaultFOV, zoomFOV, _currentZoomTimeSeconds / zoomTimeSeconds);
        }
        
    }
}
}