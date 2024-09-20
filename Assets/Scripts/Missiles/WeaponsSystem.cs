using System;
using System.Collections.Generic;
using Missiles.Components;
using UnityEngine;
using Utility;

namespace Missiles
{
    public class WeaponsSystem : MonoBehaviour
    {
        [SerializeField] private Missile defaultMissilePrefab;
        [SerializeField] private Missile[] serializedMissiles;
        
        public readonly Queue<Missile> Missiles = new();

        private Rigidbody _rigidbody;

        private void Start()
        {
            foreach (var m in serializedMissiles)
            {
                Missiles.Enqueue(m);
            }

            _rigidbody = GetComponent<Rigidbody>();
        }


        public void FireNext()
        {
            Missiles.Dequeue().Fire();
        }

        public void AddMissile(Vector3 position, Quaternion rotation, MissilePreset preset, BodyComponent body)
        {
            Missile missile = Instantiate(defaultMissilePrefab, position, rotation, transform);
            Instantiate(body.prefab, position, rotation, missile.transform);
            Instantiate(preset.avionics.prefab, position - missile.transform.forward * 1.85f, rotation, missile.transform);
            
            missile.seeker = preset.seeker;
            missile.computer = preset.computer;
            missile.warhead = preset.warhead;
            missile.booster = preset.booster;
            missile.avionics = preset.avionics;

            missile.parentRigidbody = _rigidbody;
            missile.Select();
            Missiles.Enqueue(missile);
        }

    }
}