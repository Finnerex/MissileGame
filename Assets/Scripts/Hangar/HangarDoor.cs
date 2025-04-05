using System.Linq;
using Missiles;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hangar
{
    public class HangarDoor : MonoBehaviour, IInteractable
    {

        [SerializeField] private bool checkForCost;
        public void OnInteract()
        {
            
            if (checkForCost)
            {
                foreach (var entry in SceneChangeDataManager.Instance.WeaponSystemMissiles)
                {
                    ComponentCost[] cost = entry.Value.GetCost();

                    if (cost.Any(c =>
                            !Inventory.Instance.ItemInventory.TryGetValue(c.componentItem, out int amount) ||
                            amount < c.amount))
                        return; // TODO: Tell them why

                }

                // bad but simple and effective
                foreach (var entry in SceneChangeDataManager.Instance.WeaponSystemMissiles)
                {
                    ComponentCost[] cost = entry.Value.GetCost();

                    foreach (ComponentCost c in cost)
                        Inventory.Instance.ItemInventory[c.componentItem] -= c.amount;
                }
            }
            
            SceneManager.LoadScene("OutdoorsScene");
        }
    }
}