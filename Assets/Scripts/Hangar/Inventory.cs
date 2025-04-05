using System;
using System.Collections.Generic;
using System.IO;
using Missiles;
using Missiles.Components;
using UnityEditor;
using UnityEngine;
using Utility;
using ComponentCost = Missiles.ComponentCost;

namespace Hangar
{
    public class Inventory : HangarInteractable // kinda doesnt need to extend this any more
    {
        // i would really appreciate it if i could load these dynamically using a file path but it seems that is very difficult
        public List<MissileComponent> componentInventory;
        public SerializableDictionary<ComponentItem, int> ItemInventory; // things used to make the components

        [SerializeField] private ComponentButton itemPrefab;
        [SerializeField] private GameObject componentUI;

        [SerializeField] private Transform[] componentTypeSections; // array is a thing here i guess

        public static Inventory Instance;

        private void Awake()
        {
            Instance = this;
            ItemInventory = PersistentDataHandler.LoadObjectOrNew<SerializableDictionary<ComponentItem, int>>("InventoryItems");

            // this might have to hella be changed
            if (SceneChangeDataManager.Instance == null)
            {
                Debug.Log("This is no good");
                return;
            }
            
            foreach (var entry in SceneChangeDataManager.Instance.LootedComponents)
                if (!ItemInventory.TryAdd(entry.Key, entry.Value))
                    ItemInventory[entry.Key] += entry.Value;

            // TODO: Add back the cost of the remaining missiles
            SceneChangeDataManager.Instance.LootedComponents = new Dictionary<ComponentItem, int>();

        }

        private void Start()
        {
            foreach (var component in componentInventory)
            {
                Transform parent = component switch // and this is kinda ass
                {
                    BodyComponent _ => componentTypeSections[0],
                    AvionicsComponent _ => componentTypeSections[1],
                    SeekerComponent _ => componentTypeSections[2],
                    ComputerComponent _ => componentTypeSections[3],
                    WarheadComponent _ => componentTypeSections[4],
                    _ => componentTypeSections[5], // BoosterComponent
                };
                
                ComponentButton button = Instantiate(itemPrefab, parent);
                // TODO: these should be the model or an icon
                button.SetText(string.IsNullOrEmpty(component.displayName) ? component.name : component.displayName);
                button.Component = component;
            }
            
            componentUI.SetActive(false); // has to be initially active for some reason or else the doggone buttons break
            
        }

        private void OnDestroy()
        {
            PersistentDataHandler.SaveObject("InventoryItems", ItemInventory);
        }
    }
}