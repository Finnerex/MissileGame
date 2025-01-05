using UnityEngine;

namespace Hangar
{
    public class Hardpoint : MonoBehaviour, IInteractable
    {

        [SerializeField] private int index;

        // TODO: Have some sort of something to show the user they can put a missile on the jawn
        
        public void OnInteract()
        {
            HangarController.Instance.SetEmptyMissile(index, transform);
        }
    }
}