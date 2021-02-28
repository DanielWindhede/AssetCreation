using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.FPS
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        private CharacterController _controller;
        private FPSInput _input;

        private bool _isGrounded;
        private float _pitchRotation;
        private Vector2 _mouseDelta;
        private Vector2 _moveDirection;
        private Vector3 _velocity;

        [SerializeField] private float _gravity = 9.82f;
        [SerializeField] private Vector3 _gravityDirection;
        public Vector3 GravityDirection
        {
            get { return _gravityDirection; }
            set
            {
                _gravityDirection = value;
                _gravityDirection.Normalize();
            }
        }

        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _groundCheckPosY;
        [SerializeField] private float _groundCheckRadius;
        Vector3 GroundCheckSpherePosition { get { return transform.position + Vector3.up * _groundCheckPosY; } }

        [SerializeField] private float _runSpeed = 10f;
        [SerializeField] private float _mouseSensitivity = 5;

        void Awake()
        {
            _input = new FPSInput();
            _camera = Camera.main;
            _controller = GetComponent<CharacterController>();
            _velocity = new Vector3();
            GravityDirection = _gravityDirection;
            //input.FPS.MoveDirection.performed += x => moveDirection = x.ReadValue<Vector2>();
            //input.FPS.MouseDelta.performed += x => mouseDelta = x.ReadValue<Vector2>();
        }

        public void SetPosition(Vector3 position)
        {
            _controller.Move(transform.position - position);
        }

        public void SetVelocity(Vector3 velocity)
        {
            _velocity = velocity;
        }

        public void Move(Vector3 motion)
        {
            _controller.Move(motion * Time.deltaTime);
        }

        public void DoUpdate()
        {
            HandleCamera();
            DoGroundCheck();
            HandleMovement();
        }

        private void DoGroundCheck()
        {
            // Character controller is a capsule shape hence bottom sphere
            _isGrounded = Physics.CheckSphere(GroundCheckSpherePosition, _groundCheckRadius, _groundMask, QueryTriggerInteraction.UseGlobal); 
        
            if (_isGrounded && _velocity.normalized == _gravityDirection)
            {
                _velocity = Vector3.zero;
            }
        }

        private void HandleMovement()
        {
            _moveDirection = _input.FPS.MoveDirection.ReadValue<Vector2>();

            Vector3 direction = transform.forward * _moveDirection.y +
                                transform.right   * _moveDirection.x;

            _controller.Move(direction * _runSpeed * Time.deltaTime);


            //_velocity += GravityDirection * _gravity * Time.deltaTime;
            _velocity += Vector3.down * _gravity * Time.deltaTime;

            _controller.Move(_velocity * Time.deltaTime);
        }

        private void HandleCamera()
        {
            _mouseDelta = _input.FPS.MouseDelta.ReadValue<Vector2>();

            float mouseX = _mouseDelta.x * _mouseSensitivity * Time.deltaTime;
            float mouseY = _mouseDelta.y * _mouseSensitivity * Time.deltaTime;

            _pitchRotation -= mouseY;
            _pitchRotation = Mathf.Clamp(_pitchRotation, -90f, 90f);


            _camera.transform.localRotation = Quaternion.Euler(_pitchRotation, 0, 0);

            // by rotating the entire player, camera yaw rotation is not needed
            transform.Rotate(Vector3.up * mouseX); 
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GroundCheckSpherePosition, _groundCheckRadius);
        }
    }
}