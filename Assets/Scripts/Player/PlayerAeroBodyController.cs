using System.Collections;
using Aerodynamics;
using TMPro;
using UnityEngine;
using Utility;

namespace Player
{
    public class PlayerAeroBodyController : MonoBehaviour
    {

        [SerializeField] private AeroBody aeroBody;
        [SerializeField] private Thruster engine;
        [SerializeField] private float throttleStep = 1f;
        [SerializeField] private Vector3 trim = Vector3.zero;
        [SerializeField] private Camera cam;
        [SerializeField] private Transform targetDirection;

        [SerializeField] private float controlSurfaceSpeed = 10f;

        [SerializeField] private GameObject thrusterA;
        [SerializeField] private GameObject thrusterB;
        [SerializeField] private float maxThrusterJiggleStrength = 0.0025f;
        [SerializeField] private float afterburnerThrottlePct = 0.7f;

        [SerializeField] private TextMeshProUGUI instructorText;
        [SerializeField] private TextMeshProUGUI geeForceText;
        
        private Transform _transform;
        private Vector3 _lastVelocity;
        private Vector3 _currentControl;
        
        private Material _thrusterAMaterial;
        private Material _thrusterBMaterial;

        private static readonly int Length = Shader.PropertyToID("_Length");
        private static readonly int JiggleStrength = Shader.PropertyToID("_JiggleStrength");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        private const float RollThreshold = 5;

        [SerializeField] private PIDController pid;

        private void Start()
        {
            _transform = transform;
            _thrusterAMaterial = thrusterA.GetComponent<Renderer>().material;
            _thrusterBMaterial = thrusterB.GetComponent<Renderer>().material;

            SetThrusterMaterialProperties();
        }

        private void Update()
        {

            Vector3 control = Vector3.zero;


            if (!Input.GetKey(KeyCode.C))
            {
                targetDirection.rotation = cam.transform.rotation;

                // mouse control
                float pitchAngle = Util.AngleAroundAxis(targetDirection.forward, _transform.forward, _transform.right);
                float yawAngle = Util.AngleAroundAxis(targetDirection.forward, _transform.forward, _transform.up);
                float rollAngle = Util.AngleAroundAxis(Vector3.down, -_transform.up, _transform.forward);

                control.x = pitchAngle * -0.08f;
                control.y = yawAngle * 0.06f;
                control.z = yawAngle switch
                {
                    > RollThreshold => yawAngle - RollThreshold,
                    < -RollThreshold => yawAngle + RollThreshold,
                    _ => -rollAngle / RollThreshold // auto level - should be somehow mixed with the other thing
                } * 0.04f;
            }
            else
                targetDirection.rotation = transform.rotation;
            
            
            // key control
            if (Input.GetKey(KeyCode.W))
                control.x += 1;
            if (Input.GetKey(KeyCode.S))
                control.x -= 1;
            
            if (Input.GetKey(KeyCode.Q))
                control.y += 1;
            if (Input.GetKey(KeyCode.E))
                control.y -= 1;
            
            if (Input.GetKey(KeyCode.A))
                control.z += 1;
            if (Input.GetKey(KeyCode.D))
                control.z -= 1;
            
            
            // pitch trim
            if (Input.GetKey(KeyCode.X))
            {
                if (Input.GetKey(KeyCode.W))
                    trim.x += 0.001f;
                else if (Input.GetKey(KeyCode.S))
                    trim.x -= 0.001f;
            }
            
            Vector3 localAngularVelocity = transform.InverseTransformDirection(aeroBody.rb.angularVelocity);

            Vector3 pidOut = pid.Update(-localAngularVelocity, Time.deltaTime);

            if (control.x == 0)
                control.x = pidOut.x;
            if (control.y == 0)
                control.y = pidOut.y;


            control += trim;

            Vector3 surfaceDirections = Vector3.zero;
            surfaceDirections.x = control.x > _currentControl.x ? 1 : -1;
            surfaceDirections.y = control.y > _currentControl.y ? 1 : -1;
            surfaceDirections.z = control.z > _currentControl.z ? 1 : -1;

            _currentControl += surfaceDirections * (controlSurfaceSpeed * Time.deltaTime);
            
            aeroBody.SetControl(_currentControl);

            if (Input.GetKey(KeyCode.LeftShift))
                engine.Throttle += throttleStep * Time.deltaTime;
            else if (Input.GetKey(KeyCode.LeftControl))
                engine.Throttle -= throttleStep * Time.deltaTime;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift))
                SetThrusterMaterialProperties();
            
            
            DrawInstructor();
        }


            
        

        private void FixedUpdate()
        {
            geeForceText.text = $"{Vector3.Project((aeroBody.rb.velocity - _lastVelocity) * Util.InverseFDeltaTime, -_transform.up).magnitude / 9.8:F2} G";

            _lastVelocity = aeroBody.rb.velocity;
        }

        // mach number calculation constants
        private const float A0 = 340.3f; // speed of sound at sea level
        private const float K = 0.0041f; // rate speed of sound decreases with alt
        
        private void DrawInstructor()
        {
            float speed = aeroBody.rb.velocity.magnitude;
            
            instructorText.text = 
                $"THR    {(int)(engine.Throttle * 100)} %\n" +
                $"SPD    {(int)(speed * 3.6f)} km/h\n" +
                $"MACH   {speed / (A0 - K * _transform.position.y):F2}\n" +
                $"ALT    {(int)_transform.position.y} m";
        }

        private void SetThrusterMaterialProperties()
        {
            // i dont like how im repeating things here
            if (engine.Throttle < afterburnerThrottlePct && thrusterA.activeSelf)
            {
                thrusterA.SetActive(false);
                thrusterB.SetActive(false);
            }
            else if (engine.Throttle >= afterburnerThrottlePct)
            {
                if (!thrusterA.activeSelf)
                {
                    thrusterA.SetActive(true);
                    thrusterB.SetActive(true);
                }
                
                _thrusterAMaterial.SetFloat(Length, 1 - engine.Throttle * 0.5f);
                _thrusterAMaterial.SetFloat(Alpha, (engine.Throttle - afterburnerThrottlePct) / (1 - afterburnerThrottlePct));
                _thrusterAMaterial.SetFloat(JiggleStrength, maxThrusterJiggleStrength * engine.Throttle);

                _thrusterBMaterial.SetFloat(Length, 1 - engine.Throttle * 0.5f);
                _thrusterBMaterial.SetFloat(Alpha, (engine.Throttle - afterburnerThrottlePct) / (1 - afterburnerThrottlePct));
                _thrusterBMaterial.SetFloat(JiggleStrength, maxThrusterJiggleStrength * engine.Throttle);
            }
        }
    }
}
