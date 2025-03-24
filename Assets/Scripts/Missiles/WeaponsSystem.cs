using System.Collections.Generic;
using System.Linq;
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

        private readonly Queue<(string, Queue<Missile>)> _missiles = new(); // each sub-queue is a different missile type, item1 is name. This is convoluted but works

        [SerializeField] private Rigidbody rb;
        
        public Queue<Missile> currentMissileQueue => _missiles.Peek().Item2;
        public string currentMissileName => _missiles.Peek().Item1; 
        public Missile nextMissile => currentMissileQueue.TryPeek(out Missile next) ? next : null;
        
        public int missileCount { get; private set; }


        public bool TryGetNext(out Missile next)
        {
            return currentMissileQueue.TryPeek(out next);
        }

        public void FireNext()
        {
            missileCount--;
            currentMissileQueue.Dequeue().Fire(); // likely should add some sort of check here
            
            // if (Missiles.Peek().Count == 0) // if out of this type of missile, remove it from selection, might not need/want this 
                // Missiles.Dequeue();
        }

        public void CycleMissile()
        {
            _missiles.Enqueue(_missiles.Dequeue());
        }

        private void AddMissile(Vector3 position, Quaternion rotation, MissilePreset preset)
        {
            Missile missile = Instantiate(defaultMissilePrefab, position, rotation, transform);
            Transform spawnedBody = Instantiate(preset.body.prefab, missile.transform).transform;
            Instantiate(preset.avionics.prefab, missile.transform).transform.position = spawnedBody.position;

            // missile should likely have an Initialize(MissilePreset)
            missile.presetName = preset.name;
            
            missile.seeker = preset.seeker;
            missile.computer = preset.computer;
            missile.warhead = preset.warhead;
            missile.booster = preset.booster;
            missile.avionics = preset.avionics;
            
            missile.parentRigidbody = rb;
            // missile.Select();
            
            AddByNameOrNew(missile);
            
            missileCount++;
        }

        private void AddByNameOrNew(Missile missile)
        {
            // find missile sub-queue with the same name
            Queue<Missile> toAddTo = _missiles.FirstOrDefault(typeQueue => typeQueue.Item1 == missile.presetName).Item2;

            if (toAddTo != null)
                toAddTo.Enqueue(missile);
            else
                _missiles.Enqueue((missile.presetName, new Queue<Missile>(new[] { missile }))); // cool allocation bruh \s
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