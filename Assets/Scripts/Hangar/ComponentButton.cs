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
        
        // probably do like count or something

        public void Add()
        {
            MissileWorkbench.Instance.AddMissileComponent(Component);
        }

        public void SetText(string newText) // will change for like icon or something
        {
            text.text = newText;
        }
    }
}