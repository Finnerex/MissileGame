using System.Collections.Generic;
using Missiles;
using UnityEngine;

namespace Hangar
{
    public class SceneChangeDataManager : MonoBehaviour
    {

        public static SceneChangeDataManager Instance;

        public Dictionary<int, MissilePreset> WeaponSystemMissiles;
        
        private void Start()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}