using System;
using UnityEngine;

namespace Utility
{
    public class EffectController : MonoBehaviour
    {
        public GameObject explosionEffect;

        public static EffectController Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}