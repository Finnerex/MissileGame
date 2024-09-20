using System;
using UnityEngine;

namespace Hangar
{
    public class HangarInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private GameObject cameraPoint;

        public void OnInteract()
        {
            ui.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            // HangarController.Instance.orbitCamera.center = gameObject;
            HangarController.Instance.orbitCamera.transform.position = cameraPoint.transform.position;
            HangarController.Instance.orbitCamera.transform.rotation = cameraPoint.transform.rotation;
            // HangarController.Instance.orbitCamera.transform.LookAt(transform.position);
            HangarController.Instance.orbitCamera.enabled = false;

        }

        private void Update()
        {
            if (!ui.activeSelf)
                return;
            
            if (!Input.GetKey(KeyCode.Escape))
                return;
            
            ui.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            // HangarController.Instance.orbitCamera.center = HangarController.Instance.defaultOrbitPoint;
            HangarController.Instance.orbitCamera.enabled = true;

        }
    }
}