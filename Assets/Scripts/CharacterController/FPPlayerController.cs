using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

namespace FPController
{

    /**
 * Simple FPS character controller super class build based on rigidbodies
 * taken from https://github.com/nupsi/FPController/blob/master/FPController/Assets/FPController/Script/Controller/FirstPersonController.cs
 * edits:
 *     - removed crouching
 */
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
    public class FPPlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("_settings")] [SerializeField]
        private FirstPersonConfiguration settings;
        
        // Physics
        private CapsuleCollider _collider;
        private PhysicMaterial _physicMaterial; // Used for managing friction and stuff
        private Rigidbody _rigidbody;
        private Vector3 _previousForce; // The previous force added during Move()
        private Vector3 _lastGround; //  The previous force added during Move() while grounded
        private float _targetSpeed = 5;

        // Camera
        private Camera _camera;
        private Vector3 _cameraRotation;
        private Vector3 _targetPosition; // The cameras target position
        
        // Use default settings if they are not set
        public FirstPersonConfiguration Settings =>
            settings
                ? settings
                : (settings = ScriptableObject.CreateInstance<FirstPersonConfiguration>());

        // Is the controller on the ground
        private bool Grounded => 
            Physics.SphereCast(new Ray(GroundSphereOffset, Vector3.down), SafeRadius, 0.1f);

        // Returns the center of the lower 'sphere' of the capsule collider
        private Vector3 GroundSphereCenter =>
            new Vector3(transform.position.x, transform.position.y + Settings.Radius, transform.position.z);

        // GroundSphereCenter with offset applied
        private Vector3 GroundSphereOffset => 
            GroundSphereCenter + (Vector3.up * (Settings.Radius * 0.1f));
        
        // Calculate camera offset 
        private Vector3 CameraOffset =>
            transform.position + (Vector3.up * (_collider.height - 0.1f));

        private float SafeRadius => 
            Settings.Radius * 0.95f;

        // Calculate the angle of the surface below the controller
        private float SurfaceAngle
        {
            get
            {
                var angle = 0f;
                RaycastHit hit;
                if (Physics.SphereCast(GroundSphereOffset, Settings.Radius, Vector3.down, out hit, 0.05f))
                {
                    angle = Vector3.Angle(Vector3.up, hit.normal);
                }

                return Mathf.Round(angle);
            }
        }

        private void Awake()
        {
            Reset();
            // Setup camera
            _camera.transform.parent = null; // Clear the cameras parents
            _cameraRotation = _camera.transform.eulerAngles; // Store the cameras initial rotation
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected void Update()
        {
            // Update the camera each frame
            UpdateCamera();
        }

        protected void FixedUpdate()
        {
            // set cameras target position
            _targetPosition = CameraOffset;
        }

        private void OnDisable()
        {
            if (_camera != null)
            {
                // Disable the camera when the controller is
                _camera.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (_camera != null)
            {
                // Enable the camera when the controller is
                _camera.gameObject.SetActive(true);
                // Update the cameras position after activation
                SetCamera();
            }
        }

        private void Reset()
        {
            // Cache and Reset
            this.name = Settings.Name;
            // this.tag = Settings.Tag;

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
            _rigidbody.mass = 10f;

            _collider = GetComponent<CapsuleCollider>();
            _collider.radius = Settings.Radius;
            _collider.material = _physicMaterial = new PhysicMaterial
            {
                name = String.Format("{0}PhysicMaterial", name),
                dynamicFriction = 0,
                staticFriction = 0,
                frictionCombine = PhysicMaterialCombine.Minimum
            };

            ChangeHeight(Settings.Height);

            _targetSpeed = Settings.WalkSpeed;

            _camera = GetComponentInChildren<Camera>();
            _camera.fieldOfView = 70;
            _camera.nearClipPlane = 0.01f;
            _camera.farClipPlane = 250f;
            _camera.transform.localPosition = Vector3.up * (Settings.Height - 0.1f);
            _camera.name = string.Format("{0} {1}", this.name, "Camera");
        }

        // Move input handling
        // - Move rigidbody based on given horizontal and vertical input, clamp assumes input is between -1 and 1
        public void Move(float horizontal, float vertical, bool clamp = true)
        {
            if (clamp)
            {
                horizontal = Mathf.Clamp(horizontal, -1, 1);
                vertical = Mathf.Clamp(vertical, -1, 1);
            }

            Move(new Vector3(horizontal * _targetSpeed, 0, vertical * _targetSpeed));
        }

        // Move rigidbody based on given force
        public void Move(Vector3 inputVector)
        {
            if (_rigidbody == null)
                return;

            // Calculate the force for the state of controller
            var force = Grounded ? GroundControl(inputVector) : AirControl(inputVector, _previousForce);
            force.y = 0;

            // Track the rigidbodies velocity
            var velocity = _rigidbody.velocity;
            velocity.y = 0;

            // Calculate difference between current and target velocity
            var change = Vector3.ClampMagnitude(force, _targetSpeed) - Vector3.ClampMagnitude(velocity, _targetSpeed);
            // Apply the force to the velocity
            _rigidbody.AddForce(change, ForceMode.Impulse);

            // Update the friction to allow sliding
            UpdateFriction(inputVector);
            StickToGround();

            // store the force for the next update// InverseTransformDirection = transforms vector from world to local space (where the localspace determined from the transform)
            _previousForce = transform.InverseTransformDirection(force);
        }

        // Calculate the force of the controller when the it is on the ground
        private Vector3 GroundControl(Vector3 inputVector)
        {
            _lastGround = _previousForce;
            if (inputVector == Vector3.zero) // If there is no input slow down the controller
            {
                return -(_rigidbody.velocity);
            }

            // Convert the resulting forward vector to worldspace
            return transform.TransformDirection(inputVector);
        }

        // Calculate the force of the controller when the it is on the air
        private Vector3 AirControl(Vector3 inputVector, Vector3 force)
        {
            // Previous force determines how the new force is applied
            force = force == Vector3.zero ? inputVector : inputVector + (inputVector * 0.5f);
            
            // Lerp force to 0 if none is given
            if (inputVector.x == 0)
            {
                force.x = Mathf.Lerp(force.x, 0, 2 * Time.fixedDeltaTime);
            }

            if (inputVector.z == 0)
            {
                force.z = Mathf.Lerp(force.z, 0, 2 * Time.fixedDeltaTime);
            }
            
            // Convert the resulting forward vector to worldspace
            return transform.TransformDirection(inputVector);
        }
        
        // Movements
        public void Jump()
        {
            if (Grounded)
            {
                _rigidbody.AddForce(0, Settings.JumpForce, 0, ForceMode.VelocityChange);
            }
        }
        
        // Change the physics material based on the ground angle i.e slide if too steep
        private void UpdateFriction(Vector3 inputVector)
        {
            if (!Grounded)
            {
                ZeroFriction();
            }

            if (SurfaceAngle > 0 && SurfaceAngle <= Settings.MaxSlopeAngle)
            {
                if (inputVector == Vector3.zero)
                {
                    MaxFriction(Settings.MaxFriction);
                    return;
                }
                
                _rigidbody.AddForce(Vector3.down * Physics.gravity.y, ForceMode.Force);
            }

            ZeroFriction();
        }
        
        // Fix the controllers velocity to stay on ground when the surface angle changes
        private void StickToGround()
        {
            RaycastHit hit;
            if (Physics.SphereCast(GroundSphereCenter, SafeRadius, Vector3.down, out hit, 0.075f))
            {
                if (SurfaceAngle < Settings.MaxSlopeAngle)
                {
                    // Slow down the controller based on the slope angle?
                    _rigidbody.velocity -= Vector3.Project(_rigidbody.velocity, hit.normal);
                }
            }
        }
        
        // Change the physics material properties of the character controller
        private void ChangeFriction(float amount, PhysicMaterialCombine combine)
        {
            _physicMaterial.frictionCombine = combine;
            _physicMaterial.staticFriction = amount;
        }

        private void ZeroFriction()
        {
            ChangeFriction(0, PhysicMaterialCombine.Minimum);
        }

        private void MaxFriction(float amount)
        {
            ChangeFriction(amount, PhysicMaterialCombine.Maximum);
        }

        private void ChangeHeight(float height)
        {
            _collider.height = height;
            _collider.center = Vector3.up * (_collider.height / 2);
        }
        
        // Camera Stuff
        private void SetCamera()
        {
            _camera.transform.position = CameraOffset;
        }

        private void UpdateCamera()
        {
            // Check distance between camera and controller
            if (Vector3.Distance(transform.position, _camera.transform.position) > Settings.Height * 1.3f ||
                Mathf.Round(_rigidbody.velocity.magnitude * 10) / 10 <= 1.0f)
            {
                SetCamera();
                return;
            }
            
            // Move camera to controller
            var velocity = _rigidbody.velocity * 0.2f; // Calculate offset velocity
            var target = _targetPosition + velocity; // create new position based on target and velocity
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, target, 5 * Time.deltaTime); // lerp the camera to target position

        }

        public void MouseMove(float horizontal, float vertical)
        {
            MouseMove(new Vector2(horizontal, vertical));
        }

        private void MouseMove(Vector2 mouseInput)
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;
            
            transform.Rotate(new Vector3(0, mouseInput.x, 0));
            _cameraRotation.x = Mathf.Clamp(_cameraRotation.x + -mouseInput.y, -90, 90);
            _cameraRotation = new Vector3(_cameraRotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
            _camera.transform.eulerAngles = _cameraRotation;
        }


#if UNITY_EDITOR

        [SerializeField] private bool m_drawRays = false;

        private Vector3 debug_previousPosition;

        protected void OnDrawGizmos()
        {
            if (m_drawRays && UnityEditor.EditorApplication.isPlaying)
            {
                var radius = 0.05f;
                var root = transform.position;
                var top = root + Vector3.up * _collider.height;
                var cam = _targetPosition;
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(root, radius);
                Gizmos.DrawWireSphere(top, radius);
                Gizmos.DrawLine(root, top);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(CameraOffset, Vector3.one * radius);
                Gizmos.DrawLine(CameraOffset, _camera.transform.position);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(cam, radius);
                Gizmos.DrawLine(cam, _camera.transform.position);
                Gizmos.DrawWireCube(_camera.transform.position, (Vector3.one * radius) * 0.9f);

                Gizmos.color = Color.red;
                var velocity = _rigidbody.velocity * 0.2f;
                var target = _targetPosition + velocity;
                Gizmos.DrawWireSphere(target, radius);
                // Gizmos.DrawLine(target, _camera.transform.position);

                Debug.DrawLine(transform.position, debug_previousPosition, Color.blue, 2f);
                debug_previousPosition = transform.position;
            }
        }

#endif
    }

}