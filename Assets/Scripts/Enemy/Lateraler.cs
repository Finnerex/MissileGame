using System;
using UnityEngine;

namespace Enemy
{
    public class Lateraler : MonoBehaviour
    {
        [SerializeField] private float speed = 20;
        [SerializeField] private Transform center;
        [SerializeField] private float dist;

        private bool inverted;
        
        private void Update()
        {
            transform.position += (inverted ? -1 : 1) * speed * transform.forward;

            if ((center.position - transform.position).magnitude > dist)
                inverted = !inverted;

        }
    }
}