using System;
using System.Collections.Generic;
using Missiles;
using UnityEngine;
using Utility;

namespace Hangar
{
    public class HangarController : MonoBehaviour
    {
        public static HangarController Instance;
        
        public OrbitCamera orbitCamera;
        // public GameObject defaultOrbitPoint;
        [NonSerialized] public Vector3 lastCameraPos;

        [NonSerialized] public MissilePreset? CurrentlySelectedPreset;
        // private readonly Dictionary<int, MissilePreset> _aircraftMissilePresets = new();

        private void Awake()
        {
            Instance = this;
        }

        // private void OnDestroy()
        // {
        //     SceneChangeDataManager.Instance.WeaponSystemMissiles = _aircraftMissilePresets;
        // }

        public void SetEmptyMissile(int index, Transform t)
        {
            foreach (Transform child in t)
                if (child.name != "Icon") // wow what a fix
                    Destroy(child.gameObject);

            if (CurrentlySelectedPreset is null)
            {
                SceneChangeDataManager.Instance.WeaponSystemMissiles.Remove(index);
                return;
            }
            
            GameObject avionicsPrefab = CurrentlySelectedPreset.Value.avionics.prefab;
            
            GameObject body = Instantiate(CurrentlySelectedPreset.Value.body.prefab, t);
            Instantiate(avionicsPrefab, body.transform.position, t.rotation * avionicsPrefab.transform.rotation, t);

            SceneChangeDataManager.Instance.WeaponSystemMissiles[index] = CurrentlySelectedPreset.Value;
        }

    }
}