using UnityEngine;

namespace Hangar
{
    public class Aircraft : HangarInteractable
    {
        [SerializeField] private Collider interactCollider;

        [SerializeField] private GameObject[] icons;
        public override void OnInteract()
        {
            if (AnySelected)
                return;
            
            base.OnInteract();

            interactCollider.enabled = false;

            foreach (GameObject icon in icons)
                icon.SetActive(true);
            
        }


        protected override void Exit()
        {
            base.Exit();

            interactCollider.enabled = true;

            foreach (GameObject icon in icons)
                icon.SetActive(false);
        }

        private void SelectRemoveMissile()
        {
            HangarController.Instance.CurrentlySelectedPreset = null;
        }
        
    }
}