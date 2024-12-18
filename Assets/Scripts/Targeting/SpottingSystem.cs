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
        
        public readonly HashSet<GameObject> SpottedObjects = new(); // best option idk

        private static readonly Collider[] FoundObjects = new Collider[30];

        private float _sqrSpottingRadius;
        private float _sqrKeepSpotRadius;

        private void Start()
        {
            _sqrSpottingRadius = spottingRadius * spottingRadius;
            _sqrKeepSpotRadius = keepSpotRadius * keepSpotRadius;
        }

        public void TrySpot()
        {

            SpottedObjects.RemoveWhere(item =>
                item == null || 
                (item.transform.position - transform.position).sqrMagnitude > _sqrKeepSpotRadius);

            int numSpotted = Physics.OverlapSphereNonAlloc(transform.position, spottingRadius, 
                FoundObjects, LayerMask.GetMask("Spottable"));

            for (int i = 0; i < numSpotted; i++)
            {
                Vector3 displacement = FoundObjects[i].transform.position - transform.position;
                
                if ((Vector3.Angle(transform.forward, displacement) > betterSpottingAngle
                     && displacement.sqrMagnitude < _sqrSpottingRadius * 0.5f)
                    || Vector3.Angle(transform.forward, displacement) <= betterSpottingAngle)
                    SpottedObjects.Add(FoundObjects[i].gameObject);
            }
            
        }
    }
}