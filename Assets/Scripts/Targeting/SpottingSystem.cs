using System;
using System.Collections.Generic;
using UnityEngine;

namespace Targeting
{
    public class SpottingSystem : MonoBehaviour
    {

        [SerializeField] private float spottingRadius = 1000;
        [SerializeField] private float betterSpottingAngle = 60; // half angle 
        [SerializeField] private float keepSpotRadius = 1500;

        [SerializeField] private LayerMask layerMask;
        
        public readonly HashSet<Transform> SpottedObjects = new(); // best option idk

        private static readonly Collider[] FoundObjects = new Collider[30];

        private float _sqrSpottingRadius;
        private float _sqrKeepSpotRadius;

        private Transform _transform;

        private void Start()
        {
            _sqrSpottingRadius = spottingRadius * spottingRadius;
            _sqrKeepSpotRadius = keepSpotRadius * keepSpotRadius;
            _transform = transform;
        }

        public void TrySpot()
        {

            SpottedObjects.RemoveWhere(item =>
                item == null || 
                (item.transform.position - _transform.position).sqrMagnitude > _sqrKeepSpotRadius);

            int numSpotted = Physics.OverlapSphereNonAlloc(_transform.position, spottingRadius, 
                FoundObjects, layerMask);

            for (int i = 0; i < numSpotted; i++)
            {
                Vector3 displacement = FoundObjects[i].transform.position - _transform.position;
                
                if ((Vector3.Angle(_transform.forward, displacement) > betterSpottingAngle
                     && displacement.sqrMagnitude < _sqrSpottingRadius * 0.5f)
                    || Vector3.Angle(_transform.forward, displacement) <= betterSpottingAngle)
                    SpottedObjects.Add(FoundObjects[i].transform);
            }
            
        }

        public Transform GetClosest()
        {
            float closestSqrDist = float.MaxValue;
            Transform closest = null;
            
            foreach (Transform t in SpottedObjects)
            {
                float sqrDist = (t.position - _transform.position).sqrMagnitude;
                
                if (sqrDist >= closestSqrDist) continue;

                closest = t;
                closestSqrDist = sqrDist;
            }

            return closest;
        }


        private void Update()
        {
            SpottedObjects.RemoveWhere(item => item == null); // me no like expensive
        }
    }
}