using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hangar
{
    public class HangarDoor : MonoBehaviour, IInteractable
    {
        public void OnInteract()
        {
            SceneManager.LoadScene("OutdoorsScene");
        }
    }
}