using System;
using TMPro;
using UnityEngine;
using Utility;

namespace Player
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;

        public CircleGraphic missileLockCircle;
        public RectTransform missileGimbalCircle;

        [NonSerialized] public RectTransform MissileLockRect;

        public TextMeshProUGUI selectedMissileText;
        
        private void Awake()
        {
            Instance = this;
            MissileLockRect = missileLockCircle.GetComponent<RectTransform>();
        }

        
        
    }
}