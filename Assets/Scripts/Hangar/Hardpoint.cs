using UnityEngine;

namespace Hangar
{
    public class Hardpoint : MonoBehaviour, IInteractable
    {

        [SerializeField] private int index;

        public void OnInteract()
        {
            HangarController.Instance.SetEmptyMissile(index, transform);
            // TODO: maybe turn off the icon here if not null
        }
    }
}