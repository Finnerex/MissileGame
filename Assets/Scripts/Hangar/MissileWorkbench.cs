using System.Collections.Generic;
using Missiles;
using Missiles.Components;
using TMPro;
using UnityEngine;


namespace Hangar
{
    public class MissileWorkbench : HangarInteractable
    {
        public MissilePreset preset;
        public Dictionary<string, MissilePreset> SavedPresets = new(); // wont be this when they actually get saved

        [SerializeField] private Transform missileParent;
        [SerializeField] private TMP_InputField textEntryField;
        [SerializeField] private GameObject presetsMenu;
        [SerializeField] private PresetButton presetButtonPrefab;
        [SerializeField] private GameObject componentsInventoryMenu;
        
        private int _remainingCapacity;
        private Transform _presetMenuContent;

        // public TechTreeNode currentResearchNode;

        // i dont know if i should use instance or getcomponent
        // my heart says static is bad but my brain says getcomponent is slow
        public static MissileWorkbench Instance;

        private void Awake()
        {
            Instance = this;
            _presetMenuContent = presetsMenu.transform.Find("Viewport/Content");
        }

        public bool AddMissileComponent(MissileComponent component)
        {

            if (component is BodyComponent body)
            {
                ReplaceComponent(ref preset.body, body);
                
                _remainingCapacity = body.size;

                // still a little ugly
                ReplaceComponent(ref preset.seeker, null);
                ReplaceComponent(ref preset.computer, null);
                ReplaceComponent(ref preset.warhead, null);
                ReplaceComponent(ref preset.booster, null);
                ReplaceComponent(ref preset.avionics, null);

                return true;
            }

            if (component.size > _remainingCapacity)
                return false; // cannot be added

            _remainingCapacity -= component.size;

            switch (component)
            {
                case SeekerComponent seeker:
                    ReplaceComponent(ref preset.seeker, seeker);
                    break;
                case ComputerComponent guidanceComputer:
                    ReplaceComponent(ref preset.computer, guidanceComputer);
                    break;
                // _missile.Computer = guidanceComputer;
                case WarheadComponent warhead:
                    ReplaceComponent(ref preset.warhead, warhead);
                    break;
                case BoosterComponent booster:
                    ReplaceComponent(ref preset.booster, booster);
                    break;
                case AvionicsComponent avionics:
                    ReplaceComponent(ref preset.avionics, avionics);
                    break;
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

            nextItemPosition = SetPosition(preset.seeker, nextItemPosition);
            nextItemPosition = SetPosition(preset.warhead, nextItemPosition);
            nextItemPosition = SetPosition(preset.computer, nextItemPosition);
            nextItemPosition = SetPosition(preset.avionics, nextItemPosition);

            SetPosition(preset.booster, nextItemPosition);

        }

        private int SetPosition(MissileComponent component, int nextItemPosition)
        {
            if (component is null)
                return nextItemPosition + 1;

            component.SpawnedObject.transform.position = preset.body.SpawnedObject.transform.position -
                                                         preset.body.SpawnedObject.transform.forward *
                                                         nextItemPosition /* with a transformation */;
            
            return nextItemPosition + component.size;
        }
        
        public void Save()
        {
            if (true/*preset.IsValid*/)
            {
                Debug.Log("saving preset with name " + textEntryField.text);
                
                if (SavedPresets.TryAdd(textEntryField.text, preset))
                    Instantiate(presetButtonPrefab, _presetMenuContent).SetText(textEntryField.text);
                else
                    SavedPresets[textEntryField.text] = preset;
                
                // textEntryField.text = "";
            }

            foreach (var entry in SavedPresets)
            {
                Debug.Log(entry.Key);
            }
            
        }

        public void Load(string presetName)
        {
            preset = SavedPresets[presetName];
            // ass code right here
            AddMissileComponent(preset.body);
            AddMissileComponent(preset.seeker);
            AddMissileComponent(preset.computer);
            AddMissileComponent(preset.warhead);
            AddMissileComponent(preset.booster);
            AddMissileComponent(preset.avionics);
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
    }
}