using System;
using System.Collections.Generic;
using System.Linq;
using Missiles;
using Missiles.Components;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utility;


namespace Hangar
{
    public class MissileWorkbench : HangarInteractable
    {
        public MissilePreset currentPreset;
        [NonSerialized] public SerializableDictionary<string, MissilePreset> SavedPresets;

        [SerializeField] private Transform missileParent;
        [SerializeField] private TMP_InputField textEntryField;
        [SerializeField] private GameObject presetsMenu;
        [SerializeField] private PresetButton presetButtonPrefab;
        [SerializeField] private GameObject componentsInventoryMenu;
        
        private int _remainingCapacity;
        private Transform _presetMenuContent;

        // public TechTreeNode currentResearchNode;

        public static MissileWorkbench Instance;

        private void Awake()
        {
            Instance = this;
            _presetMenuContent = presetsMenu.transform.Find("Viewport/Content");
            SavedPresets = PersistentDataHandler.LoadObjectOrNew<SerializableDictionary<string, MissilePreset>>("MissilePresets");
        }

        public bool AddMissileComponent(MissileComponent component)
        {

            Debug.Log("adding the jawn called: " + component.name);
            
            if (component is BodyComponent body)
            {
                Debug.Log("buddy is a body");
                ReplaceComponent(ref currentPreset.body, body);
                
                _remainingCapacity = body.size;

                // still a little ugly
                ReplaceComponent(ref currentPreset.seeker, null);
                ReplaceComponent(ref currentPreset.computer, null);
                ReplaceComponent(ref currentPreset.warhead, null);
                ReplaceComponent(ref currentPreset.booster, null);
                ReplaceComponent(ref currentPreset.avionics, null);

                return true;
            }

            if (component.size > _remainingCapacity)
                return false; // cannot be added

            _remainingCapacity -= component.size;

            switch (component)
            {
                case SeekerComponent seeker:
                    ReplaceComponent(ref currentPreset.seeker, seeker); break;
                case ComputerComponent guidanceComputer:
                    ReplaceComponent(ref currentPreset.computer, guidanceComputer); break;
                // _missile.Computer = guidanceComputer;
                case WarheadComponent warhead:
                    ReplaceComponent(ref currentPreset.warhead, warhead); break;
                case BoosterComponent booster:
                    ReplaceComponent(ref currentPreset.booster, booster); break;
                case AvionicsComponent avionics:
                    ReplaceComponent(ref currentPreset.avionics, avionics); break;
            }


            ResetPositions();
            return true;
        }

        // this is cool asf b/c you normally cant do ref polymorphism
        private void ReplaceComponent<T>(ref T oldComponent, T newComponent) 
        where T : MissileComponent
        {
            if (oldComponent is not null)
                Destroy(oldComponent.SpawnedObject); // despawn the old object

            if (newComponent is not null)
            {
                // not simply deleted
                newComponent.SpawnedObject = Instantiate(newComponent.prefab, missileParent); // spawn the new one, will be moved
                newComponent.SpawnedObject.transform.localScale = Vector3.one * newComponent.scaleFactor;
            }

            oldComponent = newComponent;
        }

        private void ResetPositions()
        {
            int nextItemPosition = 0;

            nextItemPosition = SetPosition(currentPreset.seeker, nextItemPosition);
            nextItemPosition = SetPosition(currentPreset.warhead, nextItemPosition);
            nextItemPosition = SetPosition(currentPreset.computer, nextItemPosition);
            nextItemPosition = SetPosition(currentPreset.avionics, nextItemPosition);

            SetPosition(currentPreset.booster, nextItemPosition);

        }

        private int SetPosition(MissileComponent component, int nextItemPosition)
        {
            if (component is null)
                return nextItemPosition + 1;

            component.SpawnedObject.transform.position = currentPreset.body.SpawnedObject.transform.position -
                                                         currentPreset.body.SpawnedObject.transform.forward *
                                                         nextItemPosition /* with a transformation */;
            
            return nextItemPosition + component.size;
        }
        
        public void Save()
        {
            if (true/*preset.IsValid*/)
            {
                Debug.Log("saving preset with name " + textEntryField.text);
                Debug.Log($"with components: {currentPreset.avionics.name}, {currentPreset.body.name}, {currentPreset.computer.name}, {currentPreset.booster.name}, {currentPreset.warhead.name}, {currentPreset.seeker.name}");
                
                if (SavedPresets.TryAdd(textEntryField.text, currentPreset))
                    Instantiate(presetButtonPrefab, _presetMenuContent).SetText(textEntryField.text);
                else // add overwrite dialogue box or something
                    SavedPresets[textEntryField.text] = currentPreset;
                
                // textEntryField.text = "";
            }

            foreach (var entry in SavedPresets)
            {
                Debug.Log(entry.Key);
            }
            
        }

        public void Load(string presetName)
        {
            MissilePreset newPreset = SavedPresets[presetName];
            
            Debug.Log("loading preset with name " + presetName);
            Debug.Log($"with components: {newPreset.avionics.name}, {newPreset.body.name}, {newPreset.computer.name}, {newPreset.booster.name}, {newPreset.warhead.name}, {newPreset.seeker.name}");
            
            // ass code right here
            // with hindsight the previous statement holds true
            AddMissileComponent(newPreset.body);
            AddMissileComponent(newPreset.seeker);
            AddMissileComponent(newPreset.computer);
            AddMissileComponent(newPreset.warhead);
            AddMissileComponent(newPreset.booster);
            AddMissileComponent(newPreset.avionics);
        } 

        public override void OnInteract()
        {
            base.OnInteract();
            presetsMenu.SetActive(true);
            componentsInventoryMenu.SetActive(true);
            Inventory.Instance.ShowComponentInventory();

            foreach (var entry in SavedPresets)
            {
                Instantiate(presetButtonPrefab, _presetMenuContent).SetText(entry.Key);
            }
        }

        protected override void Exit()
        {
            base.Exit();

            foreach (var item in _presetMenuContent)
            {
                Debug.Log("should remove object of type" + item.GetType());
                if (item is Component child)
                    Destroy(child.gameObject);
            }
            
            presetsMenu.SetActive(false);
            componentsInventoryMenu.SetActive(false);
            Inventory.Instance.HideComponentInventory();
        }

        private void OnDestroy()
        {
            PersistentDataHandler.SaveObject("MissilePresets", SavedPresets);
        }
    }
}