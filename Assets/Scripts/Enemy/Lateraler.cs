using System;
using Targeting;
using UnityEngine;
using Random = System.Random;


namespace Enemy
{
    public class Lateraler : MonoBehaviour
    {
        [SerializeField] private float speed = 20;
        [SerializeField] private Transform center;
        [SerializeField] private float dist;

        [SerializeField] private CountermeasureDispenser flares;

        private Random _random = new();

        private bool inverted;
        
        private void Update()
        {
            transform.position += (inverted ? -1 : 1) * speed * transform.forward;

            if ((center.position - transform.position).magnitude > dist)
                inverted = !inverted;

            if (_random.NextDouble() < 0.001)
                flares.DeployFlare();
            
        }
    }
}