using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.FPS
{
    [RequireComponent(typeof(Rigidbody))]
    public class Knife : MonoBehaviour
    {
        [SerializeField] private float _gravity = -9.82f;
        [SerializeField] private Vector3 _gravityDirection;
        private Transform _target;
        private Rigidbody _body;
        private Vector3 _velocity;

        public Vector3 InitialVelocity { set { _velocity = value; } }

        private Vector3 GravityDirection
        {
            get { return _gravityDirection; }
            set
            {
                _gravityDirection = value;
                _gravityDirection.Normalize();
            }
        }

        private void Start()
        {
            _body = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_target == null)
            {
                _velocity += GravityDirection * _gravity * Time.deltaTime;
                transform.position += _velocity * Time.deltaTime;
                transform.localRotation = Quaternion.LookRotation(_velocity.normalized);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _target = collision.transform;
            transform.parent = _target;
        }
    }
}