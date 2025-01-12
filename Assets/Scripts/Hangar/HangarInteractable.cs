using System.Linq;
using UnityEngine;

namespace Hangar
{
    public class HangarInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject[] ui;
        [SerializeField] private GameObject cameraPoint;
        [SerializeField] private bool lockCamera = true;

        protected static bool AnySelected;
        private bool _thisSelected;

        public virtual void OnInteract()
        {
            if (AnySelected)
                return;
            
            AnySelected = true;
            _thisSelected = true;

            foreach (GameObject element in ui)
                element.SetActive(true);

            HangarController.Instance.lastCameraPos = HangarController.Instance.orbitCamera.transform.position;
            HangarController.Instance.orbitCamera.transform.position = cameraPoint.transform.position;
            HangarController.Instance.orbitCamera.transform.rotation = cameraPoint.transform.rotation;
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            
            if (!lockCamera)
            {
                HangarController.Instance.orbitCamera.SwitchTo(transform);
                HangarController.Instance.orbitCamera.transform.LookAt(transform.position);

                return;
            }

            HangarController.Instance.orbitCamera.enabled = false;
            
        }

        private void Update()
        {
            if (!_thisSelected)
                return;
            
            if (Input.GetKey(KeyCode.Escape))
                Exit();
        }

        protected virtual void Exit()
        {
            foreach (GameObject element in ui)
                element.SetActive(false);

            HangarController.Instance.orbitCamera.enabled = true;

            HangarController.Instance.orbitCamera.transform.position = HangarController.Instance.lastCameraPos;
            
            HangarController.Instance.orbitCamera.SwitchTo(HangarController.Instance.defaultOrbitPoint.transform);
            HangarController.Instance.orbitCamera.transform.LookAt(HangarController.Instance.defaultOrbitPoint.transform);

            AnySelected = false;
            _thisSelected = false;
        }
        
    }
}