using System;
using UnityEngine;

namespace Hangar
{
    public class InteractController : MonoBehaviour
    {

        [SerializeField] private GameObject interactCrossHair;
        
        private Collider _lastPointedAtObject;
        private IInteractable _lastInteractable;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (!Physics.Raycast(_transform.position, _transform.forward, out RaycastHit hitInfo))
            {
                // Not pointing at anything
                interactCrossHair.SetActive(false);
                return;
            }
            
            // this so that i dont call get component as often
            Collider hitObject = hitInfo.collider;

            IInteractable hitInteractable;

            if (hitObject == _lastPointedAtObject) // pointing at the same object
                hitInteractable = _lastInteractable;
            else // pointing at a new object
            {
                _lastPointedAtObject = hitObject;
                // expensive method invocation can go suck mah bawls (it also shouldn't be that bad because of the prior check)
                hitInteractable = hitObject.GetComponent<IInteractable>();
                _lastInteractable = hitInteractable;
            }

            if (hitInteractable == null)
            {
                // Not pointing at interactable
                interactCrossHair.SetActive(false);
                return;
            }
            
            interactCrossHair.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Mouse0))
                hitInteractable.OnInteract();
            
        }
    }
}