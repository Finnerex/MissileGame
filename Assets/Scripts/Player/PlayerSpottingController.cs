using Targeting;
using UnityEngine;

namespace Player
{
    public class PlayerSpottingController : MonoBehaviour
    {
        [SerializeField] private SpottingSystem spottingSystem;
        [SerializeField] private float spottingPeriodSeconds = 10;
        
        [SerializeField] private Texture2D icon;
        [SerializeField] private GUIStyle style;

        // private List<ObjectMarkerIcon> _uiIcons = new();

        private Transform _transform;
        private float _currentSpotTime;
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            _transform = transform;
        }

        private void OnGUI()
        {
            if (spottingSystem.SpottedObjects.Count == 0)
                return;

            foreach (Transform t in spottingSystem.SpottedObjects)
            {
                if (t == null) continue;
                Vector3 screenPosition = _cam.WorldToScreenPoint(t.position);

                if (screenPosition.z <= 0) continue;
                
                Vector2 guiPosition = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
                
                GUI.DrawTexture(new Rect(guiPosition.x - 4, guiPosition.y - 12, 8, 8), icon);

                float dist = (t.position - _transform.position).magnitude;
                GUI.Label(new Rect(guiPosition.x - 50, guiPosition.y + 2, 100, 20), dist > 1000 ? $"{dist / 1000:F2} km" : $"{dist:F0} m", style);
                
            }
            
        }

        private void Update()
        {
            if (_currentSpotTime >= spottingPeriodSeconds)
            {
                spottingSystem.TrySpot();
                _currentSpotTime = 0;
            }

            _currentSpotTime += Time.deltaTime;

        }
    }
}