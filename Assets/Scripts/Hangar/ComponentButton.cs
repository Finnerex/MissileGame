using System;
using Missiles.Components;
using TMPro;
using UnityEngine;

namespace Hangar
{
    public class ComponentButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [NonSerialized] public MissileComponent Component;

        [SerializeField] private MouseFollower tooltipPrefab;
        private MouseFollower _spawnedTooltip;
        
        // probably do like count or something

        public void Add()
        {
            MissileWorkbench.Instance.AddMissileComponent(Component);
        }

        public void SetText(string newText) // will change for like icon or something
        {
            text.text = newText;
        }

        public void OnHover()
        {
            _spawnedTooltip = Instantiate(tooltipPrefab, Input.mousePosition, Quaternion.identity, transform.parent.parent.parent.parent.parent); // lmao i love programming
            _spawnedTooltip.transform.SetAsLastSibling();
            _spawnedTooltip.SetText(Component.ToString());
        }

        public void OnStopHover()
        {
            if (_spawnedTooltip != null)
                Destroy(_spawnedTooltip.gameObject);
        }

        private void OnDisable()
        {
            OnStopHover();
        }
    }
}