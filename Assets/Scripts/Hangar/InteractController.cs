using System;
using UnityEngine;

namespace Hangar
{
    public class InteractController : MonoBehaviour
    {

        [SerializeField] private Camera cam;

        private void Update()
        {

            if (!Input.GetKeyDown(KeyCode.Mouse0))
                 return;
            
            if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo) ||
                !hitInfo.transform.TryGetComponent(out IInteractable interactable))
                return;
            
            interactable.OnInteract();

        }
    }
}