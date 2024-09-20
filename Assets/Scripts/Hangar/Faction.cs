using UnityEngine;

namespace Hangar
{
    public class Faction : MonoBehaviour, IInteractable
    {
        [SerializeField] private string factionName;
        
        public void OnInteract()
        {
            Debug.Log("interacted with faction: " + factionName);
        }
    }
}