using System;
using Missiles;
using Missiles.Components;
using TMPro;
using UnityEngine;
using Utility;


namespace Hangar
{
    public class MissileWorkbench : HangarInteractable // kinda doesnt need to extend this any more
    {
        public MissilePreset currentPreset;
        [NonSerialized] public SerializableDictionary<string, MissilePreset> SavedPresets;

        [SerializeField] private Transform missileParent;
        [SerializeField] private TMP_InputField textEntryField;
        [SerializeField] private TextMeshProUGUI statsText;
        
        [SerializeField] private Transform presetMenuContent;
        [SerializeField] private PresetButton presetButtonPrefab;

        private int _remainingCapacity;
        

        // a solution that work however unproud of it i am
        private GameObject _spawnedBody;
        private GameObject _spawnedFins;
        
        // public TechTreeNode currentResearchNode;

        public static MissileWorkbench Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SavedPresets = PersistentDataHandler.LoadObjectOrNew<SerializableDictionary<string, MissilePreset>>("MissilePresets");
            
            // add buttons to the menu
            foreach (var entry in SavedPresets)
                Instantiate(presetButtonPrefab, presetMenuContent).SetText(entry.Key);
        }

        public bool AddMissileComponent(MissileComponent component)
        {

            if (component is BodyComponent body)
            {
                Destroy(_spawnedBody);
                Destroy(_spawnedFins);
                _spawnedBody = Instantiate(body.prefab, missileParent);
                currentPreset.body = body;

                _remainingCapacity = body.size;

                // still a little ugly
                currentPreset.seeker = null;
                currentPreset.computer = null;
                currentPreset.warhead = null;
                currentPreset.booster = null;
                currentPreset.avionics = null;

                SetStatsText();
                return true;
            }

            if (currentPreset.body is null)
                return false; // cannot be added

            switch (component)
            {
                case SeekerComponent seeker:
                    ReplaceComponent(ref currentPreset.seeker, seeker); break;
                case ComputerComponent guidanceComputer:
                    ReplaceComponent(ref currentPreset.computer, guidanceComputer); break;
                case WarheadComponent warhead:
                    ReplaceComponent(ref currentPreset.warhead, warhead); break;
                case BoosterComponent booster:
                    ReplaceComponent(ref currentPreset.booster, booster); break;
                case AvionicsComponent avionics:
                    ReplaceComponent(ref currentPreset.avionics, avionics);
                    Destroy(_spawnedFins);
                    _spawnedFins = Instantiate(avionics.prefab, _spawnedBody.transform.position, _spawnedBody.transform.rotation, missileParent);
                    break;
            }
            
            SetStatsText();
            
            return true; // could still actually be false at this point because i didnt do the size check
        }

        private void ReplaceComponent<T>(ref T oldComponent, T newComponent) where T : MissileComponent
        {
            // wt frick
            if (!((oldComponent != null && newComponent.size <= _remainingCapacity + oldComponent.size) || newComponent.size <= _remainingCapacity))
                return;

            if (oldComponent != null)
                _remainingCapacity += oldComponent.size;
            
            _remainingCapacity -= newComponent.size;
            
            oldComponent = newComponent;
        }
        
        public void Save()
        {
            if (!currentPreset.IsValid) return;
            
            if (SavedPresets.TryAdd(textEntryField.text, currentPreset))
                Instantiate(presetButtonPrefab, presetMenuContent).SetText(textEntryField.text);
            else // add overwrite dialogue box or something
                SavedPresets[textEntryField.text] = currentPreset;
                
            // textEntryField.text = "";

        }

        public void Load(string presetName)
        {
            MissilePreset newPreset = SavedPresets[presetName];
            
            // ass code right here
            // with hindsight the previous statement holds true
            AddMissileComponent(newPreset.body);
            AddMissileComponent(newPreset.seeker);
            AddMissileComponent(newPreset.computer);
            AddMissileComponent(newPreset.warhead);
            AddMissileComponent(newPreset.booster);
            AddMissileComponent(newPreset.avionics);
        }

        private void SetStatsText()
        {
            string message = "";

            // this is pretty objectively bad code, yet another result of my choice to not use an array
            if (currentPreset.body != null)
                message += $"Space Available: {_remainingCapacity}u/{currentPreset.body.size}u";

            if (currentPreset.avionics != null)
                message += "\n\nAvionics\n" + currentPreset.avionics;
            
            if (currentPreset.seeker != null)
                message += "\n\nSeeker\n" + currentPreset.seeker;
            
            if (currentPreset.computer != null)
                message += "\n\nComputer\n" + currentPreset.computer;
            
            if (currentPreset.warhead != null)
                message += "\n\nWarhead\n" + currentPreset.warhead;
            
            if (currentPreset.booster != null)
                message += "\n\nBooster\n" + currentPreset.booster;


            statsText.text = message;
        }
        
        
        private void OnDestroy()
        {
            PersistentDataHandler.SaveObject("MissilePresets", SavedPresets);
        }

    }
}