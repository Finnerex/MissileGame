using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace World
{
    public class LandingZone : MonoBehaviour
    {
        [SerializeField] private float jumpOutTimeSeconds = 3;

        private float _currentJumpTime;
        
        private void OnTriggerStay(Collider other)
        {
            
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            
            if (Input.GetKey(KeyCode.J)) // jump out to go home TODO: the plane should be slow i guess
                _currentJumpTime += Time.fixedDeltaTime;
            else
                _currentJumpTime = 0;

            if (_currentJumpTime > jumpOutTimeSeconds)
                SceneManager.LoadScene("HangarScene"); // return to hangar i guess
            
        }
    }
}