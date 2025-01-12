using System.Collections.Generic;
using Targeting;
using UnityEngine;
using Utility;

namespace Player
{
    public class PlayerSpottingController : MonoBehaviour
    {
        [SerializeField] private SpottingSystem spottingSystem;
        [SerializeField] private float spottingPeriodSeconds = 10;

        [SerializeField] private ObjectMarkerIcon enemyIcon;
        
        private List<ObjectMarkerIcon> _uiIcons = new();

        private Transform _transform;
        private float _currentSpotTime;
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            _transform = transform;
        }

        private void Update()
        {
            if (_currentSpotTime >= spottingPeriodSeconds)
            {
                spottingSystem.TrySpot();
                _currentSpotTime = 0;
            }

            _currentSpotTime += Time.deltaTime;
            
            // i dont really know if i want to do this every frame
            int difference = spottingSystem.SpottedObjects.Count - _uiIcons.Count;
                
            // Debug.Log($"Difference: {difference}, system count: {spottingSystem.SpottedObjects.Count}, ui count: {_uiIcons.Count}");
                
            for (int i = 0; i < Mathf.Abs(difference); i++)
            {
                if (difference < 0)
                {
                    Destroy(_uiIcons[i].gameObject);
                    _uiIcons.RemoveAt(i);
                }
                else
                    _uiIcons.Add(Instantiate(enemyIcon, UIController.Instance.transform));
            }
            
            int j = 0;
            foreach (GameObject spotted in spottingSystem.SpottedObjects)
            {
                if (spotted == null)
                {
                    j++;
                    continue;
                }
                
                Vector3 pos = _cam.WorldToScreenPoint(spotted.transform.position);
                if (pos.z >= 0)
                {
                    pos.z = 20;
                    pos.y += 6;

                    _uiIcons[j].rectTransform.position = pos;
                    _uiIcons[j].Distance = (_transform.position - spotted.transform.position).magnitude;
                }
                j++;
            }
            
        }
    }
}