using System;
using UnityEngine;

namespace Utility
{
    public class TimedDestroy : MonoBehaviour
    {
        [SerializeField] private float destroyTimeSeconds;
        private float _lifetimeSeconds;

        // me when i didn't realize you can do Destroy(gameObject, time)
        private void Update()
        {
            _lifetimeSeconds += Time.deltaTime;
            
            if (_lifetimeSeconds > destroyTimeSeconds)
                Destroy(gameObject);
        }
    }
}