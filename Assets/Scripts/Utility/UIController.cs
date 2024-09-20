using System;
using UnityEngine;

namespace Utility
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;
        
        public CircleGraphic missileLockCircle;
        public RectTransform missileGimbalCircle;

        [NonSerialized] public RectTransform MissileLockRect;
        
        private void Awake()
        {
            Instance = this;
            MissileLockRect = missileLockCircle.GetComponent<RectTransform>();
        }

        
        
    }
}