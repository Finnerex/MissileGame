using System;
using TMPro;
using UnityEngine;

namespace Hangar
{
    public class MouseFollower : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Update()
        {
            transform.position = Input.mousePosition;
        }

        public void SetText(string newText)
        {
            text.text = newText;
        }
    }
}