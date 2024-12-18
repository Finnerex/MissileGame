using System;
using System.Linq;
using Missiles;
using Missiles.Components;
using Targeting;
using UnityEngine;

namespace Player
{
    public class PlayerWeaponsSystemController : MonoBehaviour
    {

        [SerializeField] private WeaponsSystem weaponsSystem;
        [SerializeField] private MissilePreset testPreset;
        [SerializeField] private Transform[] hardpoints; // TODO: this

        [SerializeField] private CountermeasureDispenser[] countermeasureDispensers;
        

        private void Update()
        {
            if (Input.GetMouseButtonDown(4))
                weaponsSystem.FireNext();
            
            // this is not how i want this to work
            if (Input.GetMouseButtonDown(3))
                weaponsSystem.Missiles.Peek().Select();
            
            if (Input.GetKeyDown(KeyCode.T))
                weaponsSystem.AddMissile(transform.position - transform.up * 2, transform.rotation, testPreset);
            
            if (Input.GetKeyDown(KeyCode.Space))
                foreach (CountermeasureDispenser d in countermeasureDispensers) d.DeployFlare();
            
        }
    }
}