using System;
using TMPro;
using UnityEngine;

namespace Player
{
    public class ObjectMarkerIcon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public RectTransform rectTransform;
        [NonSerialized] public float Distance;

        private void Update()
        {
            text.text = Distance > 1000 ? $"{Distance / 1000:F2} km" : $"{Distance:F0} m";
        }
    }
}