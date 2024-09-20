using System;
using UnityEngine;

namespace Enemy
{
    public class Circler : MonoBehaviour
    {
        [SerializeField] private Transform center;
        [SerializeField] private float speed = 1;

        private void Update()
        {
            transform.RotateAround(center.position, center.up, speed * Time.deltaTime);
        }
    }
}