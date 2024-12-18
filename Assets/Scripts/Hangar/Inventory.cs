using System;
using System.Collections.Generic;
using Missiles.Components;
using UnityEngine;

namespace Hangar
{
    public class Inventory : HangarInteractable
    {
        public List<MissileComponent> componentInventory;
        // public List<InventoryItem> ItemInventory;

        [SerializeField] private GameObject scrollView;
        [SerializeField] private ComponentButton itemPrefab;
        
        private Transform _componentInventoryContent;
        
        public static Inventory Instance;

        private void Awake()
        {
            Instance = this;
            _componentInventoryContent = scrollView.transform.Find("Viewport/Content");
        }
        
        
        public override void OnInteract()
        {
            base.OnInteract();
            // componentInventory.SetActive(true);

            ShowComponentInventory();
            
        }

        protected override void Exit()
        {
            base.Exit();

            HideComponentInventory();

            // presetsMenu.SetActive(false);
        }

        public void ShowComponentInventory()
        {
            foreach (var component in componentInventory)
            {
                // TODO: these should be the model or an icon
                ComponentButton button = Instantiate(itemPrefab, _componentInventoryContent);
                button.SetText(component.name);
                button.Component = component;
            }
        }

        public void HideComponentInventory()
        {
            foreach (var item in _componentInventoryContent)
            {
                Debug.Log("should remove object of type" + item.GetType());
                if (item is Component child)
                    Destroy(child.gameObject);
            }
        }

    }
}