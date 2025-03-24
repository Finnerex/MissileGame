using Hangar;
using Missiles;
using UnityEngine;
using Utility;

namespace Player
{
    public class PlayerWeaponsSystemController : MonoBehaviour
    {

        [SerializeField] private WeaponsSystem weaponsSystem;
        [SerializeField] private float missileCircleStayTime = 5;
        [SerializeField] private OrbitCamera orbitCam;

        private float _currentCircleTime;
        private bool _shouldCountDown;
        
        private Transform _lastFiredMissile;
        private bool _focusedMissile;
        private Camera _cam;
        
        private void Awake()
        {
            foreach (var pair in SceneChangeDataManager.Instance.WeaponSystemMissiles)
                weaponsSystem.SetMissile(pair.Value, pair.Key);
        }

        private void Start()
        {
            _cam = Camera.main;
            
            SetMissileText();
        }

        private void Update()
        {
         
            if (Input.GetKeyDown(KeyCode.Space))
                weaponsSystem.DeployFlares();
            
            // focus view missile
            // if the missile gets destroyed whilst being looked at, _focusedMissile will be true despite bing focused on the player. i dont want to fix this
            if (Input.GetKeyDown(KeyCode.Slash) && _focusedMissile)
            {
                orbitCam.ResetView();
                _focusedMissile = false;
            }
            else if (Input.GetKeyDown(KeyCode.Slash) && _lastFiredMissile != null)
            {
                orbitCam.SwitchTo(_lastFiredMissile);
                _focusedMissile = true;
            }

            bool hasNext = weaponsSystem.TryGetNext(out Missile nextMissile);
            if (hasNext)
                IfHasNext(nextMissile); // things in this method do not modify the state of the weapons system
            
            // These modify has next
            // fire missile if locked
            if (Input.GetKeyDown(KeyCode.Mouse4) && hasNext && nextMissile.hasLock && nextMissile.selected)
            {
                
                _lastFiredMissile = nextMissile.transform;
                
                weaponsSystem.FireNext();
                SetMissileText();

                hasNext = weaponsSystem.TryGetNext(out nextMissile);
                if (hasNext)
                {
                    SelectMissile(nextMissile);
                    DrawMissileCircleUI(nextMissile); // new missile, new ui
                }
                else
                {
                    UIController.Instance.missileGimbalCircle.gameObject.SetActive(false);
                    UIController.Instance.MissileLockRect.gameObject.SetActive(false);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse3))
            {
                if (hasNext)
                    nextMissile.Deselect();
                
                weaponsSystem.CycleMissile();
                
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(false);
                UIController.Instance.MissileLockRect.gameObject.SetActive(false);
                
                SetMissileText();
            }

        }

        private void IfHasNext(Missile next)
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
                next.Unlock();
            
            // selecting the missile
            if (Input.GetKeyDown(KeyCode.Mouse4) && !next.selected)
            {
                SelectMissile(next);
                
                _currentCircleTime = missileCircleStayTime;
                _shouldCountDown = true;
            }
            
            // count down the timer until it gets auto deselected
            if (_shouldCountDown && _currentCircleTime > 0)
            {
                if (next.hasLock)
                    _currentCircleTime = missileCircleStayTime;
                else
                    _currentCircleTime -= Time.deltaTime;
            }
            else if (_shouldCountDown)
            {
                _shouldCountDown = false;
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(false);
                UIController.Instance.MissileLockRect.gameObject.SetActive(false);
                next.Deselect();
            }
            
            // UI stuff if missile is selected
            if (next.selected)
                DrawMissileCircleUI(next);
        }

        private void SetMissileText()
        {
            UIController.Instance.selectedMissileText.text =
                $"{weaponsSystem.missileCount} | {weaponsSystem.currentMissileQueue.Count} {weaponsSystem.currentMissileName}";
        }

        private void DrawMissileCircleUI(Missile nextMissile)
        {
            UIController.Instance.missileLockCircle.color = nextMissile.hasLock
                ? new Color(0.8f, 0.28f, 0.2f)
                : new Color(0.75f, 0.75f, 0.7f);
            
            Transform missileTransform = nextMissile.transform;
            Vector3 screenPoint = _cam.WorldToScreenPoint(missileTransform.position + missileTransform.forward * 500 /* distance */);
            if (screenPoint.z >= 0) 
                UIController.Instance.missileGimbalCircle.position = screenPoint;
            
            screenPoint = _cam.WorldToScreenPoint(nextMissile.seekerHead.position + nextMissile.seekerHead.forward * 500 /* distance */);
            if (screenPoint.z >= 0) 
                UIController.Instance.MissileLockRect.position = screenPoint;
        }
        
        
        private void SelectMissile(Missile missile)
        {
            missile.Select();

            float circleDiameter = 2 * Mathf.Tan(missile.seeker.fieldOfView * 0.5f * Mathf.Deg2Rad) * 500;
            UIController.Instance.MissileLockRect.sizeDelta = new Vector2(circleDiameter, circleDiameter);
            UIController.Instance.MissileLockRect.gameObject.SetActive(true);

            if (missile.seeker.isCaged)
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(false);
            else
            {
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(true);
                circleDiameter = 2 * Mathf.Tan((missile.seeker.maxGimbalAngle + missile.seeker.fieldOfView) * 0.5f * Mathf.Deg2Rad) * 500;
                UIController.Instance.missileGimbalCircle.sizeDelta = new Vector2(circleDiameter, circleDiameter);
            }
            
        }
        
    }
}