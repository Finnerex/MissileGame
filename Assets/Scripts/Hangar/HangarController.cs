using System;
using System.Collections.Generic;
using Missiles;
using Missiles.Components;
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
        [NonSerialized] public Vector3 lastCameraPos;

        [NonSerialized] public MissilePreset? CurrentlySelectedPreset;
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
            foreach (Transform child in t)
                if (child.name != "Icon") // wow what a fix
                    Destroy(child.gameObject);

            if (CurrentlySelectedPreset is null)
            {
                _aircraftMissilePresets.Remove(index);
                return;
            }
            
            Instantiate(CurrentlySelectedPreset.Value.body.prefab, t.position, t.rotation, t);
            Instantiate(CurrentlySelectedPreset.Value.avionics.prefab, t.position, t.rotation, t);
            
            _aircraftMissilePresets[index] = CurrentlySelectedPreset.Value;
        }

    }
}