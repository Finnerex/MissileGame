using System;
using System.Collections.Generic;
using Missiles;
using Player;
using UnityEngine;
using Utility;

namespace Hangar
{
    public class HangarController : MonoBehaviour
    {
        public static HangarController Instance;
        
        public OrbitCamera orbitCamera;
        public GameObject defaultOrbitPoint;

        [NonSerialized] public MissilePreset? CurrentlySelectedPreset; 
        // public WeaponsSystem hangarWeaponSystem;
        private readonly Dictionary<int, MissilePreset> _aircraftMissilePresets = new();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            SceneChangeDataManager.Instance.WeaponSystemMissiles = _aircraftMissilePresets;
        }

        public void SetEmptyMissile(int index, Transform t)
        {
            if (CurrentlySelectedPreset is null) return;
            
            foreach (Transform child in t)
                Destroy(child.gameObject);
            
            Instantiate(CurrentlySelectedPreset.Value.body.prefab, t.position, t.rotation, t);
            Instantiate(CurrentlySelectedPreset.Value.avionics.prefab, t.position - t.transform.forward * 1.85f, t.rotation, t);
            
            _aircraftMissilePresets[index] = CurrentlySelectedPreset.Value;
        }
    }
}