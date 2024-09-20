using System;
using UnityEngine;
using Utility;

namespace Hangar
{
    public class HangarController : MonoBehaviour
    {
        public OrbitCamera orbitCamera;
        public GameObject defaultOrbitPoint;
        
        
        public static HangarController Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}