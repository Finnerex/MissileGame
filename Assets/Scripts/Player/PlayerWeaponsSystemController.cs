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

        [SerializeField] private float missileCircleStayTime = 5;

        private float _currentCircleTime;
        private bool _shouldCountDown;

        private bool _hasSetupMissile;

        private ForwardUiIcon _gimbalIconer;
        private ForwardUiIcon _seekerIconer;

        private void Awake()
        {
            foreach (var pair in SceneChangeDataManager.Instance.WeaponSystemMissiles)
                weaponsSystem.SetMissile(pair.Value, pair.Key);
        }

        private void Update()
        {
         
            if (Input.GetKeyDown(KeyCode.Space))
                weaponsSystem.DeployFlares();

            // bool hasNext =; // maybe should be move to weapon system or smthing
            if (!weaponsSystem.Missiles.TryPeek(out Missile nextMissile)) return;
            // all of the following will only happen if the system has a next missile

            if (Input.GetKeyDown(KeyCode.Mouse2))
                nextMissile.Unlock();
            
            if (Input.GetKeyDown(KeyCode.Mouse3))
            {
                SelectMissile(nextMissile);
                
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(true);
                UIController.Instance.MissileLockRect.gameObject.SetActive(true);
                _currentCircleTime = missileCircleStayTime;
                _shouldCountDown = true;
            }

            if (_shouldCountDown && _currentCircleTime > 0)
            {
                if (nextMissile.hasLock)
                    _currentCircleTime = missileCircleStayTime;
                else
                    _currentCircleTime -= Time.deltaTime;
            }
            else if (_shouldCountDown)
            {
                _shouldCountDown = false;
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(false);
                UIController.Instance.MissileLockRect.gameObject.SetActive(false);
                nextMissile.selected = false;
            }

            UIController.Instance.missileLockCircle.color = nextMissile.hasLock ? 
                new Color(0.8f, 0.28f, 0.2f) : new Color(0.75f, 0.75f, 0.7f);

            if (Input.GetKeyDown(KeyCode.Mouse4) && nextMissile.hasLock)
            {
                weaponsSystem.FireNext();

                Destroy(_gimbalIconer);
                Destroy(_seekerIconer);
                
                _hasSetupMissile = false;
                
                if (weaponsSystem.Missiles.TryPeek(out Missile m1))
                    SelectMissile(m1);
                else
                {
                    UIController.Instance.missileGimbalCircle.gameObject.SetActive(false);
                    UIController.Instance.MissileLockRect.gameObject.SetActive(false);
                }
            }
            
        }


        private void SelectMissile(Missile missile)
        {
            missile.selected = true;
            
            if (_hasSetupMissile) return;
            
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
            
            _seekerIconer = missile.seekerHead.AddComponent<ForwardUiIcon>();
            _seekerIconer.icon = UIController.Instance.MissileLockRect;
            _gimbalIconer = missile.AddComponent<ForwardUiIcon>();
            _gimbalIconer.icon = UIController.Instance.missileGimbalCircle;

            _hasSetupMissile = true;

        }
    }
}