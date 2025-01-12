using System.Collections.Generic;
using Targeting;
using UnityEngine;

namespace Missiles
{
    public class WeaponsSystem : MonoBehaviour
    {
        [SerializeField] private Missile defaultMissilePrefab;
        [SerializeField] private Transform[] attachmentPoints;
        // TODO: add back serialize missile presets for npc aircraft
        
        [SerializeField] private CountermeasureDispenser[] countermeasureDispensers;
        
        public readonly Queue<Missile> Missiles = new();

        [SerializeField] private Rigidbody rb;
        
        
        public void FireNext()
        {
            Missiles.Dequeue().Fire();
        }

        private void AddMissile(Vector3 position, Quaternion rotation, MissilePreset preset)
        {
            Missile missile = Instantiate(defaultMissilePrefab, position, rotation, transform);
            Instantiate(preset.body.prefab, position, rotation, missile.transform);
            Instantiate(preset.avionics.prefab, position, rotation, missile.transform);
            
            missile.seeker = preset.seeker;
            missile.computer = preset.computer;
            missile.warhead = preset.warhead;
            missile.booster = preset.booster;
            missile.avionics = preset.avionics;
            
            missile.parentRigidbody = rb;
            // missile.Select();
            Missiles.Enqueue(missile);
        }

        public void SetMissile(MissilePreset preset, int index)
        {
            Transform t = attachmentPoints[index];
            AddMissile(t.position, t.rotation, preset);
        }

        public void DeployFlares()
        {
            foreach (CountermeasureDispenser d in countermeasureDispensers)
                d.DeployFlare();
        }

    }
}