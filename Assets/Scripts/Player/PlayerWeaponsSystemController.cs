using System;
using System.Linq;
using Hangar;
using Missiles;
using Unity.VisualScripting;
using UnityEngine;
using Utility;

namespace Player
{
    public class PlayerWeaponsSystemController : MonoBehaviour
    {

        [SerializeField] private WeaponsSystem weaponsSystem;
        // [SerializeField] private MissilePreset testPreset;

        private void Awake()
        {
            foreach (var pair in SceneChangeDataManager.Instance.WeaponSystemMissiles)
            {
                weaponsSystem.SetMissile(pair.Value, pair.Key);
                Debug.Log($"Adding a missile at point #{pair.Key}");
            }
            
        }

        private void Update()
        {

            if (Input.GetMouseButtonDown(4))
            {
                weaponsSystem.FireNext();
                if (weaponsSystem.Missiles.TryPeek(out Missile m1))
                    SelectMissile(m1);
            }
            
            // this is not how i want this to work
            if (Input.GetMouseButtonDown(3) && weaponsSystem.Missiles.TryPeek(out Missile m2))
                SelectMissile(m2);
            
            // if (Input.GetKeyDown(KeyCode.T))
            //     weaponsSystem.AddMissile(transform.position - transform.up * 2, transform.rotation, testPreset);
            
            if (Input.GetKeyDown(KeyCode.Space))
                weaponsSystem.DeployFlares();

            if (weaponsSystem.Missiles.TryPeek(out Missile m3))
                UIController.Instance.missileLockCircle.color = m3.HasLock ?
                new Color(0.8f, 0.28f, 0.2f) : new Color(0.75f, 0.75f, 0.7f);

        }


        private void SelectMissile(Missile missile)
        {
            float circleDiameter = 2 * Mathf.Tan(missile.seeker.fieldOfView * 0.5f * Mathf.Deg2Rad) * 500;
            UIController.Instance.MissileLockRect.sizeDelta = new Vector2(circleDiameter, circleDiameter);

            if (missile.seeker.isCaged)
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(false);
            else
            {
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(true);
                circleDiameter = 2 * Mathf.Tan((missile.seeker.maxGimbalAngle + missile.seeker.fieldOfView) * 0.5f * Mathf.Deg2Rad) * 500;
                UIController.Instance.missileGimbalCircle.sizeDelta = new Vector2(circleDiameter, circleDiameter);
            }
            
            missile.Select();
            
            // maybe should be removed when fired or idk really how to handle this
            ForwardUiIcon seekerIcon = missile.seekerHead.AddComponent<ForwardUiIcon>();
            seekerIcon.icon = UIController.Instance.MissileLockRect;
            missile.AddComponent<ForwardUiIcon>().icon = UIController.Instance.missileGimbalCircle;
        }
    }
}