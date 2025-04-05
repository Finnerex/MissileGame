using System.Collections.Generic;
using Missiles;
using UnityEngine;

namespace Hangar
{
    public class SceneChangeDataManager : MonoBehaviour
    {

        public static SceneChangeDataManager Instance;

        public Dictionary<int, MissilePreset> WeaponSystemMissiles = new();

        public Dictionary<ComponentItem, int> LootedComponents = new();

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}